using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class Explosive: Projectile
{
    private Vector2 mOriginalSize;
    private Vector2 mFinalSize;

    private float mTotalTime;
    private float mCurrentTime;


    public override void SetWeaponStats( cCompleteStats projectile, cCompleteStats resolution )
    {
        base.SetWeaponStats( projectile, resolution );

        // Explosion don't remain after being fully expanded
        // They expand to their max size, then disappear
        mOriginalWeaponResolutionStats.SetBaseStat( cStatsDescriptor.eStatsNames.WeaponLifeTime, resolution.GetBaseStat(cStatsDescriptor.eStatsNames.WeaponSpeed) );
        mDieTime = mSpawnTime + (uint)(mOriginalWeaponResolutionStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponLifeTime) * 1000);

        float expansionRange = mOriginalWeaponResolutionStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponSize);
        mTotalTime = mOriginalWeaponResolutionStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponSpeed);
        mCurrentTime = 0;
        mOriginalSize = Vector2.zero;
        mFinalSize = Vector2.one * expansionRange;
    }


    void FixedUpdate() // Func is unique to every class, no base.FixedUpdate() available
    {
        float dt = Time.deltaTime;
        if( Utilities.GetCurrentTimeEpochInMS() >= mDieTime )
            Die();

        mCurrentTime += dt;
        float lerpT = mCurrentTime / mTotalTime;

        transform.localScale = Vector2.Lerp( mOriginalSize, mFinalSize, lerpT );
    }


    override protected void OnTriggerEnter2D(Collider2D coll)
    {
        Hitable hitable = coll.GetComponent<Hitable>();
        if (hitable == null)
            return;

        // Hit the target
        HitTarget(hitable);
    }

    override protected void HitTarget(Hitable target)
    {
        target.Hit(mOriginalWeaponResolutionStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponDamage));
    }
}