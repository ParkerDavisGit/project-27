using System.Collections;
using System.Security.Cryptography;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.EventSystems;
using static System.Math;

public enum Titan_State
{
    Patrol, // Moving between waypoints
    Chase, // Pursuing the player
    Attack, // Attacking the player
    Idle, // Stationary state
    Dying
}

public class Titan : MonoBehaviour
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

    public Titan_State currentState = Titan_State.Idle;
    public Player player;

    private Rigidbody2D rb;
    private Animator animator;

    private bool flashing = false;
    public float flashTimerMax = .25f;
    private float flashTimer = 0f;

    public GameObject bulletPrefab;
    private bool attacking = false;

    public AudioSource attackSound;
    public AudioSource appearSound;

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
            case Titan_State.Patrol:
                PerformPatrol();
                break;
            case Titan_State.Chase:
                PerformChase();
                break;
            case Titan_State.Attack:
                //PerformAttack();
                break;
            //case EnemyState.Idle:
            //    PerformIdle();
            //    break;
        }
    }

    private void PerformPatrol()
    {
        Vector2 targetPosition = patrolPoints[currentWaypointIndex].position;

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        // Waypoint reached, move to next
        if (Vector3.Distance(transform.position, targetPosition) < 0.1f)
        {
            Debug.Log("Got here");
            //currentState = Titan_State.Chase;
            currentWaypointIndex += 1;
            attacking = !attacking;
            speed = 1;

            if (currentWaypointIndex == 1)
            {
                appearSound.Play();
                GameManager.PlayTrack(1);
            }
            else if (currentWaypointIndex == 2)
            {
                GameManager.PlayTrack(0);
            }
            else if (currentWaypointIndex > 2)
            {
                currentState = Titan_State.Idle;
            }
        }

        if (!attacking)
        {
            return;
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


    public void Activate()
    {
        currentState = Titan_State.Patrol;
    }



    void PerformChase()
    {
        Vector2 targetPosition = new Vector2(player.transform.position.x, transform.position.y);

        transform.position = Vector2.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

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
        Vector2 attackDirection = (player.transform.position - transform.position) + new Vector3(0, .2f, 0);
        GameObject newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
        newBullet.GetComponent<DroneBullet>().WithDirection(attackDirection);
    }



    public void Whack()
    {
        if (currentWaypointIndex != 1)
        {
            return;
        }

        if (flashing)
        {
            return;
        }

        print("Ouch!");
        health -= 1;
        if ((health <= 0))
        {
            StartCoroutine(KillSelf());
        }
        //float distanceToPlayer = Vector2.Distance(transform.position, player.transform.position);
        GetComponent<SpriteRenderer>().color = Color.red;
        flashing = true;
    }

    IEnumerator KillSelf()
    {
        GameManager.PlayTrack(0);
        rb.gravityScale = 1;
        animator.SetTrigger("OnDie");
        currentState = Titan_State.Dying;
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
