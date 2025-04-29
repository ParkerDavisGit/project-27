using System;
using UnityEngine;

public class InventoryManager
{
    static InventoryManager instance = null;

    private int apples;
    private int flours;
    private int icecreams;

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

    public static int Get(string id)
    {
        if (id.Equals("Apple"))
        {
            return instance.apples;
        }
        else if (id.Equals("Flour"))
        {
            return instance.flours;
        }
        else if (id.Equals("IceCream"))
        {
            return instance.icecreams;
        }
        return 0;
    }

    public static void Add(string id)
    {
        if (id.Equals("Apple"))
        {
            instance.apples += 1;
        }
        else if (id.Equals("Flour"))
        {
            instance.flours += 1;
        }
        else if (id.Equals("IceCream"))
        {
            instance.icecreams += 1;
        }
    }

    public static void Reset()
    {
        instance.apples = 0;
        instance.flours = 0;
        instance.icecreams = 0;
    }

    public void PrintExistance()
    {
        Debug.Log("Inventory Exists!");
    }
}
