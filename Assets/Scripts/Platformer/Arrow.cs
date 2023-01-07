using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float damage = 10f;
    public float speed = 5f;

    Rigidbody2D rb;

    private void Awake()
    {
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        rb.velocity = new Vector2(speed * transform.localScale.x, 0f);
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        // TODO : Hit 
        Debug.Log("Arrow hit target");
    }
}
