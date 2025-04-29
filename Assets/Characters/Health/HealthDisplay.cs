using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class HealthDisplay : MonoBehaviour
{
    //private  healthText;
    public Sprite[] healthBarSprites;

    private Image image;
    void Awake()
    {
        image = gameObject.GetComponent<Image>();
    }

    public void UpdateHealth(int _amount)
    {
        image.overrideSprite = healthBarSprites[_amount];
    }

    public void PrintExistance()
    {
        Debug.Log("Gello World.");
    }
}
