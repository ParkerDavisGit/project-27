using System.Collections;
using UnityEngine;

public class DroneBullet : MonoBehaviour
{
    private Vector2 direction = new Vector2(0, 0);
    public float speed = 2f;
    public float lifetimeMax = 2f;
    private float lifetime = 0f;
    private bool dying = false;

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
        if (dying)
        {
            return;
        }

        if (other.CompareTag("Player"))
        {
            other.GetComponent<Player>().GetAttacked(1);
        }
        StartCoroutine(Die());
    }

    private IEnumerator Die()
    {
        dying = true;
        GetComponent<Rigidbody2D>().linearVelocity = new Vector2(0f, 0f);
        GetComponent<Animator>().SetTrigger("OnHit");
        yield return new WaitForSeconds(.5f);
        Destroy(gameObject);
    }

    public void WithDirection(Vector2 _direction)
    {
        direction = _direction.normalized;
        //transform.rotation = Quaternion.Euler(direction);
        transform.rotation.Set(45f, 45f, 45f, 45f);
        GetComponent<Rigidbody2D>().AddForce(direction * speed * Time.fixedDeltaTime);
    }
}
