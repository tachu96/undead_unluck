using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.UI;

public class FuukoBehaviour : MonoBehaviour, IDamageable
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

    //Animator variables
    private const string ANIM_HIT_TRIGGER = "Hit";

    [Header("References")]
    [SerializeField] private GameObject player_Andy;
    [SerializeField] private Image healthBar;
    [SerializeField] Animator animator;
    public Transform FuukoCenter;

    private AudioSource audioSource;

    [Header("AudioReferences")]
    [SerializeField] private AudioClip FuukoHit;

    [Header("Values")]
    [SerializeField] private float maxHealth;
    public LayerMask groundLayer;
    public float raycastDistance = 1f;
    public float navMeshReactivateDelay;

    private bool runningToPlayer=false;
    private float currentHealth;
    private NavMeshAgent navMeshAgent;
    private bool isStunned;
    private float stuntimer;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.fillAmount = currentHealth / maxHealth;
        navMeshAgent = GetComponent<NavMeshAgent>();
        audioSource = GetComponent<AudioSource>();
    }

    private void Update()
    {
        RunStunTimer();
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

            runningToPlayer = true;
            navMeshAgent.isStopped = false;
    }

    public void CancelCallFuukoToPlayer() { 

            runningToPlayer = false;
            navMeshAgent.isStopped = true;
    }

    private void RunStunTimer()
    {
        if (!isStunned) { return; }
        //run the stun timer
        stuntimer -= Time.deltaTime;
        if (stuntimer <= 0)
        {
            isStunned = false;
            animator.SetBool(ANIM_HIT_TRIGGER, false);
        }
    }

    public void TakeDamage(int damage, float hitStunDuration, float knockbackForce, float knockbackForceUp, Vector3 hitPosition)
    {
        currentHealth -= damage;
        healthBar.fillAmount = currentHealth / maxHealth;
        //stop navmesh
        CancelCallFuukoToPlayer();

        if (currentHealth <= 0)
        {
            healthBar.fillAmount = 0;
            Die();
        }
        else
        {
            if (hitStunDuration > 0f)
            {
                isStunned = true;
                animator.SetTrigger(ANIM_HIT_TRIGGER);
                stuntimer = hitStunDuration;
                audioSource.PlayOneShot(FuukoHit);
            }

        }
    }

    private void Die() {
        Debug.Log("FUUKO DIED");
    }
}
