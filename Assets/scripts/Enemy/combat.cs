using UnityEngine;

public class EnemyCombat : MonoBehaviour
{
    [Header("Combat Stats")]
    public int damage = 1;
    public float knockbackForce = 6f;
    public float weaponRange = 0.5f;

    [Header("References")]
    public Transform attackPoint;
    public LayerMask playerLayer;

    private Vector2 _knockbackDirection;

    public void SetAttackDirection(Vector2 direction)
    {
        // Stores the direction provided by the EnemyController for knockback
        _knockbackDirection = direction.normalized;
    }

    // This function must be called via an Animation Event on the attack frame.
    public void Attack()
    {
        if (attackPoint == null) return;

        // Detect enemies in range
        Collider2D[] hits = Physics2D.OverlapCircleAll(attackPoint.position, weaponRange, playerLayer);

        foreach (Collider2D hit in hits)
        {
            // Apply Damage
            if (hit.TryGetComponent(out PlayerHealth health))
            {
                health.ChangeHealth(-damage);
            }

            // Apply Knockback
            if (hit.TryGetComponent(out PlayerKnockback kb))
            {
                // Note: PlayerKnockback script is assumed to exist.
                kb.ApplyKnockback(_knockbackDirection, knockbackForce);
            }
        }
    }

    private void OnDrawGizmosSelected()
    {
        if (attackPoint == null) return;
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, weaponRange);
    }
}