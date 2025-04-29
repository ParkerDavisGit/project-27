using UnityEngine;

public class Bullet : MonoBehaviour
{
    private Vector2 direction = new Vector2(0, 0);
    public float speed = 2f;
    public float lifetimeMax = 2f;
    private float lifetime = 0f;

    public void Update()
    {
        lifetime += Time.deltaTime;
        if (lifetime > lifetimeMax)
        {
            Destroy(gameObject);
        }
    }

    public void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Enemy"))
        {
            other.GetComponent<Enemy>().Whack();
        }
        else if (other.CompareTag("SFWarrior"))
        {
            other.GetComponent<SciFiWarrior>().Whack();
        }
        else if (other.CompareTag("ShootyDrone"))
        {
            other.GetComponent<ShootyDrone>().Whack();
        }
        else if (other.CompareTag("Titan"))
        {
            other.GetComponent<Titan>().Whack();
        }

        Destroy(gameObject);
    }

    public void WithDirection(Vector2 _direction)
    {
        direction = _direction.normalized;
        GetComponent<Rigidbody2D>().AddForce(direction * speed * Time.fixedDeltaTime);
    }
}
