using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    private cWeapon mOriginalWeapon;

    private float startingPos;
    private uint mSpawnTime = 0;
    private int mPierceRemaining = 0;

    public void SetWeapon(cWeapon weapon)
    {
        mOriginalWeapon = weapon;
        mPierceRemaining = (int)weapon.mStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponPierceAmount);
    }

    void OnEnable()
    {
        startingPos = transform.position.x;
        mSpawnTime = Utilities.GetCurrentTimeEpochInMS();
    }

    void FixedUpdate()
    {
        if (mOriginalWeapon == null) return;

        // Destroy arrow after certain distance if not hitting anything
        if (Mathf.Abs(transform.position.x - startingPos) > mOriginalWeapon.mStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponRange))
            Die();

        if (Utilities.GetCurrentTimeEpochInMS() >= mSpawnTime + (uint)(mOriginalWeapon.mStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponLifeTime) * 1000))
            Die();
    }

    private void OnTriggerEnter2D(Collider2D coll)
    {
        if (coll.tag == "Blocking")
        {
            Die();
            return;
        }

        Hitable hitable = coll.GetComponent<Hitable>();

        if (hitable == null)
            return;

        // Hit the target
        HitTarget(hitable);
        --mPierceRemaining;

        if (mPierceRemaining < 0) Die();
    }

    protected void HitTarget(Hitable target)
    {
        target.Hit(mOriginalWeapon.mStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponDamage));
    }

    public virtual void Die()
    {
        DestroySelf();
    }

    protected void DestroySelf()
    {
        Destroy(gameObject);
    }
}
