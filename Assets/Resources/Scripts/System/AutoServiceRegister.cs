using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;


public static class AutoServiceRegister
{
    private static HashSet<Type> _registeredTypes = new HashSet<Type>();

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.BeforeSceneLoad)]
    private static void PrepareRegister()
    {
        ServiceLocator.Clear();

        var assemblies = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == "Assembly-CSharp")
            ?? Assembly.GetExecutingAssembly();

        List<(Type type, AutoRegisterAttribute attr)> list = new List<(Type type, AutoRegisterAttribute attr)>();
        foreach (var type in assemblies.GetTypes())
        {
            if (type.IsAbstract || type.IsInterface)
            {
                continue;
            }

            var attributes = type.GetCustomAttributes(typeof(AutoRegisterAttribute), true).Cast<AutoRegisterAttribute>();
            foreach (var attribute in attributes)
            {
                list.Add((type, attribute));
            }
        }

        foreach (var (type, attr) in list.OrderBy(c => c.attr.Priority))
        {
            EnsureRegistered(attr.ServiceType, type);
        }

        ServiceLocator.SetInitialized();
    }

    private static void EnsureRegistered(Type serviceType, Type implementationType)
    {
        if (ServiceLocator.GetService(serviceType) != null)
        {
            return;
        }

        var instance = Construct(implementationType);
        if (instance == null)
        {
            return;
        }

        ServiceLocator.RegisterService(serviceType, instance);
    }

    private static object Construct(Type implementationType)
    {
        if (_registeredTypes.Contains(implementationType))
        {
            return null;
        }

        _registeredTypes.Add(implementationType);

        var constructors = implementationType.GetConstructors().OrderByDescending(c => c.GetParameters().Length);

        foreach (var constructor in constructors)
        {
            var parms = constructor.GetParameters();
            var args = new object[parms.Length];
            bool canConstruct = true;

            for (int i = 0; i < parms.Length; i++)
            {
                var dependencyType = parms[i].ParameterType;
                var dependency = ServiceLocator.GetService(dependencyType);
                if (dependency == null)
                {
                    var dependencyImplementation = FindImplementationForService(dependencyType);
                    if (dependencyImplementation == null)
                    {
                        canConstruct = false;
                        break;
                    }

                    dependency = Construct(dependencyImplementation);
                    if (dependency == null)
                    {
                        canConstruct = false;
                        break;
                    }

                    ServiceLocator.RegisterService(dependencyType, dependency);
                }

                args[i] = dependency;
            }

            if (canConstruct)
            {
                return Activator.CreateInstance(implementationType, args);
            }
        }

        if (implementationType.GetConstructor(Type.EmptyTypes) != null)
            return Activator.CreateInstance(implementationType);

        return null;
    }

    private static Type FindImplementationForService(Type serviceType)
    {
        var assemblies = AppDomain.CurrentDomain.GetAssemblies().FirstOrDefault(assembly => assembly.GetName().Name == "Assembly-CSharp")
            ?? Assembly.GetExecutingAssembly();

        foreach (var type in assemblies.GetTypes())
        {
            if (type.IsAbstract || type.IsInterface)
            {
                continue;
            }

            var attributes = type.GetCustomAttributes(typeof(AutoRegisterAttribute), true).Cast<AutoRegisterAttribute>();
            if (attributes.Any(attr => attr.ServiceType == serviceType))
            {
                return type;
            }
        }

        return null;
    }
}
