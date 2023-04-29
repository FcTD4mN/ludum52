using System.Collections.Generic;
using UnityEngine;

public class cWeapon
{
    public GameObject mWeaponOwner;
    public cCompleteStats mProjectileStats;
    public cCompleteStats mResolutionStats;

    private uint mLastShootTime = 0;
    public bool mAutoFire = false;



    public enum eProjectileType
    {
        kBasic
    };

    public enum eProjectileResolutionType
    {
        kNone,
        kExplosion,
        kDamagingAreas
    };

    public eProjectileType mProjectileType                          = eProjectileType.kBasic;
    public eProjectileResolutionType mProjectileResolutionType      = eProjectileResolutionType.kDamagingAreas;


    public cWeapon(GameObject owner)
    {
        mWeaponOwner = owner;
        cStatsDescriptor baseValues = new cStatsDescriptor();
        baseValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponCooldown.ToString()] = 0.2f;
        baseValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponDamage.ToString()] = 10f;
        baseValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponLifeTime.ToString()] = 5f;
        baseValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponRange.ToString()] = 10f;
        baseValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponSize.ToString()] = 1f;
        baseValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponSpeed.ToString()] = 1f;
        baseValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponProjectileCount.ToString()] = 1f;
        baseValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponPierceAmount.ToString()] = 0f;
        baseValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponHomingForce.ToString()] = 1f;
        baseValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponHomingDistance.ToString()] = 2f;

        mProjectileStats = new cCompleteStats();
        mProjectileStats.SetBaseStats(baseValues);


        // mResolutionStats = new cCompleteStats( mProjectileStats );

        cStatsDescriptor resolutionValues = new cStatsDescriptor();
        resolutionValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponCooldown.ToString()] = 0.2f;
        resolutionValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponDamage.ToString()] = 10f;
        resolutionValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponLifeTime.ToString()] = 5f;
        resolutionValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponRange.ToString()] = 10f;
        resolutionValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponSize.ToString()] = 2f;
        resolutionValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponSpeed.ToString()] = 3f;
        resolutionValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponProjectileCount.ToString()] = 1f;
        resolutionValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponPierceAmount.ToString()] = 0f;
        resolutionValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponHomingForce.ToString()] = 0f;
        resolutionValues.mStatValues[cStatsDescriptor.eStatsNames.WeaponHomingDistance.ToString()] = 0f;
        mResolutionStats = new cCompleteStats();
        mResolutionStats.SetBaseStats(resolutionValues);

    }


    // ===================================
    // Updates
    // ===================================
    public void Update(float dt)
    {
        if (!mAutoFire) return;

        ShootPrimary(mWeaponOwner.transform.position);
    }


    // ===================================
    // Shooting
    // ===================================
    public bool CanShoot()
    {
        // Check resources
        if ((int)GameManager.mResourceManager.GetRessource(cResourceDescriptor.eResourceNames.Arrows) <= 0) return false;

        // Check time

        uint cur_time = Utilities.GetCurrentTimeEpochInMS();
        var weaponCoolDownInSec = mProjectileStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponCooldown);
        var timeWhenShootIsAvailable = mLastShootTime + (uint)(weaponCoolDownInSec * 1000f);

        return timeWhenShootIsAvailable <= cur_time;
    }


    public void ShootPrimary(Vector3 originPoint)
    {
        if (!CanShoot()) return;

        Vector2 originNoZ = originPoint;
        Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

        // Instantiate arrow
        GameObject arrowPrefab = Resources.Load<GameObject>("Prefabs/Platformer/Character/Arrow");

        int projectileCount = (int)mProjectileStats.GetFinalStat( cStatsDescriptor.eStatsNames.WeaponProjectileCount );

        // Compute target point and distance from originPoint
        Vector2 mouseToCharVector = mousePos - originNoZ;
        float mouseToCharDistanceSQRD = mouseToCharVector.sqrMagnitude;

        float closeWideAngle = 180;
        float farTightAngle = 40;
        float closestDistance = 4; // 2 unit squared
        float furthestDistance = 49; // 7 units squared

        mouseToCharDistanceSQRD = Mathf.Clamp( mouseToCharDistanceSQRD, closestDistance, furthestDistance );
        float paramT = (mouseToCharDistanceSQRD - closestDistance) / (furthestDistance - closestDistance);
        float finalAngleLerped = Mathf.Lerp( closeWideAngle, farTightAngle, paramT );

        var splitTargets = SplitVector2( originNoZ, mousePos, finalAngleLerped, projectileCount );

        for (int i = 0; i < projectileCount; i++)
        {
            GameObject arrow = GameObject.Instantiate(arrowPrefab, originPoint, arrowPrefab.transform.rotation);

            var projectileSize = mProjectileStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponSize);
            arrow.transform.localScale = new Vector3(arrow.transform.localScale.x * projectileSize, arrow.transform.localScale.y * projectileSize, 1);

            Vector2 subTarget = splitTargets[i];
            Vector2 directionRaw = (subTarget - (Vector2)originPoint);
            Vector2 direction = directionRaw.normalized;

            // Move towards next WP
            float distance = Vector2.Distance(subTarget, originPoint);
            Rigidbody2D rbArrow = arrow.GetComponent<Rigidbody2D>();
            rbArrow.velocity = direction * mProjectileStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponSpeed);

            var projectile = arrow.GetComponent<Projectile>();
            if (projectile != null)
            {
                projectile.SetWeapon(this);
            }
        }

        GameManager.mResourceManager.AddResource(cResourceDescriptor.eResourceNames.Arrows, -1, false);
        mLastShootTime = Utilities.GetCurrentTimeEpochInMS();
    }


    // Used for multishots: Splits an angle <angleMax> into <splitCount> sub parts to generate <splitCount> points splitting appart from the center
    // of the angle cone
    private List<Vector2> SplitVector2(Vector2 center, Vector2 target, float angleMax, int splitCount)
    {
        if( splitCount == 1 ) return  new List<Vector2> { target };

        // Spliting angle into one vector output means we split angle in 2; two vectors output => split in 3
        float angleStep = angleMax / (float)(splitCount + 1);
        Vector2 targetVector = target - center;

        Vector2 startingVector = Quaternion.Euler( 0, 0, angleMax/2 ) * targetVector;
        Vector2 targetPointVector = startingVector;
        var output = new List<Vector2>();
        for( int i = 0; i < splitCount; ++i )
        {
            targetPointVector = Quaternion.Euler(0, 0, -angleStep) * targetPointVector;
            output.Add( center + targetPointVector );
        }

        return  output;
    }
}