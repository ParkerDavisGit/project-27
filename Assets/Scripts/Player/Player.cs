using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UIElements;

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
        ATTACK = 1,
          DASH = 2,
    }

    private Dictionary<KeyType, KeyState> keys;
    //public Camera camera;

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
    private Animator animator;

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

    private Vector2 readMovement;
    private bool alive = true;

    public float dashForce = 20f;
    public float dashCooldownMax = 1.0f;
    public float dashTimeMax = 0.5f;
    private float dashCooldownCurrent = 0f;
    private float dashTimeCurrent = 0f;
    private bool dashing = false;
    private bool dashUsed = false;

    public GameObject bulletPrefab;

    public AudioSource shootSound;
    public AudioSource hurtSound;
    public AudioSource pickupSound;

    void Awake()
    {
        GameManager.Instantiate();
        feet = GetComponentInChildren<Feet>();
        action = new PlayerAction();
        rb = GetComponent<Rigidbody2D>();
        onGround = false;
        timeInAir = 0f;
        gravityScale = 1.0f;
        jumped = false;
        animator = GetComponent<Animator>();
        buttonPresses = new float[5];
        buttonPresses[0] = 0.0f;   // JUMP
        buttonPresses[1] = 0.0f;
        buttonPresses[2] = 0.0f;
        buttonPresses[3] = 0.0f;
        buttonPresses[4] = 0.0f;

        keys = new Dictionary<KeyType, KeyState>();

        keys.Add(KeyType.JUMP, new KeyState());
        keys.Add(KeyType.ATTACK, new KeyState());
        keys.Add(KeyType.DASH, new KeyState());

        GameManager.ToggleInventory();
    }

    private void Start()
    {
        GameManager.UpdatePlayerHealth(health);
    }

    // Update is called once per frame
    void Update()
    {
        if (alive)
        {
            ReadMovement();
            return;
        }
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
        // DASH
        if (action.Movement.Dash.WasPressedThisFrame())
        {
            keys[KeyType.DASH].pressed = true;
            keys[KeyType.DASH].justPressed = true;
        }
        else if (action.Movement.Dash.WasReleasedThisFrame())
        {
            keys[KeyType.DASH].pressed = false;
            keys[KeyType.DASH].justReleased = true;
        }

        // INVENTORY
        //if (action.Movement.Inventory.WasPressedThisFrame())
        //{
        //    inventory.ToggleInventory();
        //}
    }

    private void Move()
    {
        onGround = feet.OnGround();

        if (keys[KeyType.ATTACK].justPressed)
        {
            //RaycastHit2D results = Physics2D.Raycast(transform.position, new Vector2(1f, 0f), 1f, LayerMask.GetMask("Enemies"));

            //if (results)
            //{
            //    
            //}

            var mousePos = Input.mousePosition;
            mousePos.z += 10;
            shootSound.Play();
            Vector2 mouseDirection = (Camera.main.ScreenToWorldPoint(mousePos) - transform.position);
            GameObject newBullet = Instantiate(bulletPrefab, transform.position, Quaternion.identity);
            newBullet.GetComponent<Bullet>().WithDirection(mouseDirection);
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
            animator.SetBool("OnJump", false);
            
            if (!keys[KeyType.JUMP].pressed)
            {
                jumped = false;
            }
            timeInAir = 0;

            if (!dashing)
            {
                dashUsed = false;
            }
        }
        else
        {
            timeInAir += Time.fixedDeltaTime;
        }

        if (!dashing)
        {
            rb.linearVelocity = new Vector2(Input.GetAxis("Horizontal") * speed * Time.fixedDeltaTime, rb.linearVelocity.y - gravity * Time.fixedDeltaTime * gravityScale);
        }

        else
        {
            dashTimeCurrent += Time.fixedDeltaTime;
            if (dashTimeCurrent > dashTimeMax)
            {
                dashing = false;
                animator.SetBool("OnDash", false);
            }
        }

        readMovement = action.Movement.Move.ReadValue<Vector2>();
        if (readMovement.x == 0)
        {
            animator.SetBool("OnWalk", false);
        }
        else if (readMovement.x > 0)
        {
            animator.SetBool("OnWalk", true);
            gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
        }
        else
        {
            animator.SetBool("OnWalk", true);
            gameObject.transform.localScale = new Vector3(-1f, 1f, 1f);
        }


        if (Mathf.Abs(rb.linearVelocityY) > 1f)
        {
            animator.SetBool("OnJump", true);
            if (rb.linearVelocityY > 0f)
            {
                animator.SetBool("OnFall", false);
            }
            else
            {
                animator.SetBool("OnFall", true);
            }
        }

        // JUMP
        if (keys[KeyType.JUMP].pressed && ((jumpBuffer < .2f && onGround) || (timeInAir < .1f && !jumped)))
        {
            rb.linearVelocityY = Time.fixedDeltaTime * jumpForce;
            jumped = true;
        }

        // DASH
        if (keys[KeyType.DASH].justPressed && dashCooldownCurrent > dashCooldownMax && !dashUsed)
        {
            rb.linearVelocity = new Vector2(dashForce * transform.localScale.x, 0);
            animator.SetBool("OnDash", true);
            dashing = true;
            dashTimeCurrent = 0f;
            dashCooldownCurrent = 0;
            dashUsed = true;
        }


        dashCooldownCurrent += Time.fixedDeltaTime;


        keys[KeyType.JUMP].justPressed = false;
        keys[KeyType.JUMP].justReleased = false;
        keys[KeyType.ATTACK].justPressed = false;
        keys[KeyType.ATTACK].justReleased = false;
        keys[KeyType.DASH].justPressed = false;
        keys[KeyType.DASH].justReleased = false;
    }

    public void GetAttacked(int damage)
    {
        // If you are not healing, and dashing, dodge damage.
        if (damage >= 0)
        {
            // Dashing "I-Frames"
            if (dashing)
            {
                return;
            }
        }

        if (damage < 0)
        {
            hurtSound.Play();
        }
        
        health -= damage;
        
        if (health < 0)
        {
            health = 0;
        }
        if (health > maxHealth)
        {
            health = maxHealth;
        }

        GameManager.UpdatePlayerHealth(health);

        if (health <= 0)
        {
            Die();
        }
    }

    public IEnumerator Die()
    {
        animator.SetBool("OnWalk", false);
        animator.SetTrigger("OnDie");
        alive = false;
        rb.bodyType = RigidbodyType2D.Static;
        yield return new WaitForSeconds(1.2f);
        GameManager.LoadScene("MainMenu");
    }

    public void PickupItem(string obj)
    {
        pickupSound.Play();
        if (obj.Equals("Health"))
        {
            GetAttacked(-3);
            return;
        }
        InventoryManager.Add(obj);
    }
}
