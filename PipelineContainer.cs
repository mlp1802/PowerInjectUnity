using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;



namespace PowerInject
{
    public interface Injector {
        void inject(object o);
    }
    [Insert (Typed=typeof(Injector))]
    public class PipelineContainer:Injector
    {
        List<Producer> producers = new List<Producer>();
        List<object> toBeInjected = new List<object>();

        Dictionary<String, FinalValue> finalObjects = new Dictionary<String, FinalValue>();
        IPipelineLogger logger = new ConsoleLogger();
        protected int index;
        public PipelineContainer()
        {
        }
        
        public PipelineContainer(IPipelineLogger logger)
            : this()
        {
            this.logger = logger;
        }
        
        protected String getWarning(String message)
        {
            return "PipelineIOC: " + message;
        }
        
        protected Exception getException(String message)
        {

            return new Exception(getWarning(message));
        }

        public T getFinalObject<T>()
        {
            var foundList = finalObjects.Values.Where(o => ReflectionUtils.sameType(typeof(T), o.obj.GetType()));
            if (foundList.Count() == 0)
            {

                return default(T);
            }
            else
            {
                return (T)foundList.First().obj;
            }
        }

        protected void addFinalObject(String key, FinalValue o,Boolean shouldBeInjected)
        {
            
            
            if (o.obj != null) 
            { 
                finalObjects[key] = o;
                if (shouldBeInjected) {
                    toBeInjected.Add(o.obj);
                }
                
            }
        }

        public Dictionary<String, FinalValue> getFinalObjectsMap()
        {
            return finalObjects;
        }

        public void addToFinalObjects(Dictionary<String, FinalValue> values)
        {
            values.Keys.ToList().ForEach(x => addFinalObject(x, values[x], false));
        }

        protected List<object> getFinalObjectsThatFitsParameters(List<Parameter> parameters)
        {

            return parameters.Select(param => getFinalObjectByKey(param.key.getCode())).Where(a => a != null).Select(f => f.obj).ToList();
        }

        protected void callProducer(Producer producer, List<object> values)
        {
            var returnValue = producer.invoke(values.ToArray());
            addFinalObject(producer.invokeable.getKey().getCode(), new FinalValue(producer.invokeable.getKey(), returnValue),true);

        }
        
