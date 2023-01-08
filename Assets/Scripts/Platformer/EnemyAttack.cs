using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyAttack : MonoBehaviour
{
    Collider2D attackCollider;

    // Start is called before the first frame update
    void OnEnable()
    {
        attackCollider = GetComponent<Collider2D>();
    }

    void OnTriggerEnter2D(Collider2D collision)
    {
        Hitable hitable = collision.GetComponent<Hitable>();

        if (hitable == null)
            return;

        // TODO : Check PlayerStats 
        hitable.Hit(10);
    }
}
