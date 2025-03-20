using Unity.VisualScripting;
using UnityEngine;

public class Player : MonoBehaviour
{
    // Variables
    public int speed;
    private PlayerAction action;
    private Vector2 moveDirection;
    private Rigidbody2D rb;

    void Awake()
    {
        action = new PlayerAction();
        rb = GetComponent<Rigidbody2D>();
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
        rb.MovePosition(rb.position + moveDirection * speed * Time.fixedDeltaTime);
    }
}
