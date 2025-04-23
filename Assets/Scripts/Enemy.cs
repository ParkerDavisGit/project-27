using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static System.Math;

public enum EnemyState
{
    Patrol, // Moving between waypoints
    Chase, // Pursuing the player
    Attack, // Attacking the player
    Idle // Stationary state
}

public class Enemy : MonoBehaviour
{
    public float speed = 0f;
    public int health = 0;
    public int damage = 0;
    public float attackTimer = 0f;
    public float attackCooldown = 0f;

    public float detectionRadius = 0f;
    public float attackRadius = 0f;

    public Transform[] patrolPoints;
    int currentWaypointIndex = 0;

    public EnemyState currentState = EnemyState.Idle;
    public Player player;

    private Rigidbody2D rb;

    public enemyDrops drops;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        switch (currentState)
        {
            case EnemyState.Patrol:
                PerformPatrol();
                break;
            case EnemyState.Chase:
                PerformChase();
                break;
            //case EnemyState.Attack:
            //    PerformAttack();
            //    break;
            //case EnemyState.Retreat:
            //    PerformRetreat();
            //    break;
            //case EnemyState.Idle:
            //    PerformIdle();
            //    break;
        }

        //if (rb.linearVelocityX > 0f)
        //{
        //    rb.linearVelocityX = rb.linearVelocityX * (1-Time.deltaTime);
        //}
        //else if (rb.linearVelocityX < 0f)
        //{
        //    rb.linearVelocityX = rb.linearVelocityX * (1-Time.deltaTime);
        //}
    }

    private void PerformPatrol()
    {
        Vector2 targetPosition = new Vector2(patrolPoints[currentWaypointIndex].position.x, transform.position.y);

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        //Vector2 moveDirection = new Vector2(targetPosition.x - transform.position.x, 0).normalized;

        //rb.linearVelocity = moveDirection * speed * Time.deltaTime + extraVelocity;

        // Waypoint reached, move to next
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolPoints.Length;
            transform.localScale = new Vector3(transform.localScale.x * -1, 2, 2);
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < detectionRadius)
        {
            currentState = EnemyState.Chase;
        }
    }

    void PerformChase()
    {
        Vector2 targetPosition = new Vector2(player.transform.position.x, transform.position.y);

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        attackTimer += Time.deltaTime;

        if (attackTimer > attackCooldown)
        {
            float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

            if (distanceToPlayer < attackRadius)
            {
                player.GetAttacked(damage);
                attackTimer = 0f;
            }
        }
    }

    public void Whack()
    {
        print("Ouch!");
        health -= 1;
        if ((health <= 0) && currentState != EnemyState.Idle)
        {
            KillSelf();
        }
        //float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        rb.AddForce(new Vector2(transform.position.x - player.transform.position.x, .5f) * 200);
    }

    public void KillSelf()
    {
        Material drop = Instantiate(drops.common_drop, transform.position, new Quaternion()).GetComponent<Material>();

        drop.asEnemyDrop();

        Destroy(gameObject);
    }

    // Visualize detection and attack ranges in scene view
    void OnDrawGizmosSelected()
    {
        //Detection radius visualization
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);

        // Attack radius visualization
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, attackRadius);
    }
}
