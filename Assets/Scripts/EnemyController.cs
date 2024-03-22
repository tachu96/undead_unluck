using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using UnityEngine.UI;
using System;
using TMPro;
using System.Collections.Generic;

public class EnemyController : MonoBehaviour
{
    //Animator variables
    private const string ANIM_HITSTUNBOOL = "Hitstun";

    [Header("Values")]
    public float maxHealth = 100f;
    public LayerMask groundLayer;
    public float raycastDistance = 1f;
    public float navMeshReactivateDelay;
    public float detectionRange;
    public Transform enemyCenter;
    public LayerMask layerDetectionMask;
    public float stoppingDistanceNavmesh;

    private bool canSeePlayer;
    private bool canSeeFuuko;

    [Header("Patrolling Values")]
    public List<Transform> patrolPoints;
    public float patrolWaitTime = 2f;
    
    private bool patrolling;
    private int currentPatrolIndex = 0;
    private float patrolTimer = 0f;

    [Header("References")]
    public Image healthBar;

    private Animator animator;
    private float currentHealth;
    private Rigidbody rb;
    private bool isStunned = false;
    private float stuntimer;
    private NavMeshAgent navMeshAgent;

    private Vector3 lastHitPosition;
    private float lastKnockbackForce;
    private float lastKnockbackForceUp;
    private bool isGrounded;

    private Transform currentTargetTransform;
    private bool randomTargetSelectionPerformed;

    private void Start()
    {
        currentHealth = maxHealth;
        healthBar.fillAmount = currentHealth / maxHealth;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
        navMeshAgent = GetComponent<NavMeshAgent>();
        navMeshAgent.stoppingDistance = 0f;

        PatrolToNextPoint();
    }

    private void Update()
    {
        GroundDetection();
        RunStunTimer();
        AwarenessCheck();
        TargetSelection();
        DeterminePatrolling();
    }

    private void GroundDetection()
    {
        RaycastHit hit;
        isGrounded = Physics.Raycast(transform.position, Vector3.down, out hit, raycastDistance, groundLayer);
    }

    private void RunStunTimer()
    {
        if (!isStunned) { return; }
        //run the stun timer
        stuntimer -= Time.deltaTime;
        rb.velocity = Vector3.zero;
        if (stuntimer <= 0)
        {
            isStunned = false;
            animator.SetBool(ANIM_HITSTUNBOOL, false);
            ApplyKnockback(lastHitPosition, lastKnockbackForce, lastKnockbackForceUp);
        }
    }


    public void TakeDamage(int damage, float hitStunDuration, float knockbackForce, float knockbackForceUp, Vector3 hitPosition)
    {
        currentHealth -= damage;
        healthBar.fillAmount = currentHealth / maxHealth;
        //deactivate the navmeshagent so that the rigidbody can take control of the physics
        navMeshAgent.enabled = false;

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
                animator.SetBool(ANIM_HITSTUNBOOL, true);
                stuntimer = hitStunDuration;
            }

