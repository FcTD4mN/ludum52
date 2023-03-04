using System;
using System.Collections.Generic;
using UnityEngine;

public abstract class WeaponBuilding : BuffBuilding
{
    // ===================================
    // Internal Setup
    // ===================================
    override internal void Initialize()
    {
        base.Initialize();
        mObjectToApplyStatsTo = GameManager.mInstance.playerCtrler.mWeaponComponent.mWeapon.mStats;
    }
}
