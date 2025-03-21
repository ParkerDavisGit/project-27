using NUnit.Framework;
using UnityEngine;
using System.Collections.Generic;

public class Feet : MonoBehaviour
{
    public List<Collider2D> collisions;
    private BoxCollider2D feetCollider;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Awake()
    {
        collisions = new List<Collider2D>();
        feetCollider = GetComponent<BoxCollider2D>();
    }

    public bool OnGround()
    {
        feetCollider.Overlap(collisions);
        foreach (Collider2D obj in collisions)
        {
            if (obj.tag == "ground")
            {
                return true;
            }
        }
        return false;
    }
}
