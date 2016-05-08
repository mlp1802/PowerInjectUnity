using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Reflection;
using PowerInject;
public class KeyProducer
{
    public static IKey produceKey(Type t, Produce produce)
    {
        if (produce.Named != null)
        {
            return new NamedTypeKey(produce.Named, t);
        }
        else
        {
            return new TypeKey(t);
        }
    }

    public static IKey produceKey(PropertyInfo info)
    {
        var type = info.PropertyType;

        var inject = ReflectionUtils.getAttributeInfo<Inject>(info);
        var named = ReflectionUtils.getAttributeInfo<Named>(info);
        var typed = ReflectionUtils.getAttributeInfo<Typed>(info);
        if (typed != null)
        {
            type = typed.type;
        }
        if (named != null)
        {
            return new NamedTypeKey(named.name, type);
        }
        else
        {
            return new TypeKey(type);
        }


    }
    public static IKey produceKey(FieldInfo info)
    {
        var type = info.FieldType;

        var inject =  ReflectionUtils.getAttributeInfo<Inject>(info);
        var named = ReflectionUtils.getAttributeInfo<Named>(info);
        var typed  = ReflectionUtils.getAttributeInfo<Typed>(info);
        if (typed != null) 
        {
            type = typed.type;
        }
        if (named != null)
        {
            return new NamedTypeKey(named.name, type);
        }
        else 
        {
            return new TypeKey(type);
        }
        

    }
    public static IKey getInsertKeyForType(Type type, Insert produce) 
    {
        TypeKey typeKey = null;
        if (produce.Named != null)
        {
            typeKey = new NamedTypeKey(produce.Named, type);

        }
        else
        {
            typeKey = new TypeKey(type);
        }
        return typeKey;
    }
    public static IKey getInsertKey(object obj, Insert insert)
    {
        Type type = null;
        if (insert.Typed != null)
        {
            return getInsertKeyForType(insert.Typed, insert);
        }
        else {
            return getInsertKeyForType(obj.GetType(), insert);
        }

        
    }

    
    public static IKey getParameterKey(ParameterInfo info)
    {
        var type = info.ParameterType;
        var named = ReflectionUtils.getAttributeInfo<Named>(info);
        var onlyType = ReflectionUtils.getAttributeInfo<Typed>(info);
        if(onlyType!=null) {
            if(onlyType.type!=null) {
                type = onlyType.type;
            }
        }
        if (named != null) 
        { 
          
            return new NamedTypeKey(named.name,type);
        }else {
            return new TypeKey(type);
        }
        
    }
    public static List<IKey> getInsertKeys(object obj)
    {
        var inserts = ReflectionUtils.getAttributeInfos<Insert>(obj).ToList();
        return inserts.Select(insert => getInsertKey(obj,insert)).ToList();
    }
    public static IKey getProduceKey(MethodInfo info)
    {
        var p = ReflectionUtils.getAttributeInfo<Produce>(info);
        return produceKey(info.ReturnType, p);

    }
}

