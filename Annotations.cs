using System;

namespace PowerInject
{
    [AttributeUsage(AttributeTargets.Property| AttributeTargets.Field | AttributeTargets.Constructor, AllowMultiple = true)]
    public class Inject : System.Attribute
    {
    }


    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public class NewInstance : System.Attribute
    {
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    public class Power : System.Attribute
    {
    }

    [AttributeUsage(AttributeTargets.Method, AllowMultiple = false)]
    public class Module : System.Attribute
    {
        public String Named { get; set; }
    }
    [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
    public class Produce : System.Attribute
    {
        public String Named { get;set; }
    }
    [AttributeUsage(AttributeTargets.Parameter|AttributeTargets.Field, AllowMultiple = false)]
    public class Named : System.Attribute
    {
        public String name;
        public Named(String name) { 
            this.name = name;
        }
    }
    [AttributeUsage(AttributeTargets.Parameter | AttributeTargets.Field, AllowMultiple = false)]
    public class Typed : System.Attribute
    {
        public Type type;
        public Typed(Type type)
        {
            this.type = type;
        }
    }
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = true)]
    public class Insert : System.Attribute
    {
        public Type Typed { get; set; }
        public String Named { get; set; }
    }
    
    public class OnInjected : System.Attribute
    {
    }

}