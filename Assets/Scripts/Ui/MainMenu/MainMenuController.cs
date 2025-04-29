using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MainMenuController : MonoBehaviour
{
    public Button startGameButton;

    void Start()
    {
        startGameButton.onClick.AddListener(onStartGame);
    }

    void onStartGame()
    {
        GameManager.LoadScene("IntroCutscene");
    }

    private void Update()
    {
        //Debug.Log("Hi!");
    }
}
