using UnityEngine;

public class Camera : MonoBehaviour
{
    public MonoBehaviour tracking;
    // Update is called once per frame
    void FixedUpdate()
    {
        transform.position = new Vector3(tracking.transform.position.x, transform.position.y, transform.position.z);
    }
}
