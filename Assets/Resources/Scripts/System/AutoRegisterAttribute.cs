using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = true, Inherited = false)]
public class AutoRegisterAttribute : Attribute
{
    public Type ServiceType { get; }
    public int Priority { get; }

    public AutoRegisterAttribute(Type serviceType, int priority = 0)
    {
        ServiceType = serviceType;
        Priority = priority;
    }
}
