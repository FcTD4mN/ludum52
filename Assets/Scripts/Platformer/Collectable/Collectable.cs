using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    protected string label;
    protected float value;
    Rigidbody2D rb;

    void OnEnable()
    {
        Initialize();
    }

    protected void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.name == "Character")
        {
            Destroy(gameObject);
        }
        else if (coll.tag == "Blocking")
        {
            // Prevent from going through floor
            rb.bodyType = RigidbodyType2D.Static;
        }

    }
}
