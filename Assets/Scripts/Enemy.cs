using System.Security.Cryptography;
using UnityEngine;
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
    public float damage = 0f;

    public float detectionRadius = 0f;
    public float attackRadius = 0f;

    public Transform[] patrolPoints;
    int currentWaypointIndex = 0;

    public EnemyState currentState = EnemyState.Idle;
    public Player player;

    private Rigidbody2D rb;

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
    }

    private void PerformPatrol()
    {
        Vector2 targetPosition = new Vector2(patrolPoints[currentWaypointIndex].position.x, transform.position.y);

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        // Waypoint reached, move to next
        if (Vector2.Distance(transform.position, targetPosition) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolPoints.Length;
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

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer > detectionRadius)
        {
            currentState = EnemyState.Patrol;
        }
    }

    public void Whack()
    {
        print("Ouch!");
        health -= 1;
        if ((health <= 0) && currentState != EnemyState.Idle)
        {
            Destroy(gameObject);
        }
        //float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        rb.AddForce(new Vector2(transform.position.x - player.transform.position.x, .5f) * 200);
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
