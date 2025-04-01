using System.Collections.Generic;
using System.Threading;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    private BoxCollider2D attackHitbox;
    private bool attacking = false;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        attackHitbox = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    public void Attack()
    {
        List<Collider2D> results = new List<Collider2D>();

        attackHitbox.Overlap(new Vector2(0f, 0f), 0f, results);

        print(results.Count);
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        print("Ouch!");
    }
}
