using System.Collections;
using System.Collections.Generic;
using UnityEngine;

class DamagingAreaSection : Projectile
{
    public cDamagingArea mDamagingArea;


    public override void SetWeaponStats(cCompleteStats projectile, cCompleteStats resolution)
    {
        base.SetWeaponStats(projectile, resolution);
        mDieTime = mSpawnTime + (uint)(mOriginalWeaponResolutionStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponLifeTime) * 1000);
    }

    void FixedUpdate() // Func is unique to every class, no base.FixedUpdate() available
    {
        float dt = Time.deltaTime;

        // Only one section handles the update call, otherwise it'll be called multiple times
        if( mDamagingArea.IsFirstSection(this) )
            mDamagingArea?.Update( dt );

        if (Utilities.GetCurrentTimeEpochInMS() >= mDieTime)
            Die();
    }


    // ===================================
    // Collision
    // ===================================
    override protected void OnTriggerEnter2D(Collider2D coll)
    {
        if (mDamagingArea == null) return;

        Hitable hitable = coll.GetComponent<Hitable>();
        if (hitable == null)
            return;

        mDamagingArea.mHitablesInArea.Add( hitable );
    }


    override protected void OnTriggerExit2D(Collider2D coll)
    {
        if( mDamagingArea == null ) return;

        Hitable hitable = coll.GetComponent<Hitable>();
        if (hitable == null)
            return;

        mDamagingArea.mHitablesInArea.Remove( hitable );
    }


    public override void Die()
    {
        mDamagingArea?.RemoveSection( this );
        base.Die();
    }

    override protected void HitTarget(Hitable target)
    {
        target.Hit(mOriginalWeaponResolutionStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponDamage));
    }
}



// ===================================
// The damaging area. Manages all sections, allowing to not tick multiple times when monsters are inside 2 subSections
// ===================================
class cDamagingArea
{
    private List<DamagingAreaSection> mSections;
    private cCompleteStats mResolutionStats;

    public List<Hitable> mHitablesInArea;
    private float mTick;
    private float mNextTick;


    public cDamagingArea(cCompleteStats resolution)
    {
        mSections = new List<DamagingAreaSection>();
        mHitablesInArea = new List<Hitable>();
        mResolutionStats = resolution;

        mTick = resolution.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponSpeed);
        mNextTick = mTick;
    }

    public void AddSection( DamagingAreaSection section )
    {
        section.mDamagingArea = this;
        mSections.Add( section );
    }
    public void RemoveSection( DamagingAreaSection section )
    {
        section.mDamagingArea = null;
        mSections.Remove( section );
    }
    public bool IsFirstSection( DamagingAreaSection section )
    {
        return  mSections.Count > 0 && mSections[0] == section;
    }


    public void Update( float deltaTime )
    {
        mNextTick -= deltaTime;
        if (mNextTick <= 0)
        {
            foreach (var hitable in mHitablesInArea)
            {
                if (hitable.gameObject == null) continue;
                HitTarget(hitable);
            }

            mNextTick = mTick + mNextTick; // mNextTick is negative or 0, so this substracts the extra dt
        }
    }

    private void HitTarget( Hitable target )
    {
        target.Hit(mResolutionStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponDamage));
    }
}
