using UnityEngine;

public class EventTrigger : MonoBehaviour
{
    public string eventName;
    public bool exhausted = false;
    public void OnTriggerEnter2D(Collider2D collision)
    {
        if (exhausted)
        {
            return;
        }

        if (collision.CompareTag("Player"))
        {
            GameManager.EventTrigger(eventName);
            exhausted = true;
        }
    }
}
