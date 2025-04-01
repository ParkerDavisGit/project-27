using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    private class KeyState
    {
        public bool pressed;
        public bool justPressed;
        public bool justReleased;

        public KeyState()
        {
            pressed = false;
            justPressed = false;
            justReleased = false;
        }
    }

    enum KeyType
    {
        JUMP = 0
    }

    private Dictionary<KeyType, KeyState> keys;

    // Variables
    public int health;

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

    public LayerMask enemyLayer;

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

        keys = new Dictionary<KeyType, KeyState>();

        keys.Add(KeyType.JUMP, new KeyState());
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
            keys[KeyType.JUMP].pressed = true;
            keys[KeyType.JUMP].justPressed = true;

            test = true;
            gravityScale = .5f;

            Collider2D[] enemiesToAttack = Physics2D.OverlapCircleAll(transform.position, 1.0f, enemyLayer);
            foreach (Collider2D obj in enemiesToAttack)
            {
                obj.GetComponent<Enemy>().Whack();
            }
        }
        else if (action.Movement.Jump.WasReleasedThisFrame())
        {
            keys[KeyType.JUMP].pressed = false;
            keys[KeyType.JUMP].justReleased = true;

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
            if (!keys[KeyType.JUMP].pressed)
            {
                jumped = false;
            }
            timeInAir = 0;
        }
        else
        {
            timeInAir += Time.fixedDeltaTime;
        }

        rb.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * speed * Time.fixedDeltaTime, rb.linearVelocity.y - gravity * Time.fixedDeltaTime * gravityScale);

        if (keys[KeyType.JUMP].pressed && ((jumpBuffer < .2f && onGround) || (timeInAir < .1f && !jumped)))
        {
            rb.linearVelocityY = Time.fixedDeltaTime * jumpForce;
            jumped = true;
        }

        keys[KeyType.JUMP].justPressed = false;
        keys[KeyType.JUMP].justReleased = false;
    }

    public void Damage(int damage)
    {
        health -= damage;
        print(health);
    }
}
