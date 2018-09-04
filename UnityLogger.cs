using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;
namespace PowerInject
{
    class UnityLogger : IPipelineLogger
    {
        public void logException(String message,Exception e)
        {
            logMessage(message);
            Debug.LogException(e);
        }

        public void logMessage(object message)
        {
            MonoBehaviour.print("PipelineInject: "+message);
        }
        public void logWarning(object message)
        {
            Debug.LogWarning("PipelineInject: " + message);
        }
    }
}
