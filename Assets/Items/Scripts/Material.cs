using UnityEngine;

public class Material : MonoBehaviour
{
    public string type;
    private Rigidbody2D rb;

    public void asEnemyDrop()
    {
        Vector2 newForce = new Vector2(Random.Range(-10, 10), Random.Range(3, 10));
        GetComponent<Rigidbody2D>().AddForce(newForce);
    }

    public void OnTriggerEnter2D(Collider2D obj)
    {
        Player player = obj.GetComponent<Player>();
        if (player != null)
        {
            player.PickupItem(this.type);
            Destroy(this.gameObject);
        }
    }

    private void Start()
    {
        rb = GetComponent<Rigidbody2D>();
    }
}
