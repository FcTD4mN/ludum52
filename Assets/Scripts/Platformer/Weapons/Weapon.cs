using System.Collections.Generic;
using UnityEngine;

public class cWeapon
{
    public GameObject mWeaponOwner;
    public cCompleteStats mStats;

    private uint mLastShootTime = 0;
    public bool mAutoFire = false;

    public cWeapon( GameObject owner )
    {
        mWeaponOwner = owner;
        cStatsDescriptor baseValues = new cStatsDescriptor();
        baseValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponCooldown.ToString()] = 0.2f;
        baseValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponDamage.ToString()] = 10f;
        baseValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponLifeTime.ToString()] = 5f;
        baseValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponRange.ToString()] = 10f;
        baseValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponSize.ToString()] = 1f;
        baseValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponSpeed.ToString()] = 1f;
        baseValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponPierceAmount.ToString()] = 0f;

        mStats = new cCompleteStats();
        mStats.SetBaseStats(baseValues);
    }


    // ===================================
    // Updates
    // ===================================
    public void Update( float dt )
    {
        if( !mAutoFire ) return;

        ShootPrimary( mWeaponOwner.transform.position );
    }


    // ===================================
    // Shooting
    // ===================================
    public bool CanShoot()
    {
        // Check resources
        if ( (int)GameManager.mResourceManager.GetRessource(cResourceDescriptor.eResourceNames.Arrows) <= 0 ) return  false;

        // Check time

        uint cur_time = Utilities.GetCurrentTimeEpochInMS();
        var weaponCoolDownInSec = mStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponCooldown);
        var timeWhenShootIsAvailable = mLastShootTime + (uint)(weaponCoolDownInSec * 1000f);

        return timeWhenShootIsAvailable <= cur_time;
    }


    public void ShootPrimary(Vector3 originPoint)
    {
        if (!CanShoot()) return;

        // Instantiate arrow
        GameObject arrowPrefab = Resources.Load<GameObject>("Prefabs/Platformer/Character/Arrow");
        GameObject arrow = GameObject.Instantiate(arrowPrefab, originPoint, arrowPrefab.transform.rotation);

        var projectileSize = mStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponSize);
        arrow.transform.localScale = new Vector3(arrow.transform.localScale.x * projectileSize, arrow.transform.localScale.y * projectileSize, 1);

        // Retrieve direction to next WP
        // Can be adjusted for AIM ASSIT
        Vector3 target = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2 directionRaw = (target - originPoint);
        Vector2 direction = directionRaw.normalized;

        // Move towards next WP
        float distance = Vector2.Distance(target, originPoint);
        Rigidbody2D rbArrow = arrow.GetComponent<Rigidbody2D>();
        rbArrow.velocity = direction * mStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponSpeed);

        var projectile = arrow.GetComponent<Projectile>();
        if (projectile != null)
        {
            projectile.SetWeapon( this );
        }

        GameManager.mResourceManager.AddResource(cResourceDescriptor.eResourceNames.Arrows, -1, false);

        mLastShootTime = Utilities.GetCurrentTimeEpochInMS();
    }
}