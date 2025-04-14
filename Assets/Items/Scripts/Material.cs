using UnityEngine;

public class Material : MonoBehaviour
{
    public string type;

    public void OnTriggerEnter2D(Collider2D obj)
    {
        Player player = obj.GetComponent<Player>();
        if (player != null)
        {
            player.PickupItem(this);
            Destroy(this.gameObject);
        }
    }
}
