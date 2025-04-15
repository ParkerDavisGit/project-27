using TMPro;
using UnityEngine;

public class HealthDisplay : MonoBehaviour
{
    private TextMeshProUGUI healthText;
    void Start()
    {
        healthText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetText(string _text)
    {
        healthText.text = _text;
    }
}
