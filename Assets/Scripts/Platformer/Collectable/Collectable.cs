using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collectable : MonoBehaviour
{
    protected string label;
    public float value;
    Rigidbody2D rb;
    protected SpriteRenderer sr;

    void OnEnable()
    {
        Initialize();
    }

    protected void Initialize()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.name == "Character")
        {
            Collect();
            Destroy(gameObject);
        }
        else if (coll.tag == "Blocking")
        {
            // Prevent from going through floor
            rb.bodyType = RigidbodyType2D.Static;
        }

    }

    virtual public void Collect()
    {
        // Implement
    }
}
