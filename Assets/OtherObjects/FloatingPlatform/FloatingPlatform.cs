using System.Collections;
using Unity.VisualScripting;
using UnityEngine;

public class FloatingPlatform : MonoBehaviour
{
    private Rigidbody2D rb;
    private bool moving = false;
    private Vector3 direction;
    public Transform stopPoint;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    public void GoTo(Vector2 _destination)
    {
        Debug.Log("Here we go!");
        rb = GetComponent<Rigidbody2D>();
        direction = new Vector3(1, 0, 0).normalized;
        StartCoroutine(TheMoveWait());
    }

    private IEnumerator TheMoveWait()
    {
        yield return new WaitForSeconds(2.2f);
        moving = true;
    }

    public void Update()
    {
        if (!moving)
            return;

        transform.position += direction  * Time.deltaTime;
        if (transform.position.x > stopPoint.position.x)
        {
            moving = false;
        }
    }
}
