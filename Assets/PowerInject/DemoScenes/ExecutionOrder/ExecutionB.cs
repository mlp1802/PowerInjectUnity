using UnityEngine;
using System.Collections;
using PowerInject;
[Power]
public class ExecutionB : MonoBehaviour {



    void Awake()
    {
        MonoBehaviour.print("AwakeB");
    }
    void Start()
    {
        MonoBehaviour.print("StartB");
    }
    [Produce]
    string produceB_1()
    {
        MonoBehaviour.print("produceB_1");
        return "";
    }
    [Produce]
    float produceB_2()
    {
        MonoBehaviour.print("produceB_2");
        return 0;
    }
    [OnInjected]
    public void injectedA()
    {
        MonoBehaviour.print("Injected_B");
    }
    bool ranupdate = false;
    bool ranFixedUpdate = false;
    void Update() {
        if (!ranupdate) {
            MonoBehaviour.print("Update_B");
            ranupdate = true;
        }
    }
    void FixedUpdate()
    {
        if (!ranFixedUpdate)
        {
            MonoBehaviour.print("FixedUpdate_B");
            ranFixedUpdate = true;
        }
    }
}
