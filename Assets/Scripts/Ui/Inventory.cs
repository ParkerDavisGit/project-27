using System.Collections;
using System.Text.RegularExpressions;
using TMPro;
using UnityEngine;

public class Inventory : MonoBehaviour
{
    class Item
    { 
        public string name;
        public int count;
        public Sprite sprite;

        public Item(string _name, int _count)
        {
            name = _name;
            count = _count;
            //sprite = _sprite;
        }
    }

    private Item apple;
    public InventorySlot appleText;

    public SpriteDex sprites;
    private PlayerAction action;

    private ArrayList slots;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        gameObject.SetActive(false);
        apple = new Item("Apple", 0);
        action = new PlayerAction();
    }

    public void ToggleInventory()
    {
        if (gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);
        }
        else
        {
            UpdateDisplay();
            gameObject.SetActive(true);
        }
    }

    void UpdateDisplay()
    {
        appleText.SetText("Apple: " + apple.count);
    }

    public void Add(string name)
    {
        if (name.Equals("Apple"))
        {
            apple.count++;
        }
    }
}
