using UnityEngine;

public class EndingScrawl : MonoBehaviour
{
    private RectTransform textBox;
    private float speed = 15f;
    private void Start()
    {
        textBox = GetComponent<RectTransform>();
    }
    private void Update()
    {
        transform.position += new Vector3(0, speed * Time.deltaTime, 0);
    }
}
