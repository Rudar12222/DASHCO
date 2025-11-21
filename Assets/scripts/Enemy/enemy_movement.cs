using UnityEngine;

public class EnemyMovement : MonoBehaviour
{
    [Header("Settings")]
    public float speed = 2f;

    private Rigidbody2D rb;
    private Transform player;
    public Animator anim;

    private bool isChasing = false;

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponentInChildren<Animator>(); // Animator on child sprite
    }

    private void Update()
    {
        if (isChasing && player != null)
        {
            Vector2 direction = (player.position - transform.position).normalized;
            rb.linearVelocity = direction * speed;

            // Flip enemy sprite based on movement
            if (direction.x < 0)
                transform.localScale = new Vector3(-1, 1, 1);
            else if (direction.x > 0)
                transform.localScale = new Vector3(1, 1, 1);

            anim.SetBool("isChasing", true);
        }
        else
        {
            rb.linearVelocity = Vector2.zero;
            anim.SetBool("isChasing", false);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            player = collision.transform;
            isChasing = true;
        }
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            isChasing = false;
            rb.linearVelocity = Vector2.zero;
        }
    }
}
