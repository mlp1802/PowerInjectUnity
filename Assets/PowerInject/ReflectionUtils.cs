using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using System.Linq;
namespace PowerInject
{
    public class ReflectionUtils
    {
        public static Boolean sameType(Type t2, Type t1)
        {

            return t1.IsAssignableFrom(t2);

        }
        public static List<Type> getDerivedClasses(Type t)
        {
            var result = new List<Type>();
            if (t != null)
            {
                result.Add(t);
                result.AddRange(getDerivedClasses(t.BaseType));

            }
            return result;

        }
        public static List<String> printMethods(object function)
        {
            var methods = function.GetType().GetMethods();
            methods.ToList().ForEach((m) => printMethod(m));
            return null;
        }

        public static void printMethod(MethodInfo info)
        {
            Console.Write(info.Name + " -> ");
            info.GetParameters().ToList().ForEach((p) => Console.Write(p.Name));
            Console.WriteLine();

        }

        public static ConstructorInfo getDefaultConstructor(Type t) 
        {
            return t.GetConstructor(Type.EmptyTypes);
        }
        public static List<ConstructorInfo> getConstructors(Type t)
        {
            var injectConstructors = t.GetConstructors().Where(c => getAttributeInfo<Inject>(c) != null).OrderByDescending(c=>c.GetParameters().Count()).ToList();
            var defaultConstructor = getDefaultConstructor(t);
            if (defaultConstructor != null) 
            {
                injectConstructors.Add(defaultConstructor);
            }
            
            return injectConstructors;

        }

        public static List<FieldInfo> getFieldsAnnotatedWith(Type t,Type annotatedType)
        {
            
            var members = getFields(t).Where(m =>
            {

                var alist = m.GetCustomAttributes(false).ToList();
                var filtered = alist.Where(a => a.GetType() == annotatedType).ToList();
                return filtered.Count() > 0;
            }).ToList();


            return members;
        }
        public static List<PropertyInfo> getPropertiesAnnotatedWith(Type t, Type annotatedType)
        {

            var members = getProperties(t).Where(m =>
            {

                var alist = m.GetCustomAttributes(false).ToList();
                var filtered = alist.Where(a => a.GetType() == annotatedType).ToList();
                return filtered.Count() > 0;
            }).ToList();


            return members;
        }
        public static List<Parameter> getParameters(List<ParameterInfo> parameters)
        {
            return parameters.Select(p =>
            {
                IKey key = KeyProducer.getParameterKey(p);
                return new Parameter(p, key);
            }).ToList();
        }
        public static List<FieldInfo> getFields(Type t) 
        {
            return t.GetFields(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).ToList();
        }

        public static List<PropertyInfo> getProperties(Type t)
        {
            return t.GetProperties(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).ToList();
        }

        public static List<Field> getFieldsToInject(object o)
        {

           
            var members = getFieldsAnnotatedWith(o.GetType(),typeof(Inject)).Select(p=>new Field(p,KeyProducer.produceKey(p))).ToList();
            return members;
        }

        public static List<Property> getPropertiesToInject(object o)
        {
            var members = getPropertiesAnnotatedWith(o.GetType(), typeof(Inject)).Select(f => new Property(f, KeyProducer.produceKey(f))).ToList();
            return members;
        }
        public static List<FieldInfo> getNewInstances(object o)
        {
            var members = getFieldsAnnotatedWith(o.GetType(), typeof(NewInstance));
            return members;
        }
        public static List<Type> GetObjectsToCreate(object obj) { 
            //var fields =  obj.GetType().GetFields(BindingFlags.p).wh
            return null;
        }
        public static T getAttributeInfo<T>(FieldInfo o)
        {
            return (T)o.GetCustomAttributes(false).ToList().Find(a => a.GetType() == typeof(T));
        }
        public static T getAttributeInfo<T>(ConstructorInfo o)
        {
            return (T)o.GetCustomAttributes(false).ToList().Find(a => a.GetType() == typeof(T));
        }
        public static T getAttributeInfo<T>(ParameterInfo o)
        {
            return (T)o.GetCustomAttributes(false).ToList().Find(a => a.GetType() == typeof(T));
        }
        public static T getAttributeInfo<T>(object o)
        {
            return (T)o.GetType().GetCustomAttributes(false).ToList().Find(a => a.GetType() == typeof(T));
        }
        public static List<T> getAttributeInfos<T>(object o)
        {
            return o.GetType().GetCustomAttributes(false).ToList().Where(a => a.GetType() == typeof(T)).Select(a=>(T)a).ToList();
        }
        public static T getAttributeInfo<T>(MethodInfo m)
        {
            return (T)m.GetCustomAttributes(false).ToList().Find(a => a.GetType() == typeof(T));
        }
        public static Boolean isAnnotatedWith<T>(object o)
        {
            var alist = getAttributeInfos<T>(o);
            return alist.Count() > 0;
            

        }
        public static List<MethodInfo> getOnInjectedMethods(object o)
        {

            var methods = o.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Where(m =>
            {
                var alist = m.GetCustomAttributes(false).ToList();
                var filtered = alist.Where(a => a.GetType() == typeof(OnInjected)).ToList();
                return filtered.Count() > 0;
            }).ToList();

            return methods;
        }
        
        
        public static List<MethodInfoObject> extractProducers(object obj)
        {

            var methods = obj.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Where(m =>
            {
                var alist = m.GetCustomAttributes(false).ToList();
                var filtered = alist.Where(a => a.GetType() == typeof(Produce)).ToList();
                return filtered.Count() > 0;
            }).Select(methodInfo => new MethodInfoObject(obj, methodInfo,KeyProducer.getProduceKey(methodInfo))).ToList();

            return methods;
        }
        public static List<Object> extractModules(object obj)
        {

            var methods = obj.GetType().GetMethods(BindingFlags.NonPublic | BindingFlags.Public | BindingFlags.Instance).Where(m =>
            {
                var alist = m.GetCustomAttributes(false).ToList();
                var filtered = alist.Where(a => a.GetType() == typeof(Module)).ToList();
                return filtered.Count() > 0;
            });
            //var a = new object[0];
            return methods.Select(x => x.Invoke(obj,new object[0])).ToList();
            
        }
    }

}