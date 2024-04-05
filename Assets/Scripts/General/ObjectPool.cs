using System.Collections.Generic;
using UnityEngine;
public interface IObjectPool_able
{
    public ObjectPool Pool { get; set; }
    public GameObject MyInstantiate(Vector3 fPos, Quaternion fRot, Transform fParent = null);
    public void MyDestroy(GameObject obj);
}
public class ObjectPool_ability:MonoBehaviour,IObjectPool_able
{
    private GameObject targetManager;
    public ObjectPool_ability(GameObject targetManager)
    { this.targetManager = targetManager; }
    private ObjectPool pool = null;
    public ObjectPool Pool
    {
        get
        {
            if (pool == null)
            {
                if((pool = targetManager.GetComponent<ObjectPool>()) == null)
                    pool = targetManager.AddComponent(typeof(ObjectPool)) as ObjectPool;
                pool.Initialize();
            }
            return pool;
        }
        set
        {
            pool = value;
        }
    }
    public GameObject MyInstantiate(Vector3 fPos, Quaternion fRot, Transform fParent = null)
    {
        return Pool.MyInstantiate(fPos, fRot, fParent);
    }
    public void MyDestroy(GameObject obj)
    {
        Pool.MyDestroy(obj);
    }
}
public class ObjectPool : MonoBehaviour
{       
    /*[SerializeField] */GameObject tPrefab;
    /*[SerializeField] */
    readonly int initCount = 16;
    readonly int poolCount;
    List<GameObject> allObject;
    Stack<GameObject> availableObject;
    public void Initialize()
    {
        allObject = new();
        availableObject = new();
        TryGetPrefab();
        ClearChild(transform);
        MyCreateNew(poolCount);
    }
    void TryGetPrefab()
    {
        if (tPrefab == null)
        {
            tPrefab = transform.GetChild(0).gameObject;
            if (tPrefab == null)
            {
                throw new System.Exception(name + "'s prefab is null !");
            }
        }
    }
    void MyCreateNew(int newCount)
    {
        for (int i = 0; i < newCount; i++)
        {
            GameObject g = Instantiate(tPrefab, Vector3.zero, Quaternion.identity,transform);
            g.SetActive(false);
            allObject.Add(g);
            availableObject.Push(g);
        }
    }
    public GameObject MyInstantiate(Vector3 fPos, Quaternion fRot, Transform fParent = null)
    {
        if (availableObject.Count == 0)
        {
            Debug.LogWarning(tPrefab.name + " available pool is empty!");
            MyCreateNew((int)(poolCount == 0 ? initCount : poolCount * 0.5f));
        }
        GameObject g = availableObject.Pop();
        g.transform.position = fPos;
        g.transform.rotation = fRot;
        if (fParent != null)
            g.transform.parent = fParent;
        g.gameObject.SetActive(true);
        return g;
    }
    public void MyDestroy(GameObject obj)
    {
        if (obj == null)
        {
            Debug.LogError("MyDestroy null " + tPrefab.name + " !");
            return;
        }
        obj.gameObject.SetActive(false);
        availableObject.Push(obj);
    }
    void ClearChild(Transform t)
    {
        for (int i = 0; i < t.childCount; i++)
        {
            UnityEngine.GameObject g = t.GetChild(i).gameObject;
            if (g.activeSelf)
                Destroy(g);
        }
    }
}