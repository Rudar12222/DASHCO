using UnityEngine;

public class Combat : MonoBehaviour
{
    [Header("Attack Settings")]
    public int damage = 1;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        // Try to get PlayerHealth component
        PlayerHealth health = collision.gameObject.GetComponent<PlayerHealth>();

        // If the object has PlayerHealth, deal damage
        if (health != null)
        {
            health.ChangeHealth(-damage);
        }
    }
}
