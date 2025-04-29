using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager
{
    static bool hasBeenCreated = false;

    // Mono Objects
    static GameObject uiCanvas;
    static Player player;
    static CurrentLevelHandler level;
    static Jukebox jukebox;

    static HealthDisplay uiHealth;
    //static uiInventory;

    // Singleton Managers
    static InventoryManager inventory = null;


    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    static void OnAfterSceneLoadRuntimeMethod()
    {
        GameManager.Instantiate();
    }

    public static void Instantiate()
    {
        Debug.Log("Instantiatign Self");
        GameObject[] objectsInRoot = UnityEngine.SceneManagement.SceneManager.GetActiveScene().GetRootGameObjects();

        foreach (GameObject obj in objectsInRoot)
        {
            if (obj.CompareTag("UICanvas"))
            {
                Debug.Log("AHHHHHHHHHHHHHHHHHHHHHHHHHHHHHH");
                uiCanvas = obj;
                foreach (Transform child in uiCanvas.transform)
                {
                    if (child.CompareTag("UIHealthDisplay"))
                    {
                        uiHealth = child.GetComponent<HealthDisplay>();
                        uiHealth.PrintExistance();
                    }
                }
                
                Debug.Log("WallaWalla");
                Debug.Log(uiHealth);
            }
            else if (obj.CompareTag("Player"))
            {
                player = obj.GetComponent<Player>();
            }
            else if (obj.CompareTag("LevelHandler"))
            {
                level = obj.GetComponent<CurrentLevelHandler>();
            }
            else if (obj.CompareTag("Jukebox"))
            {
                jukebox = obj.GetComponent<Jukebox>();
            }
        }

        if (inventory == null)
        {
            inventory = InventoryManager.Instantiate();
        }

        hasBeenCreated = true;
    }

    public static void ToggleInventory()
    {
        if (!hasBeenCreated)
        {
            GameManager.Instantiate();
        }
    }

    public static void UpdatePlayerHealth(int _new_health)
    {
        uiHealth.UpdateHealth(_new_health);
    }


    public static void EventTrigger(string name)
    {
        if (name.Equals("ActivateFloatingPlatformOne"))
            level.ActivateTitan();
        else if (name.Equals("EndLevel"))
        {
            LoadScene("EndRating");
        }
    }

    public static void PlayTrack(int id)
    {
        jukebox.playTrack(id);
    }

    public static void LoadScene(string _name)
    {
        if (_name.Equals("MainMenu"))
        {
            InventoryManager.Reset();
        }
        SceneManager.LoadScene(_name);
        //GameManager.Instantiate();
    }
}
