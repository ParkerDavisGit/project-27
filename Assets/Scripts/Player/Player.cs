using System.Collections;
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
        JUMP = 0,
        ATTACK = 1
    }

    private Dictionary<KeyType, KeyState> keys;

    // Variables
    public int health;
    public int maxHealth;

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

    public LayerMask enemyLayer;

    public Inventory inventory;
    public HealthDisplay healthDisplay;

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

        keys = new Dictionary<KeyType, KeyState>();

        keys.Add(KeyType.JUMP, new KeyState());
        keys.Add(KeyType.ATTACK, new KeyState());

        //healthDisplay.SetText(string.Format("Health: {0}/{1}", health, maxHealth));
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

            gravityScale = .5f;
        }
        else if (action.Movement.Jump.WasReleasedThisFrame())
        {
            keys[KeyType.JUMP].pressed = false;
            keys[KeyType.JUMP].justReleased = true;

            gravityScale = 1.0f;
        }
        if (action.Movement.Attack.WasPressedThisFrame())
        {
            keys[KeyType.ATTACK].pressed = true;
            keys[KeyType.ATTACK].justPressed = true;
        }
        else if (action.Movement.Attack.WasReleasedThisFrame())
        {
            keys[KeyType.ATTACK].pressed = false;
            keys[KeyType.ATTACK].justReleased = true;
        }

        if (action.Movement.Inventory.WasPressedThisFrame())
        {
            inventory.ToggleInventory();
        }
    }

    private void Move()
    {
        onGround = feet.OnGround();

        if (keys[KeyType.ATTACK].justPressed)
        {
            RaycastHit2D results = Physics2D.Raycast(transform.position, new Vector2(1f, 0f), 1f, LayerMask.GetMask("Enemies"));
            
            if (results)
            {
                results.transform.gameObject.GetComponent<Enemy>().Whack();
            }
        }

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
        keys[KeyType.ATTACK].justPressed = false;
        keys[KeyType.ATTACK].justReleased = false;
    }

    public void GetAttacked(int damage)
    {
        health -= damage;

        if (health > maxHealth)
        {
            health = maxHealth;
        }

        healthDisplay.SetText(string.Format("Health: {0}/{1}", health, maxHealth));

        if (health <= 0)
        {
            Destroy(gameObject);
        }
    }

    public void PickupItem(string obj)
    {

        if (obj.Equals("Health"))
        {
            GetAttacked(-3);
            return;
        }
        inventory.Add(obj);
    }
}
