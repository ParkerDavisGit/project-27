using UnityEngine;

public class OpeningScrawl : MonoBehaviour
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

        Debug.Log(textBox.position.y);
        if (textBox.position.y > -130)
        {
            GameManager.LoadScene("CityScape");
        }

        if (Input.anyKeyDown)
        {
            speed = 100;
        }
    }
}
