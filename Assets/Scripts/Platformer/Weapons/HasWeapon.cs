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
    public float WeaponPierceAmount = 0f;

    void OnEnable()
    {
        mWeapon = new cWeapon( gameObject );
        mWeapon.mStats.SetBaseStat( cStatsDescriptor.eStatsNames.WeaponCooldown, WeaponCooldown );
        mWeapon.mStats.SetBaseStat( cStatsDescriptor.eStatsNames.WeaponDamage, WeaponDamage );
        mWeapon.mStats.SetBaseStat( cStatsDescriptor.eStatsNames.WeaponLifeTime, WeaponLifeTime );
        mWeapon.mStats.SetBaseStat( cStatsDescriptor.eStatsNames.WeaponRange, WeaponRange );
        mWeapon.mStats.SetBaseStat( cStatsDescriptor.eStatsNames.WeaponSize,WeaponSize );
        mWeapon.mStats.SetBaseStat( cStatsDescriptor.eStatsNames.WeaponSpeed, WeaponSpeed );
        mWeapon.mStats.SetBaseStat( cStatsDescriptor.eStatsNames.WeaponPierceAmount, WeaponPierceAmount );
    }


    void FixedUpdate()
    {
        mWeapon.Update(Time.fixedDeltaTime);
    }
}