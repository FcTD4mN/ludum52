using System;
using System.Collections;
using System.Collections.Generic;

using UnityEngine;

// ===================================
// Handles creation of all sub areas + animation
// ===================================
public class cDamagingAreaCreatorFalling
{
    private cCompleteStats mWeaponStatsProjectile;
    private cCompleteStats mWeaponStatsResolution;
    public cDamagingAreaCreatorFalling(cCompleteStats projStats, cCompleteStats resolStats)
    {
        mWeaponStatsProjectile = new cCompleteStats(projStats);
        mWeaponStatsResolution = new cCompleteStats(resolStats);
    }

    // ===================================
    // Generation
    // ===================================
    public void GenerateAtLocation(Vector2 position)
    {
        if (mWeaponStatsProjectile == null) return;
        if (mWeaponStatsResolution == null) return;

        cDamagingArea area = new cDamagingArea(mWeaponStatsResolution);

        // Instantiate explosion
        GameObject damagingAreaPrefab = Resources.Load<GameObject>("Prefabs/Platformer/Projectiles/DamagingAreaSection1");

        var areaCount = mWeaponStatsResolution.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponSize);
        float totalRequiredWidthInUnit = areaCount;
        // xMin                        +0.5 to get xMid
        float leftMostCenterX = (position.x - totalRequiredWidthInUnit / 2) + 0.5f;
        Vector2 finalSpawnPosition = new Vector2(leftMostCenterX, position.y);

        for (int i = 0; i < areaCount; i++)
        {
            /*
                Add animation from position to finalSpawnLocation
            */

            // TODO: Add boxcast to ensure subSection isn't spawning inside any wall
            // Physics2D.BoxCast(  )

            GameObject obj = GameObject.Instantiate(damagingAreaPrefab, finalSpawnPosition, damagingAreaPrefab.transform.rotation);
            var damagingAreaSection = obj.GetComponent<DamagingAreaSection>();
            damagingAreaSection?.SetWeaponStats(mWeaponStatsProjectile, mWeaponStatsResolution);
            area.AddSection(damagingAreaSection);

            finalSpawnPosition += new Vector2(1f, 0f);
        }
    }
}


// ===================================
// Handles creation of sub area + animation
// ===================================
public class cDamagingAreaCreatorStatic
{
    private GameObject mObjectToAnimate;

    private cCompleteStats mWeaponStatsProjectile;
    private cCompleteStats mWeaponStatsResolution;

    public cDamagingAreaCreatorStatic(cCompleteStats projStats, cCompleteStats resolStats)
    {
        mWeaponStatsProjectile = new cCompleteStats(projStats);
        mWeaponStatsResolution = new cCompleteStats(resolStats);
    }

    // ===================================
    // Generation
    // ===================================
    public void GenerateAtLocation(Vector2 position)
    {
        if (mWeaponStatsProjectile == null) return;
        if (mWeaponStatsResolution == null) return;

        cDamagingArea area = new cDamagingArea(mWeaponStatsResolution);
        var size = mWeaponStatsResolution.GetFinalStat( cStatsDescriptor.eStatsNames.WeaponSize );

        // Instantiate explosion
        GameObject damagingAreaPrefab = Resources.Load<GameObject>("Prefabs/Platformer/Projectiles/DamagingAreaStatic");

        GameObject obj = GameObject.Instantiate(damagingAreaPrefab, position, damagingAreaPrefab.transform.rotation);
        var damagingAreaSection = obj.GetComponent<DamagingAreaSection>();

        var finalSize = new Vector3( size, size, 1 );
        obj.transform.localScale = Vector3.zero;
        mObjectToAnimate = obj;
        GameManager.mInstance.StartCoroutine( _ExpandAnimation( 1f, finalSize ) );


        damagingAreaSection?.SetWeaponStats(mWeaponStatsProjectile, mWeaponStatsResolution);
        area.AddSection(damagingAreaSection);
    }


    // Using coroutine so that the whole creation / animations to get into position is fully focused here, and the area sections
    // only handles the damage and global existence logic, not the creation part
    // Also allows for different style of creation animations
    private IEnumerator _ExpandAnimation( float time, Vector3 finalSize )
    {
        float timer = time;
        float t = 0;
        float timerInv = 1/time;
        Vector3 originalSize = mObjectToAnimate.transform.localScale;

        while (timer > 0)
        {
            float deltaTime = Time.deltaTime;
            timer -= deltaTime;

            t = 1 - timer * timerInv;
            var currentSize = Vector3.Lerp( originalSize, finalSize, t );
            mObjectToAnimate.transform.localScale = currentSize;

            yield return null;
        }

        mObjectToAnimate = null;
    }
}