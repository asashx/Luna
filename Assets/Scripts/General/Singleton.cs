using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Singleton<T> : MonoBehaviour where T : Singleton<T>
{
    static T instance;
    public bool global = true;
    public static T Instance
    {
        get
        {
            if (instance == null)
                instance = FindObjectOfType<T>();
            return instance;
        }
    }
    private void Awake()
    {
        if (global)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    
}
