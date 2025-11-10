using UnityEngine;

public class playermovement : MonoBehaviour
{
    public float speed = 5;
    public Rigidbody2D rb;
    public Animator anim;

    void FixedUpdate()
    {
        float horizontal = Input.GetAxis("Horizontal");
        float vertical = Input.GetAxis("Vertical");

        // Move player
        rb.linearVelocity = new Vector2(horizontal, vertical) * speed;

        // Send values to Animator
        anim.SetFloat("horizontal", horizontal);
        anim.SetFloat("vertical", vertical);

        // Check if moving or idle
        bool isMoving = horizontal != 0 || vertical != 0;
        anim.SetBool("isMoving", isMoving);
       
        
        // FLIP PLAYER WHEN MOVING LEFT/RIGHT
        if (horizontal > 0)
            transform.localScale = new Vector3(1, 1, 1);  // Face Right
        else if (horizontal < 0)
            transform.localScale = new Vector3(-1, 1, 1); // Face Left
    }
}
