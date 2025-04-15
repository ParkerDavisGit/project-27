using UnityEngine;

[CreateAssetMenu(fileName = "enemyDrops", menuName = "Scriptable Objects/enemyDrops")]
public class enemyDrops : ScriptableObject
{
    public string enemyName;
    public GameObject common_drop;
    public GameObject uncommon_drop;
}
