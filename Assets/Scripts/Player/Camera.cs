using UnityEngine;

public class Camera : MonoBehaviour
{
    public MonoBehaviour tracking;
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(tracking.transform.position.x, tracking.transform.position.y + 0.5f, transform.position.z);
    }
}
