using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


namespace PowerInject { 
public interface IPipelineLogger
    {
    void logException(String message,Exception e);
    void logWarning(object message);
    void logMessage(object message);
    }
}

