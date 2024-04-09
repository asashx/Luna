using UnityEngine;
//单例
//需要被继承 xxx : Singleton<xxx>
//获取单例 xxx.Instance
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
    //private void Awake()
    //{
    //    if (global)
    //    {
    //        DontDestroyOnLoad(gameObject);
    //    }
    //}
}
