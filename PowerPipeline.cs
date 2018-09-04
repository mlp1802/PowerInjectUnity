using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Linq;


namespace PowerInject
{
    public class PowerPipeline : MonoBehaviour
    {

        protected PipelineContainer pipeline;
        protected List<MonoBehaviour> getChildren(GameObject g)
        {
            if (!g.activeSelf) { 
                return new List<MonoBehaviour>();
            }
            var components = g.GetComponents<MonoBehaviour>().Where(x=>x!=null).Where(c => c.GetType() != typeof(PowerPipeline)).ToList();//.Where(c => ReflectionUtils.isAnnotatedWith(c, typeof(Module))).ToList();
            var children = Utils.getChildren(g);
            children.ForEach((childGameObject) =>
            {
                if (childGameObject != g && childGameObject.GetComponent<PowerPipeline>() == null)
                {
                    components.AddRange(getChildren(childGameObject));
                }
            });
            return components.Where(x=>x.enabled).ToList();
        }

        protected List<MonoBehaviour> getBehaviors()
        {
            return getChildren(gameObject);
        }
        // Update is called once per frame

        public PowerPipeline getParentPipeline()
        {
            var parentTransform = gameObject.transform.parent;
            if (parentTransform != null)
            {
                return Utils.getFirstComponentInParent<PowerPipeline>(parentTransform.gameObject);
            }
            else
            {
                return null;
            }
        }

        bool didRun = false;
        
        void run()
        {
            var parentPipeline = getParentPipeline();
            if (parentPipeline == null)
            {
                run(new Dictionary<string, FinalValue>());
            }

        }

        void Start()
        {
            run();
        }
        public void run(Dictionary<string, FinalValue> arguments)
        {
            if (!didRun)
            {
                didRun = true;
                pipeline = new PipelineContainer(new UnityLogger());
                pipeline.addToFinalObjects(arguments);
                var behaviors = getBehaviors().Where(b => ReflectionUtils.isAnnotatedWith<Power>(b) || ReflectionUtils.isAnnotatedWith<Insert>(b)).ToList();
                addToProducers(behaviors);
                pipeline.run();
                runChildpipelines();
            }
        }
        protected void runChildpipelines()
        {
            var childPipeLines = getChildPipelines(gameObject);
            childPipeLines.ForEach((p) => p.run(pipeline.getFinalObjectsMap()));
        }
        protected void addToProducers(List<MonoBehaviour> behaviors)
        {
            behaviors.ForEach(x =>
            {
                pipeline.add(x);
            });
        }



        protected List<PowerPipeline> getChildPipelines(GameObject g)
        {

            List<PowerPipeline> result = new List<PowerPipeline>();
            var pipeline = g.GetComponent<PowerPipeline>();
            if (pipeline != null && g != gameObject)
            {
                result.Add(pipeline);
            }
            else
            {
                var children = Utils.getChildren(g);
                children.ForEach(c =>
                {
                    result.AddRange(getChildPipelines(c));
                });
            }
            return result;

        }

    }
}