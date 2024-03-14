using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FuukoBehaviour : MonoBehaviour
{
    [Header("References")]
    [SerializeField] private GameObject player_Andy;

    private bool runningToPlayer=false;
    private NavMeshAgent navMeshAgent;

    private void Start()
    {
        navMeshAgent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (runningToPlayer)
        {
            navMeshAgent.SetDestination(player_Andy.transform.position);
        }
    }

    public void PrepareForCarry() {
        gameObject.SetActive(false);
    }
    public void PrepareForRelease()
    {
        gameObject.SetActive(true);
    }

    public void CallFuukoToPlayer() { 
        runningToPlayer=true;
        navMeshAgent.isStopped=false;
    }

    public void CancelCallFuukoToPlayer() { 
        runningToPlayer = false;
        navMeshAgent.isStopped = true;
    }
}
