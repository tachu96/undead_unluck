using UnityEngine;

public class EnemAttackHitbox: MonoBehaviour
{
    [Header("References")]
    public Transform enemy_visual;

    [Header("Values")]
    public bool usesEnemyPosition;
    public int damage = 10;
    public float hitStunDuration = 0.2f;
    public float knockbackForce;
    public float knockbackForceUp;

    private bool hitSoundPlayed;
    private AudioSource audioSource;

    private Vector3 sourceHitPosition;

    private void Start()
    {
        audioSource = GetComponent<AudioSource>();
    }

    private void OnTriggerEnter(Collider other)
    {
        IDamageable damageable = other.GetComponent<IDamageable>();

        if (damageable != null)
        {
            if (!hitSoundPlayed)
            {
                audioSource.Play();
                hitSoundPlayed = true;
            }

            sourceHitPosition = transform.position;
            if (usesEnemyPosition)
            {
                sourceHitPosition = enemy_visual.position;
            }
            damageable.TakeDamage(damage, hitStunDuration, knockbackForce, knockbackForceUp, sourceHitPosition);
        }
    }

    private void OnDisable()
    {
        //reset hit sound
        hitSoundPlayed = false;
    }
}