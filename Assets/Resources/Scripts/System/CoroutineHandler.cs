using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoroutineHandler : MonoBehaviour, ICoroutineService
{
    private static CoroutineHandler _instance;
    public static CoroutineHandler Instance { get { Init(); return _instance; } }
    private static bool _isApplicationQuitting = false;

    private void OnApplicationQuit()
    {
        _isApplicationQuitting = true;
    }

    private static void Init()
    {
        if (_isApplicationQuitting)
        {
            return;
        }

        if (_instance == null)
        {
            GameObject manager = GameObject.Find("@CoroutineHandler");

            if (manager == null)
            {
                manager = new GameObject { name = "@CoroutineHandler" };
                manager.AddComponent<CoroutineHandler>();
            }

            _instance = manager.GetComponent<CoroutineHandler>();
            DontDestroyOnLoad(manager);
        }
    }

    public Coroutine RunCoroutine(IEnumerator coroutine)
    {
        return StartCoroutine(coroutine);
    }

    public void TerminateCoroutine(Coroutine coroutine)
    {
        if (coroutine == null)
        {
            return;
        }

        StopCoroutine(coroutine);
    }

    private void Start()
    {
        Init();
    }
}