        protected Producer runProducer(Producer producer)
        {
            try
            {
                var parameters = producer.getParameters();
                var values = getFinalObjectsThatFitsParameters(parameters);
                if (values.Count == parameters.Count)
                {
                    callProducer(producer, values);
                    return producer;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                logger.logException("Producer failed with: ", e);
                return null;
            }

        }

        public List<Producer> getProducers()
        {
            return producers;
        }

        public List<Producer> getSortedProducers()
        {
            producers.Sort();
            return producers;
        }

        public List<Producer> runProducers(List<Producer> producers)
        {
            return producers.Select(p => runProducer(p)).ToList();
        }

        public FinalValue getFinalObjectByKey(IKey key)
        {
            return getFinalObjectByKey(key.getCode());
        }
        public FinalValue getFinalObjectByKey(String name)
        {
            if (finalObjects.ContainsKey(name))
            {
                return finalObjects[name];
            }
            else
            {
                return null;
            }
        }

        protected void addNewProducer(object orgObject, Invokeable inv)
        {
            producers.Add(new Producer(index, orgObject, inv));
            index++;
        }

        public void printFinalObjects() {
            MonoBehaviour.print("FInAL OBJECTS");
            finalObjects.Keys.ToList().ForEach(p =>
            {
                var v = finalObjects[p];
                MonoBehaviour.print(p + "->" + v);
            });
        }
        protected void extractProducers(object o) {
            ReflectionUtils.extractProducers(o).ForEach(p => addNewProducer(o,p));
        }
        
        
        protected void extractInsertKeys(object o) 
        {

            var keys = KeyProducer.getInsertKeys(o);
            if (keys.Count() > 0)
            {
                keys.ForEach(key =>
                {
                    addFinalObject(key.getCode(), new FinalValue(key, o),true);

                });
            }
            else
            {
                toBeInjected.Add(o);
            }
        }

        public void add(object obj)
        {
            ReflectionUtils.extractModules(obj).ForEach(x => add(x));//.ForEach(add);
            extractProducers(obj);
            extractInsertKeys(obj);
        }

        protected System.Type getProducedObjectType(object b)
        {
            var annotation = ReflectionUtils.getAttributeInfo<Insert>(b);
            if (annotation != null)
            {
                if (annotation.Typed != null)
                {
                    return annotation.Typed;

                }
                else
                {
                    return b.GetType();
                }
            }
            else
            {
                return b.GetType();
            }
        }

        public void injectField(object obj, Field field)
        {
            var value = getFinalObjectByKey(field.key.getCode());
            if (value != null)
            {
                try
                {
                    field.fieldInfo.SetValue(obj, value.obj);
                }
                catch (Exception e)
                {
                    logger.logException("Injection on object " + obj + " failed, field " + field.fieldInfo.Name, e);
                }
            }
            else
            {
                logger.logWarning("Could not find object for field '" + field.getName() + "' on object " + obj.GetType().FullName);
            }
        }
        public void injectProperty(object obj, Property property)
        {
            var value = getFinalObjectByKey(property.key.getCode());
            if (value != null)
            {
                try
                {
                    property.propertyInfo.SetValue(obj, value.obj,null);
                }
                catch (Exception e)
                {
                    logger.logException("Injection on object " + obj + " failed, property " + property.getName(), e);
                }
            }
            else
            {
                logger.logWarning("Could not find object for property '" + property.getName() + "' on object " + obj.GetType().FullName);
            }
        }

        //**********
        public object createObject(Type parentObjectType, ConstructorInfo info)
        {
            if (info.DeclaringType == parentObjectType)
            {
                return null;
            }
            else
            {
                var cparams = info.GetParameters().ToList();
                var values = ReflectionUtils.getParameters(cparams).Select(p => getFinalObjectByKey(p.key)).Where(fv => fv != null).Select(fv => fv.obj).ToList();
                if (values.Count() == cparams.Count())
                {
                    return info.Invoke(values.ToArray());
                }
                else
                {
                    return null;
                }
            }



        }
        public void createNewInstance(object obj, FieldInfo field)
        {
            var constructors = ReflectionUtils.getConstructors(field.FieldType);
            Boolean notFound = true;
            var index = 0;
            var count = constructors.Count();
            object newInstance = null;
            for (int i = 0; i < count; i++)
            {
                var constructor = constructors[i];
                newInstance = createObject(obj.GetType(), constructor);
                if (newInstance != null)
                {
                    break;
                }
            }
            if (newInstance != null)
            {
                field.SetValue(obj, newInstance);
                injectFields(newInstance);
                createNewInstances(newInstance);
                onInjected(newInstance);
            }

        }


        protected void createNewInstances(object obj) 
        {
            try
            {
                var fieldsToCreate = ReflectionUtils.getNewInstances(obj);
                fieldsToCreate.ForEach(field => createNewInstance(obj, field));
            }
            catch (Exception e)
            {
                logger.logException("Injection on object " + obj.GetType().FullName + "failed ", e);
            }
        }
        protected void injectFields(object obj)
        {
            try
            {
                var fields = ReflectionUtils.getFieldsToInject(obj);
                fields.ForEach(field => injectField(obj, field));
            }
            catch (Exception e)
            {
                logger.logException("Injection on object " + obj.GetType().FullName + "failed ", e);
            }
        }
        protected void injectProperties(object obj)
        {
            try
            {
                
                var fields = ReflectionUtils.getPropertiesToInject(obj);
                fields.ForEach(field => injectProperty(obj, field));
            
            }
            catch (Exception e)
            {
                logger.logException("Injection on object " + obj.GetType().FullName + "failed ", e);
            }
        }       
        public List<Producer> getProducersWithNoArguments()
        {
            return producers.Where(p => p.getArgumentTypeNames().Count() == 0).ToList();
        }

        public List<Producer> loop(List<Producer> producers)
        {
            var ran = runProducers(producers).Where(r => r != null).ToList();
            if (ran.Count() > 0)
            {
                var leftTorun = producers.Where((x) => !ran.Contains(x)).ToList();
                return loop(leftTorun);
            }
            else 
            {
                var leftTorun = producers.Where((x) => !ran.Contains(x)).ToList();
                return leftTorun;
            }
        }

        public List<FinalValue> GetFinalObjects()
        {
            return finalObjects.Values.ToList();
        }
        protected void onInjected(object m)
        {
            
            try
            {
                var methods = ReflectionUtils.getOnInjectedMethods(m);
                
                methods.ForEach(method =>
                {
                    var cparams = method.GetParameters().ToList();
                    var values = ReflectionUtils.getParameters(cparams).Select(p => getFinalObjectByKey(p.key)).Where(fv => fv != null).Select(fv => fv.obj).ToList();
                    method.Invoke(m, values.ToArray());
                    //logger.logMessage("ON injected on " + m.ToString());
                });
            }
            catch (Exception e)
            {
                logger.logException("OnInjection call on object :" + m.GetType().FullName + " failed", e);
            }
        }

        protected void onInjected()
        {
            toBeInjected.ForEach(o => onInjected(o));
        }


        public void inject(object obj)
        {
            injectInternal(obj);
            onInjected(obj);
        }
        protected void injectInternal(object obj) {
            injectFields(obj);
            injectProperties(obj);
            createNewInstances(obj);
            //onInjected(obj);
        }
        protected void injectAll()
        {
            toBeInjected.ForEach(obj => injectInternal(obj));
            onInjected();

        }

        public void run()
        {
            add(this);
            producers.Sort();
            var ran = runProducers(getProducersWithNoArguments());
            var leftTorun = producers.Where((x) => !ran.Contains(x)).ToList();
            leftTorun = loop(leftTorun);
            leftTorun.ForEach(p =>
            {
                logger.logWarning("Producer didn't run '"+p.getName()+"'");
            });
            injectAll();
            
        }
    }
}