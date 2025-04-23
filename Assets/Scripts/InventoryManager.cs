using System;
using UnityEngine;

public class InventoryManager
{
    static InventoryManager instance = null;

    InventoryManager()
    {
        // Placeholder
    }

    public static InventoryManager Instantiate()
    {
        if (instance == null)
        {
            instance = new InventoryManager();
        }

        return instance;
    }


    public void PrintExistance()
    {
        Debug.Log("Inventory Exists!");
    }
}
