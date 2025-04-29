using System.Collections;
using System.Net.NetworkInformation;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static System.Math;

public enum SD_State
{
    Patrol, // Moving between waypoints
    Chase, // Pursuing the player
    Attack, // Attacking the player
    Idle, // Stationary state
    Dying
}

public class ShootyDrone : MonoBehaviour
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

    public SD_State currentState = SD_State.Idle;
    public Player player;

    private Rigidbody2D rb;
    private Animator animator;

    public enemyDrops drops;

    private bool flashing = false;
    public float flashTimerMax = .25f;
    private float flashTimer = 0f;

    private bool flippedRight = false;
    public GameObject bulletPrefab;

    public AudioSource attackSound;


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
            case SD_State.Patrol:
                PerformPatrol();
                break;
            case SD_State.Chase:
                PerformChase();
                break;
            case SD_State.Attack:
                //PerformAttack();
                break;
            case SD_State.Dying:
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
                transform.localScale = new Vector3(-2.5f, 2.5f, 2.5f);
            }
        }
        else
        {
            if (!flippedRight)
            {
                flippedRight = true;
                transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
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
            currentState = SD_State.Chase;
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
                transform.localScale = new Vector3(-2.5f, 2.5f, 2.5f);
            }
        }
        else
        {
            if (!flippedRight)
            {
                flippedRight = true;
                transform.localScale = new Vector3(2.5f, 2.5f, 2.5f);
            }
        }

        if (attackTimer < attackCooldown)
        {
            attackTimer += Time.deltaTime;
            return;
        }
        attackTimer = 0f;

        float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);

        if (distanceToPlayer < attackRadius)
        { 
            PerformAttack();
        }
    }

    void PerformAttack()
    {
        attackSound.Play();
        animator.SetTrigger("OnAttack");
        Vector2 attackDirection = (player.transform.position - transform.position);
        GameObject newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        Debug.Log(newBullet);
        Debug.Log(newBullet.GetComponent<DroneBullet>().speed);
        newBullet.GetComponent<DroneBullet>().WithDirection(attackDirection);
    }



    public void Whack()
    {
        if (health <= 0)
        {
            return;
        }

        if (currentState == SD_State.Patrol)
        {
            currentState = SD_State.Chase;
        }

        if (flashing)
        {
            return;
        }

        print("Ouch!");
        health -= 1;
        if ((health <= 0) && currentState != SD_State.Idle)
        {
            StartCoroutine(KillSelf());
        }
        //float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        GetComponent<SpriteRenderer>().color = Color.red;
        flashing = true;
    }

    public IEnumerator KillSelf()
    {
        rb.gravityScale = 1;
        animator.SetTrigger("OnDie");
        currentState = SD_State.Dying;
        Material drop = Instantiate(drops.common_drop, transform.position, new Quaternion()).GetComponent<Material>();
        drop.asEnemyDrop();
        yield return new WaitForSeconds(1.2f);
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
    }
}
