/*****************************************************
/* Created by Wizcas Chen (http://wizcas.me)
/* Please contact me if you have any question
/* E-mail: chen@wizcas.me
/* 2017 © All copyrights reserved by Wizcas Zhuo Chen
*****************************************************/

using System;
using System.Collections.Generic;
using System.Linq;

public static class TypeUtility
{
    #region Type Operations
    public static object DefaultValue(this Type type)
    {
        if (type.IsValueType)
            return Activator.CreateInstance(type);
        return null;
    }
    #endregion

    #region Find Common Type
    public static Type FindCommonTypeWithin(params Type[] types)
    {
        if (types == null || types.Length == 0) return null;
        Type typeLeft = types[0];
        for(int i = 0; i < types.Length - 1; i++)
        {
            var typeRight = types[i + 1];
            typeLeft = typeLeft.FindCommonTypeWith(typeRight);
            if (typeLeft == null)
                return null;
        }
        return typeLeft;
    }

    // provide common base class or implemented interface
    public static Type FindCommonTypeWith(this Type typeLeft, Type typeRight)
    {
        if (typeLeft == null || typeRight == null) return null;

        var commonBaseClass = typeLeft.FindBaseClassWith(typeRight) ?? typeof(object);

        return commonBaseClass.Equals(typeof(object))
                ? typeLeft.FindInterfaceWith(typeRight)
                : commonBaseClass;
    }

    // searching for common base class (either concrete or abstract)
    public static Type FindBaseClassWith(this Type typeLeft, Type typeRight)
    {
        if (typeLeft == null || typeRight == null) return null;

        return typeLeft
                .GetClassHierarchy()
                .Intersect(typeRight.GetClassHierarchy())
                .FirstOrDefault(type => !type.IsInterface);
    }

    // searching for common implemented interface
    // it's possible for one class to implement multiple interfaces, 
    // in this case return first common based interface
    public static Type FindInterfaceWith(this Type typeLeft, Type typeRight)
    {
        if (typeLeft == null || typeRight == null) return null;

        return typeLeft
                .GetInterfaceHierarchy()
                .Intersect(typeRight.GetInterfaceHierarchy())
                .FirstOrDefault();
    }

    // iterate on interface hierarhy
    public static IEnumerable<Type> GetInterfaceHierarchy(this Type type)
    {
        if (type.IsInterface) return new[] { type }.AsEnumerable();

        return type
                .GetInterfaces()
                .OrderByDescending(current => current.GetInterfaces().Count())
                .AsEnumerable();
    }

    // interate on class hierarhy
    public static IEnumerable<Type> GetClassHierarchy(this Type type)
    {
        if (type == null) yield break;

        Type typeInHierarchy = type;

        do
        {
            yield return typeInHierarchy;
            typeInHierarchy = typeInHierarchy.BaseType;
        }
        while (typeInHierarchy != null && !typeInHierarchy.IsInterface);
    }
    #endregion

    #region Collection
    public static void ForEach<T>(this IEnumerable<T> source, Action<T> action)
    {
        foreach (var item in source)
            action(item);
    }
    #endregion
}
