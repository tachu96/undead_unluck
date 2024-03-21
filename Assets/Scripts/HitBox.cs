using UnityEngine;

public class HitBox : MonoBehaviour
{
    [Header("References")]
    public Transform player_visual;

    [Header("Values")]
    public bool usesPlayerPosition;
    public int damage = 10;
    public float hitStunDuration = 0.5f;
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
        EnemyController enemy = other.GetComponent<EnemyController>();

        if (enemy != null)
        {
            if (!hitSoundPlayed) { 
                audioSource.Play();
                hitSoundPlayed = true;
            }

            sourceHitPosition = transform.position;
            if (usesPlayerPosition)
            {
                sourceHitPosition = player_visual.position;
            }
            enemy.TakeDamage(damage, hitStunDuration, knockbackForce, knockbackForceUp, sourceHitPosition);
        }
    }

    private void OnDisable()
    {
        //reset hit sound
        hitSoundPlayed = false;
    }
}