            lastHitPosition = hitPosition;
            lastKnockbackForce = knockbackForce;
            lastKnockbackForceUp = knockbackForceUp;
        }
    }


    private void ApplyKnockback(Vector3 hitPosition, float knockbackForce, float knockbackForceUp)
    {
        Vector3 knockbackDirection = transform.position - hitPosition;
        knockbackDirection.y = 0f;

        // Apply knockback force with x and z components
        rb.AddForce(knockbackDirection.normalized * knockbackForce, ForceMode.Impulse);

        // Apply knockback force upwards
        rb.AddForce(Vector3.up * knockbackForceUp, ForceMode.Impulse);

        // Wait for a brief moment before reactivating NavMeshAgent
        StartCoroutine(ReactivateNavMeshAfterDelay(navMeshReactivateDelay));
    }

    private IEnumerator ReactivateNavMeshAfterDelay(float delay)
    {
        yield return new WaitForSeconds(delay);

        // Wait until the enemy is grounded before reactivating the NavMeshAgent
        while (!isGrounded)
        {
            yield return null; // Wait until the next frame
        }

        // Ensure NavMeshAgent is enabled when the enemy is grounded.
        navMeshAgent.enabled = true;
    }

    private void Die()
    {
        Destroy(gameObject);
    }


    private void AwarenessCheck()
    {
        // Perform a raycast to check if the player is visible
        RaycastHit hitPlayer;
        bool hitPlayerSomething = Physics.Raycast(enemyCenter.transform.position, PlayerController.Instance.transform.position - enemyCenter.transform.position, out hitPlayer, detectionRange, layerDetectionMask);

        // Perform a raycast to check if Fuuko is visible
        RaycastHit hitFuuko;
        bool hitFuukoSomething = Physics.Raycast(enemyCenter.transform.position, FuukoBehaviour.Instance.FuukoCenter.position - enemyCenter.transform.position, out hitFuuko, detectionRange, layerDetectionMask);

        // Check if the player is seen
        canSeePlayer = hitPlayerSomething && hitPlayer.collider.CompareTag("Player");

        // Check if Fuuko is seen
        canSeeFuuko = hitFuukoSomething && hitFuuko.collider.CompareTag("Fuuko");
    }

    private void TargetSelection()
    {

        if (canSeeFuuko && canSeePlayer)
        {
            if (!randomTargetSelectionPerformed) {
                // Generate a random number between 0 and 1
                float randomNumber = UnityEngine.Random.value;

                if (randomNumber < 0.5f)
                {
                    currentTargetTransform = PlayerController.Instance.transform;
                    Debug.Log("Random Target selected: Player");
                }
                else
                {
                    currentTargetTransform = FuukoBehaviour.Instance.transform;
                    Debug.Log("Random Target selected: Fuuko");
                }
                navMeshAgent.stoppingDistance = stoppingDistanceNavmesh;
                if (navMeshAgent.enabled)
                {
                    navMeshAgent.SetDestination(currentTargetTransform.position);
                }
                randomTargetSelectionPerformed = true;
            }
        }
        else if (canSeePlayer)
        {
            currentTargetTransform = PlayerController.Instance.transform;
            randomTargetSelectionPerformed = false;
            navMeshAgent.stoppingDistance = stoppingDistanceNavmesh;
            if (navMeshAgent.enabled) { 
                navMeshAgent.SetDestination(currentTargetTransform.position); 
            }
            Debug.Log("Target selected: Player");

        }
        else if (canSeeFuuko)
        {
            currentTargetTransform = FuukoBehaviour.Instance.transform;
            randomTargetSelectionPerformed = false;
            navMeshAgent.stoppingDistance = stoppingDistanceNavmesh;
            if (navMeshAgent.enabled)
            {
                navMeshAgent.SetDestination(currentTargetTransform.position);
            }
            Debug.Log("Target selected: Fuuko");
        }
        else
        {
            // Set currentTargetTransform to null to indicate no target
            currentTargetTransform = null;
            randomTargetSelectionPerformed = false;
            navMeshAgent.stoppingDistance = 0f;
            Debug.Log("No target detected");
        }
    }
    private void DeterminePatrolling()
    {
        patrolling = currentTargetTransform == null;
        if (patrolling)
        {
            if (navMeshAgent.remainingDistance < 0.01f)
            {
                patrolTimer += Time.deltaTime;

                if (patrolTimer >= patrolWaitTime)
                {
                    patrolTimer = 0f;
                    PatrolToNextPoint();
                }
            }
        }
        else {
            patrolTimer = 0f;
        }
    }

    private void PatrolToNextPoint()
    {
        // If there are no patrol points, return
        if (patrolPoints == null || patrolPoints.Count == 0)
        {
            Debug.LogWarning("No patrol points assigned! FOR ENEMY:" +gameObject.name);
            return;
        }

        // Randomly select a new patrol point
        int randomIndex = UnityEngine.Random.Range(0, patrolPoints.Count);
        currentPatrolIndex = randomIndex;

        // Set the destination to the selected patrol point
        navMeshAgent.SetDestination(patrolPoints[currentPatrolIndex].position);
    }
}