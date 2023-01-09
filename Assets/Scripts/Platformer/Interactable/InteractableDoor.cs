using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InteractableDoor : Interactable
{
    public Sprite openedDoor;
    BoxCollider2D coll;
    Animator animator;

    internal override void Initialize()
    {
        base.Initialize();
        mShowButton = false;
        animator = GetComponent<Animator>();
        coll = GetComponent<BoxCollider2D>();
        animator.enabled = false;
    }

    public override void Interact()
    {
        animator.enabled = true;
        coll.enabled = false;
    }
}
