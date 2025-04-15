using TMPro;
using UnityEngine;

public class InventorySlot : MonoBehaviour
{
    private TextMeshProUGUI slotText;
    void Start()
    {
        slotText = GetComponentInChildren<TextMeshProUGUI>();
    }

    public void SetText(string _text)
    {
        slotText.text = _text;
    }
}
