using System.Collections.Generic;
using UnityEngine;

public class HasWeapon : MonoBehaviour
{
    public cWeapon mWeapon;


    // Exposed stats for easy tweaks in editor
    public float WeaponCooldown = 0.2f;
    public float WeaponDamage = 10f;
    public float WeaponLifeTime = 5f;
    public float WeaponRange = 10f;
    public float WeaponSize = 1f;
    public float WeaponSpeed = 1f;
    public float WeaponProjectileCount = 1f;
    public float WeaponPierceAmount = 0f;
    public float WeaponHomingDistance = 2f;
    public float WeaponHomingForce = 0f;

    void OnEnable()
    {
        mWeapon = new cWeapon( gameObject );
        mWeapon.mProjectileStats.SetBaseStat( cStatsDescriptor.eStatsNames.WeaponCooldown, WeaponCooldown );
        mWeapon.mProjectileStats.SetBaseStat( cStatsDescriptor.eStatsNames.WeaponDamage, WeaponDamage );
        mWeapon.mProjectileStats.SetBaseStat( cStatsDescriptor.eStatsNames.WeaponLifeTime, WeaponLifeTime );
        mWeapon.mProjectileStats.SetBaseStat( cStatsDescriptor.eStatsNames.WeaponRange, WeaponRange );
        mWeapon.mProjectileStats.SetBaseStat( cStatsDescriptor.eStatsNames.WeaponSize,WeaponSize );
        mWeapon.mProjectileStats.SetBaseStat( cStatsDescriptor.eStatsNames.WeaponSpeed, WeaponSpeed );
        mWeapon.mProjectileStats.SetBaseStat( cStatsDescriptor.eStatsNames.WeaponProjectileCount, WeaponProjectileCount );
        mWeapon.mProjectileStats.SetBaseStat( cStatsDescriptor.eStatsNames.WeaponPierceAmount, WeaponPierceAmount );
        mWeapon.mProjectileStats.SetBaseStat( cStatsDescriptor.eStatsNames.WeaponHomingDistance, WeaponHomingDistance );
        mWeapon.mProjectileStats.SetBaseStat( cStatsDescriptor.eStatsNames.WeaponHomingForce, WeaponHomingForce );
    }


    void FixedUpdate()
    {
        mWeapon.Update(Time.fixedDeltaTime);
    }
}