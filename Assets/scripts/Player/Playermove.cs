using UnityEngine;

public class playermovement : MonoBehaviour
{
    [Header("Movement")]
    public float speed = 5f;
    public float acceleration = 15f;

    [Header("References")]
    public Rigidbody2D rb;
    public Animator anim;

    private Vector2 inputVector;
    private Vector2 moveVelocity;
    private bool isMoving;

    private void Awake()
    {
        if (!rb) rb = GetComponent<Rigidbody2D>();
        if (!anim) anim = GetComponentInChildren<Animator>();
        
        if (!rb) Debug.LogError("❗ Rigidbody2D missing on Player!");
        if (!anim) Debug.LogError("❗ Animator missing on PlayerSprite child!");
    }

    private void Update()
    {
        float h = Input.GetAxisRaw("Horizontal");
        float v = Input.GetAxisRaw("Vertical");
        inputVector = new Vector2(h, v).normalized;

        if (anim)
        {
            anim.SetFloat("horizontal", h);
            anim.SetBool("isMoving", inputVector.magnitude > 0);
        }

        // Flip sprite
        if (h > 0.1f) transform.localScale = new Vector3(1, 1, 1);
        else if (h < -0.1f) transform.localScale = new Vector3(-1, 1, 1);

        isMoving = inputVector.magnitude > 0;
    }

    private void FixedUpdate()
    {
        if (!rb) return;

        moveVelocity = inputVector * speed;

        // Smooth movement
        rb.linearVelocity = Vector2.MoveTowards(
            rb.linearVelocity,
            moveVelocity,
            acceleration * Time.fixedDeltaTime
        );
        
        rb.freezeRotation = true;
    }
}
