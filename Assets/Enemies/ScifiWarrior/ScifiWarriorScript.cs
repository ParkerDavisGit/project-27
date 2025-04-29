using System.Collections;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static System.Math;

public enum SFW_State
{
    Patrol, // Moving between waypoints
    Chase, // Pursuing the player
    Attack, // Attacking the player
    Idle // Stationary state
}

public class SciFiWarrior : MonoBehaviour
{
    public float speed = 0f;
    public int health = 0;
    public int damage = 0;
    public float attackTimer = 0f;
    public float attackCooldown = 0f;
    public Transform attackPoint;

    public float detectionRadius = 0f;
    public float attackRadius = 0f;

    public Transform[] patrolPoints;
    int currentWaypointIndex = 0;

    public SFW_State currentState = SFW_State.Idle;
    public Player player;

    private Rigidbody2D rb;
    private Animator animator;

    public enemyDrops drops;

    private bool flashing = false;
    public float flashTimerMax = .25f;
    private float flashTimer = 0f;

    private bool flippedRight = false;

    public AudioSource attack;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (flashing)
        {
            flashTimer += Time.deltaTime;
            // Reset
            if (flashTimer > flashTimerMax)
            {
                GetComponent<SpriteRenderer>().color = Color.white;
                flashing = false;
                flashTimer = 0f;
            }
        }

        switch (currentState)
        {
            case SFW_State.Patrol:
                PerformPatrol();
                break;
            case SFW_State.Chase:
                PerformChase();
                break;
            case SFW_State.Attack:
                //PerformAttack();
                break;
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

        if (targetPosition.x - transform.position.x < 0)
        {
            if (flippedRight)
            {
                flippedRight = false;
                transform.localScale = new Vector3(-3, 3, 3);
            }
        }
        else
        {
            if (!flippedRight)
            {
                flippedRight = true;
                transform.localScale = new Vector3(3, 3, 3);
            }
        }

        //Vector2 moveDirection = new Vector2(targetPosition.x - transform.position.x, 0).normalized;

        //rb.linearVelocity = moveDirection * speed * Time.deltaTime + extraVelocity;

        // Waypoint reached, move to next
        if (Abs(transform.position.x - targetPosition.x) < 0.1f)
        {
            currentWaypointIndex = (currentWaypointIndex + 1) % patrolPoints.Length;
            transform.localScale = new Vector3(transform.localScale.x * -1, 3, 3);
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < detectionRadius)
        {
            currentState = SFW_State.Chase;
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

        if (targetPosition.x - transform.position.x < 0)
        {
            if (flippedRight)
            {
                flippedRight = false;
                transform.localScale = new Vector3(-3, 3, 3);
            }
        }
        else
        {
            if (!flippedRight)
            {
                flippedRight = true;
                transform.localScale = new Vector3(3, 3, 3);
            }
        }

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < attackRadius)
        {
            StartCoroutine(PerformAttack());
        }
    }

    IEnumerator PerformAttack()
    {
        animator.SetTrigger("OnAttack");
        currentState = SFW_State.Attack;

        yield return new WaitForSeconds(.8f);
        attack.Play();
        float distanceToPlayer = Vector2.Distance(attackPoint.position, player.transform.position);
        if (distanceToPlayer < 1.1f)
            player.GetAttacked(damage);

        yield return new WaitForSeconds(.3f);
        currentState = SFW_State.Chase;
    }



    public void Whack()
    {
        if (health <= 0)
        {
            return;
        }


        if (currentState == SFW_State.Patrol)
        {
            currentState = SFW_State.Chase;
        }

        if (flashing)
        {
            return;
        }

        print("Ouch!");
        health -= 1;
        if ((health <= 0) && currentState != SFW_State.Idle)
        {
            KillSelf();
        }
        //float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        GetComponent<SpriteRenderer>().color = Color.red;
        flashing = true;
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

        // Attack detection visualization
        Gizmos.color = new Color(255, 165, 0);
        Gizmos.DrawWireSphere(transform.position, attackRadius);

        // Attack Point Area
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(attackPoint.position, 1.1f);
    }
}
