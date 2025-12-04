using UnityEngine;

public class PlayerKnockback : MonoBehaviour
{
    public float knockbackDuration = 0.18f;

    private Rigidbody2D rb;
    private bool isKnocked = false;
    private Vector2 storedVelocity;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        if (rb) rb.freezeRotation = true;
    }

    public void ApplyKnockback(Vector2 direction, float force)
    {
        if (!rb || isKnocked) return;

        isKnocked = true;
        storedVelocity = direction.normalized * force;

        rb.linearVelocity = Vector2.zero;
        rb.AddForce(storedVelocity, ForceMode2D.Impulse);
        Invoke(nameof(Stop), knockbackDuration);
    }

    private void Stop()
    {
        isKnocked = false;
        if (rb) rb.linearVelocity = Vector2.zero;
    }

    public bool IsBeingKnocked() => isKnocked;
}
