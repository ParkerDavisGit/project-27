using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Variables
    public float speed;
    public float gravity;
    public float jumpForce;
    public float floatCoefficient;
    private PlayerAction action;
    private Vector2 moveDirection;
    private Rigidbody2D rb;

    private Feet feet;
    private bool onGround;
    private float timeInAir;
    private int jumpBuffer;

    void Awake()
    {
        feet = GetComponentInChildren<Feet>();
        action = new PlayerAction();
        rb = GetComponent<Rigidbody2D>();
        onGround = false;
        timeInAir = 0f;
    }

    // Update is called once per frame
    void Update()
    {
        ReadMovement();
    }

    private void FixedUpdate()
    {
        Move();
    }

    private void OnEnable()
    {
        action.Enable();
    }

    private void OnDisable()
    {
        action.Disable();
    }

    private void ReadMovement()
    {
        moveDirection = action.Movement.Move.ReadValue<Vector2>().normalized;
    }

    private void Move()
    {
        onGround = feet.OnGround();

        if (Input.GetKey(KeyCode.W))
        {
            jumpBuffer++;
        }
        else
        {
            jumpBuffer = 0;
            if (rb.linearVelocityY > 0)
            {
                rb.linearVelocityY = 0;
            }
        }

        if (onGround) {
            if (timeInAir > 1) {
                print("My Feet!");
            }
            timeInAir = 0;
        }

        else
        {
            timeInAir += Time.fixedDeltaTime;
            jumpBuffer++;
        }


        rb.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * speed * Time.fixedDeltaTime, rb.linearVelocity.y - gravity * Time.fixedDeltaTime * (floatCoefficient * timeInAir));

        if (jumpBuffer < 8 && jumpBuffer > 0 && onGround)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, Time.fixedDeltaTime * jumpForce);
        }
    }
}
