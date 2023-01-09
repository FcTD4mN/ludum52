using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingAttackEnemy : FlyingEnemy
{
    public float airAttackSpeed = 3f;
    public float airAttackCoolDown = 1f;
    private float cooldownElapsed = 0.0f;

    public void FireProjectile()
    {
        // Instantiate arrow
        GameObject arrowPrefab = Resources.Load<GameObject>("Prefabs/Platformer/Enemy/EnemyProjectile");
        GameObject arrow = Instantiate(arrowPrefab, transform.position, arrowPrefab.transform.rotation);

        // Retrieve direction to next WP
        // Can be adjusted if multiple target
        Transform target = GameManager.mInstance.playerCtrler.transform;
        Vector2 direction = (target.position - transform.position).normalized;

        // Move towards next WP
        float distance = Vector2.Distance(target.position, transform.position);
        Rigidbody2D rbArrow = arrow.GetComponent<Rigidbody2D>();
        rbArrow.velocity = direction * airAttackSpeed;
    }

    void Update()
    {
        HasTarget = (attackZone.detectedColliders.Count > 0 && GameManager.mInstance.playerCtrler.IsAlive);

        if (HasTarget)
        {
            if (airAttackCoolDown > cooldownElapsed)
                cooldownElapsed += Time.deltaTime;
            else
            {
                cooldownElapsed = 0;
                animator.SetTrigger("AirAttack");
            }
        }

    }


}
