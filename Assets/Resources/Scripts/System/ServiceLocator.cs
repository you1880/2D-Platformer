using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ServiceLocator
{
    public static bool IsInitialized { get; private set; } = false;
    private static Dictionary<Type, object> _services = new Dictionary<Type, object>();

    public static void RegisterService<T>(T service) where T : class => _services[typeof(T)] = service;
    public static void RegisterService(Type type, object service) => _services[type] = service;

    public static T GetService<T>() where T : class
    {
        if (_services.TryGetValue(typeof(T), out object service))
        {
            return service as T;
        }

        Debug.LogError($"{typeof(T).Name} not Registered");
        return default;
    }

    public static object GetService(Type type)
    {
        _services.TryGetValue(type, out object service);

        return service;
    }

    public static void SetInitialized() => IsInitialized = true;

    public static void Clear() => _services.Clear();
}
