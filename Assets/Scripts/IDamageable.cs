using UnityEngine;

public interface IDamageable
{
    // Method to apply damage
    public void TakeDamage(int damage, float hitStunDuration, float knockbackForce, float knockbackForceUp, Vector3 hitPosition);
}