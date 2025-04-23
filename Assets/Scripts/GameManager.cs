using UnityEngine;

public class GameManager
{
    static bool hasBeenCreated = false;

    // Mono Objects
    static GameObject uiCanvas;
    static Player player;

    // Singleton Managers
    static InventoryManager inventory = null;

    static void Instantiate()
    {
        GameObject[] objectsInRoot = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject obj in objectsInRoot)
        {
            if (obj.CompareTag("UICanvas"))
            {
                uiCanvas = obj;
            }
            else if (obj.CompareTag("Player"))
            {
                player = obj.GetComponent<Player>();
            }
        }

        if (inventory == null)
        {
            inventory = InventoryManager.Instantiate();
        }

        inventory.PrintExistance();

        hasBeenCreated = true;
    }

    public static void ToggleInventory()
    {
        if (!hasBeenCreated)
        {
            GameManager.Instantiate();
        }
    }
}
