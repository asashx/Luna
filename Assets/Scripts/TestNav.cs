using NavMeshPlus.Components;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.PlayerLoop;

public class TestNav : MonoBehaviour
{
    //[SerializeField]Transform target;
    NavMeshAgent navMeshAgent;
    public NavMeshSurface navMeshSurface;
    private void Awake()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.updateRotation = navMeshAgent.updateUpAxis = false;
        //StartCoroutine(RefreshSurface());
        
    }
    private void Update()
    {
        //SetIdle();
    }
    IEnumerator RefreshSurface()
    {
        while(true)
        {
            yield return new WaitForSeconds(0.500f);
            navMeshSurface.BuildNavMesh();
        }
    }
    public void SetIdle()
    {
        navMeshAgent.velocity = Vector3.zero;
    }    
    public void SetTargetAndSpeed(Vector3 tar, float s)
    {
        navMeshAgent.SetDestination(tar);
        navMeshAgent.speed = s;
        print(navMeshAgent.velocity);
    }
}
