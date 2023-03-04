using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HitableLock : Hitable
{
    SpriteRenderer sr;

    public override void OnEnable()
    {
        sr = gameObject.GetComponent<SpriteRenderer>();
    }

    public override void Update()
    {
        // Nothing
    }

    public override void Hit(float damage)
    {
        InteractableDoor door = transform.parent.gameObject.GetComponent<InteractableDoor>();
        door.Interact();
        Color newColor = new Color(0f, 1f, 0f);
        sr.color = newColor;
    }
}
