using UnityEngine;

[CreateAssetMenu(fileName = "enemyData", menuName = "Scriptable Objects/enemyData")]
public class enemyData : ScriptableObject
{
    public string enemyName;
    public float speed = 0f;
    public float health = 0f;
    public float damage = 0f;
}
