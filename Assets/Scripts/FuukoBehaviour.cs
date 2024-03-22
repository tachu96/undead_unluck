using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class FuukoBehaviour : MonoBehaviour
{
    private static FuukoBehaviour instance;

    // Public static property to access the single instance of the class
    public static FuukoBehaviour Instance
    {
        get
        {
            // If the instance hasn't been created yet, create it
            if (instance == null)
            {
                instance = FindObjectOfType<FuukoBehaviour>();

                if (instance == null)
                {
                    Debug.LogWarning("No instance of FuukoBehaviour found in the scene. Creating a new instance.");
                    GameObject obj = new GameObject("Fuuko");
                    instance = obj.AddComponent<FuukoBehaviour>();
                }
            }
            return instance;
        }
    }


    [Header("References")]
    [SerializeField] private GameObject player_Andy;
    public Transform FuukoCenter;


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
