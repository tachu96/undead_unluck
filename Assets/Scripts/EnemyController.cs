using UnityEngine;
using UnityEngine.AI;
using System.Collections;

public class EnemyController : MonoBehaviour
{
    //Animator variables
    private const string ANIM_HITSTUNBOOL = "Hitstun";

    [Header("References")]
    public int maxHealth = 100;

    private Animator animator;
    private int currentHealth;
    private Rigidbody rb;
    private bool isStunned = false;
    private float stuntimer;

    private Vector3 lastHitPosition;
    private float lastKnockbackForce;
    private float lastKnockbackForceUp;

    private void Start()
    {
        currentHealth = maxHealth;
        rb = GetComponent<Rigidbody>();
        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        RunStunTimer();
    }

    private void RunStunTimer()
    {
        if (!isStunned) { return; }
        //run the stun timer
        stuntimer -= Time.deltaTime;
        rb.velocity = Vector3.zero;
        if (stuntimer <= 0) {
            isStunned = false;
            animator.SetBool(ANIM_HITSTUNBOOL, false);
            ApplyKnockback(lastHitPosition, lastKnockbackForce, lastKnockbackForceUp);
        }
    }


    public void TakeDamage(int damage, float hitStunDuration, float knockbackForce, float knockbackForceUp, Vector3 hitPosition)
    {
        currentHealth -= damage;

        
        if (currentHealth <= 0)
        {
            Die();
        }
        else
        {
            if (hitStunDuration>0f)
            {
                isStunned = true;
                animator.SetBool(ANIM_HITSTUNBOOL, true);
                stuntimer = hitStunDuration;
            }

            lastHitPosition= hitPosition;
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
    }

    private void Die()
    {
        Destroy(gameObject);
    }
}