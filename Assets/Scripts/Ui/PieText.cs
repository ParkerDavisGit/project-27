using TMPro;
using UnityEngine;

public class PieText : MonoBehaviour
{
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        TextMeshProUGUI textBox = GetComponent<TextMeshProUGUI>();
        if (InventoryManager.Get("IceCream") == 1)
        {
            if (InventoryManager.Get("Apple") == 4 && InventoryManager.Get("Flour") == 5)
            {
                textBox.text = "Now this is a pie!\nThe creature is satisfied with this pie.";
            }
            else
            {
                textBox.text = "It's a nice pie, but it's missing something.\nThe ice cream is a nice touch.\nTry to find all the ingredients!";
            }
        }
        else
        {
            if (InventoryManager.Get("Apple") == 4 && InventoryManager.Get("Flour") == 5)
            {
                textBox.text = "The creature loves the pie.\nBut there's something it's missing it can't quite place.\nTry to find all the ingredients!";
            }
            else
            {
                textBox.text = "The creature believes this pie is missing something.\nA lot of somethings, actually.\nTry to find all the ingredients!";
            }
        }
    }
}
