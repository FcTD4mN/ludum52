using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyProjectile : Projectile
{
    Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public override void Die()
    {
        animator.SetTrigger("Die");
    }
}
