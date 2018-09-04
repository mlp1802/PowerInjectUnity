using UnityEngine;
using System.Collections;
using PowerInject;
[Power]
public class ExecutionA : MonoBehaviour {


    void Awake()
    {
        MonoBehaviour.print("AwakeA");
    }
    void Start() {
        MonoBehaviour.print("StartA");
    }
    
    [Produce]
    float produceA_2()
    {
        MonoBehaviour.print("produceA_2");
        return 0;
    }
    [Produce]
    string produceA_1() {
        MonoBehaviour.print("produceA_1");
        return "";
    }
    [OnInjected]
    public void injectedA() {
        MonoBehaviour.print("Injected_A");
    }
    bool ranupdate = false;
    bool ranFixedUpdate = false;
    void Update()
    {
        if (!ranupdate)
        {
            MonoBehaviour.print("Update_A");
            ranupdate = true;
        }
    }
    void FixedUpdate()
    {
        if (!ranFixedUpdate)
        {
            MonoBehaviour.print("FixedUpdate_A");
            ranFixedUpdate = true;
        }
    }
    
}
