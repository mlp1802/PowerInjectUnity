using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace PowerInject
{
    class ConsoleLogger:IPipelineLogger
    {
        public void logException(String message,Exception e) 
        {
            logMessage(message);
            Console.WriteLine(e.StackTrace.ToString());
        }
        public void logMessage(object message)
        {
            Console.WriteLine(message.ToString());
        }
        public void logWarning(object message)
        {
            Console.WriteLine(message.ToString());
        }
    }
}
