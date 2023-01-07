using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public float destroyAfterDistance = 10f;
    private float startingPos;

    void OnEnable()
    {
        startingPos = transform.position.x;
    }

    void FixedUpdate()
    {
        // Destroy arrow after certain distance if not hitting anything
        if (Mathf.Abs(transform.position.x - startingPos) > destroyAfterDistance)
            Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {

        if (coll.tag == "Blocking")
        {
            Destroy(gameObject);
            return;
        }

        Hitable hitable = coll.GetComponent<Hitable>();

        if (hitable == null)
            return;

        // Hit the target
        if (hitable.Hit())
        {
            Debug.Log("We hit the object named : " + coll.name);
            Destroy(gameObject);
        }
    }
}
