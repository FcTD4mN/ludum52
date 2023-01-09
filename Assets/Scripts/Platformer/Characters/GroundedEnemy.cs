using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundedEnemy : Enemy
{
    public DetectionZone backZone;

    public bool HasEnemyBack
    {
        get
        {
            return backZone.detectedColliders.Count > 0;
        }
    }

    void Update()
    {
        HasTarget = (attackZone.detectedColliders.Count > 0 && GameManager.mInstance.playerCtrler.IsAlive);

        if (HasEnemyBack && GameManager.mInstance.playerCtrler.IsAlive)
        {
            Debug.Log("Back trigger : " + backZone.detectedColliders.Count);
            FlipDirection();
        }
    }
}
