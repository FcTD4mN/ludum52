using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
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
            Die();
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag == "Blocking")
        {
            Die();
            return;
        }

        Hitable hitable = coll.GetComponent<Hitable>();

        if (hitable == null)
            return;

        // Hit the target
        HitTarget(hitable);
    }

    protected void HitTarget(Hitable target)
    {
        // Todo: read damage from weaponStatManager
        target.Hit(30);
        Die();
    }

    public virtual void Die()
    {
        DestroySelf();
    }

    protected void DestroySelf()
    {
        Destroy(gameObject);
    }
}
