using TMPro;
using UnityEngine;

public class IngredientsText : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TextMeshProUGUI textBox = GetComponent<TextMeshProUGUI>();
        textBox.text = "" + InventoryManager.Get("Apple") + " / 4\n\n"
                          + InventoryManager.Get("Flour") + " / 5\n\n"
                          + InventoryManager.Get("IceCream") + " / 1";
    }
}
