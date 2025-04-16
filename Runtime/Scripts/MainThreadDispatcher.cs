using System;
using System.Collections.Concurrent;
using UnityEngine;

public class MainThreadDispatcher : MonoBehaviour
{
    private readonly ConcurrentQueue<Action> _actions = new();
    private static MainThreadDispatcher _instance;
    public static MainThreadDispatcher Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<MainThreadDispatcher>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(MainThreadDispatcher).Name);
                    _instance = singletonObject.AddComponent<MainThreadDispatcher>();

                    Debug.LogWarning("MainThreadDispatcher instance was created automatically.");
                }
            }
            return _instance;
        }
    }

    void OnEnable()
    {
        if (_instance != null && _instance != this)
        {
            Debug.LogWarning("Duplicate instance of MainThreadDispatcher detected! Destroying new instance.");
            Destroy(gameObject);
            return;
        }

        _instance = this;
        DontDestroyOnLoad(gameObject);
    }


    public void Enqueue(Action action)
    {
        _actions.Enqueue(action);
    }

    private void Update()
    {
        while (_actions.TryDequeue(out var action))
        {
            action?.Invoke();
        }
    }
}
