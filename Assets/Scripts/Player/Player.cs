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
    private float gravityScale;
    private float timeInAir;
    private float currentJumpBuffer;
    private bool jumped;

    public float jumpBuffer;
    public float coyoteTimeBuffer;

    private float[] buttonPresses;

    private bool test;

    void Awake()
    {
        feet = GetComponentInChildren<Feet>();
        action = new PlayerAction();
        rb = GetComponent<Rigidbody2D>();
        onGround = false;
        timeInAir = 0f;
        gravityScale = 1.0f;
        jumped = false;

        buttonPresses = new float[5];
        buttonPresses[0] = 0.0f;   // JUMP
        buttonPresses[1] = 0.0f;
        buttonPresses[2] = 0.0f;
        buttonPresses[3] = 0.0f;
        buttonPresses[4] = 0.0f;

        test = false;
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

        buttonPresses[0] = action.Movement.Jump.ReadValue<float>();
        if (action.Movement.Jump.WasPressedThisFrame())
        {
            test = true;
            gravityScale = .5f;
        }
        else if (action.Movement.Jump.WasReleasedThisFrame())
        {
            gravityScale = 1.0f;
        }
    }

    private void Move()
    {
        onGround = feet.OnGround();

        if (buttonPresses[0] > .1f)
        {
            jumpBuffer += Time.deltaTime;
        }
        else
        {
            jumpBuffer = 0;
        }

        if (onGround)
        {
            jumped = false;
            timeInAir = 0;
        }
        else
        {
            timeInAir += Time.fixedDeltaTime;
        }

        rb.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * speed * Time.fixedDeltaTime, rb.linearVelocity.y - gravity * Time.fixedDeltaTime * gravityScale);

        if (test && ((jumpBuffer < .2f && onGround) || (timeInAir < .1f && !jumped)))
        {
            rb.linearVelocityY = Time.fixedDeltaTime * jumpForce;
            jumped = true;
        }

        test = false;
    }
}
