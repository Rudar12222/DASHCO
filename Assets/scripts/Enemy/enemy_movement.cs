using UnityEngine;

[RequireComponent(typeof(Rigidbody2D))]
public class EnemyController : MonoBehaviour
{
    public enum EnemyState { Idle, Patrol, Chase, Attack, Return }

    [Header("References")]
    public EnemyCombat combatScript;
    public Animator anim;
    
    [Header("Movement Settings")]
    public float moveSpeed = 2f;
    public float acceleration = 12f;

    [Header("Detection Ranges")]
    public float chaseRange = 5f;
    public float attackRange = 1.2f;
    public float loseRange = 7f;

    [Header("Patrol Settings")]
    public Transform[] patrolPoints;
    public float patrolWaitTime = 3f;
    public float patrolPointTolerance = 0.2f;

    [Header("Combat Settings")]
    public float attackCooldown = 1f;

    [Header("Debug")]
    public bool showRanges = true;

    // Internal State
    private EnemyState state = EnemyState.Idle;
    private Rigidbody2D rb;
    private Transform player;
    private PlayerHealth playerHealth; // ✅ NEW: Reference to player's health
    private Vector2 desiredVelocity;
    private Vector3 spawnPosition;
    
    private float attackTimer;
    private float waitTimer;
    private int currentPatrolIndex;
    private bool isRunning;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
        spawnPosition = transform.position;

        // Auto-find combat script if not assigned
        if (!combatScript) combatScript = GetComponent<EnemyCombat>();
        if (!anim) anim = GetComponentInChildren<Animator>();

        // Find Player safely and get the PlayerHealth component
        GameObject pObj = GameObject.FindGameObjectWithTag("Player");
        if (pObj) 
        {
            player = pObj.transform;
            playerHealth = pObj.GetComponent<PlayerHealth>(); // ✅ Get the health component
        }
    }

    private void Update()
    {
        // ✅ CRITICAL FIX: If player is gone or dead, immediately stop movement and logic.
        if (player == null || (playerHealth != null && playerHealth.IsDead)) 
        {
            state = EnemyState.Idle;
            desiredVelocity = Vector2.zero;
            SetRun(false);
            return;
        }

        // Decrease Attack Cooldown
        if (attackTimer > 0f) attackTimer -= Time.deltaTime;

        // State Machine
        switch (state)
        {
            case EnemyState.Idle:   UpdateIdle();   break;
            case EnemyState.Patrol: UpdatePatrol(); break;
            case EnemyState.Chase:  UpdateChase();  break;
            case EnemyState.Attack: UpdateAttack(); break;
            case EnemyState.Return: UpdateReturn(); break;
        }

        // Animation handling
        if (anim) anim.SetBool("isRunning", isRunning);
    }

    private void FixedUpdate()
    {
        // Smooth Movement Physics
        // NOTE: Use rb.velocity instead of rb.linearVelocity if not on Unity 6
        rb.linearVelocity = Vector2.MoveTowards(
            rb.linearVelocity, 
            desiredVelocity, 
            acceleration * Time.fixedDeltaTime
        );
    }

    #region State Logic

    private void UpdateIdle()
    {
        desiredVelocity = Vector2.zero;
        SetRun(false);

        if (CanSeePlayer())
        {
            state = EnemyState.Chase;
        }
        else if (patrolPoints.Length > 0)
        {
            state = EnemyState.Patrol;
            waitTimer = 0f;
        }
    }

    private void UpdatePatrol()
    {
        if (CanSeePlayer())
        {
            state = EnemyState.Chase;
            return;
        }

        if (patrolPoints.Length == 0)
        {
            state = EnemyState.Idle;
            return;
        }

        Transform target = patrolPoints[currentPatrolIndex];
        float dist = Vector2.Distance(transform.position, target.position);

        if (dist <= patrolPointTolerance)
        {
            desiredVelocity = Vector2.zero;
            SetRun(false);

            waitTimer += Time.deltaTime;
            if (waitTimer >= patrolWaitTime)
            {
                currentPatrolIndex = (currentPatrolIndex + 1) % patrolPoints.Length;
                waitTimer = 0f;
            }
        }
        else
        {
            MoveTowards(target.position);
        }
    }

    private void UpdateChase()
    {
        float dist = DistanceToPlayer();

        if (dist > loseRange || !CanSeePlayer()) // Added check for CanSeePlayer() here
        {
            state = EnemyState.Return;
            return;
        }

        if (dist <= attackRange)
        {
            state = EnemyState.Attack;
            return;
        }

        MoveTowards(player.position);
    }

    private void UpdateAttack()
    {
        desiredVelocity = Vector2.zero;
        SetRun(false);

        float dist = DistanceToPlayer();
        
        // If player moves out of range OR is no longer visible (dead), chase again/return
        if (dist > attackRange * 1.2f || !CanSeePlayer())
        {
            state = EnemyState.Chase;
            return;
        }

        // Always face the player while attacking
        Vector2 dir = (player.position - transform.position).normalized;
        FlipTowards(dir);

        // Update Combat Script with direction
        if(combatScript) combatScript.SetAttackDirection(dir);

        if (attackTimer <= 0f)
        {
            anim.SetTrigger("Right attack"); 
            attackTimer = attackCooldown;
        }
    }

    private void UpdateReturn()
    {
        float dist = Vector2.Distance(transform.position, spawnPosition);

        if (dist <= patrolPointTolerance)
        {
            state = EnemyState.Idle;
            return;
        }

        // Check for player again while returning
        if (CanSeePlayer())
        {
            state = EnemyState.Chase;
            return;
        }

        MoveTowards(spawnPosition);
    }

    #endregion

    #region Helpers

    private void MoveTowards(Vector2 targetPos)
    {
        Vector2 dir = (targetPos - (Vector2)transform.position).normalized;
        desiredVelocity = dir * moveSpeed;
        FlipTowards(dir);
        SetRun(true);
    }

    private void FlipTowards(Vector2 dir)
    {
        // Using Mathf.Sign handles the flip cleanly based on X direction
        if (Mathf.Abs(dir.x) > 0.1f)
        {
            float scaleX = Mathf.Abs(transform.localScale.x) * Mathf.Sign(dir.x);
            transform.localScale = new Vector3(scaleX, transform.localScale.y, transform.localScale.z);
        }
    }

    private void SetRun(bool run) => isRunning = run;

    private float DistanceToPlayer() => player ? Vector2.Distance(transform.position, player.position) : Mathf.Infinity;

    private bool CanSeePlayer() 
    {
        // ✅ CRITICAL FIX: Player must be in range AND not dead to be "seen"
        return player != null && 
               (playerHealth == null || !playerHealth.IsDead) &&
               DistanceToPlayer() <= chaseRange;
    }

    private void OnDrawGizmosSelected()
    {
        if (!showRanges) return;
        
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, chaseRange);
        
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRange);
        
        Gizmos.color = Color.gray;
        Gizmos.DrawWireSphere(transform.position, loseRange);
    }

    #endregion
}