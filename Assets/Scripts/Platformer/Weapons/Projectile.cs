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
        var deltaTime = Time.fixedDeltaTime;
        if (mOriginalWeapon == null) return;

        // Destroy arrow after certain distance if not hitting anything
        if (Mathf.Abs(transform.position.x - startingPos) > mOriginalWeapon.mStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponRange))
            Die();

        if (Utilities.GetCurrentTimeEpochInMS() >= mSpawnTime + (uint)(mOriginalWeapon.mStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponLifeTime) * 1000))
            Die();

        PerformHoming( deltaTime );
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



    private void PerformHoming( float deltaTime )
    {
        if( gameObject == null ) return;

        var homingForce = mOriginalWeapon.mStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponHomingForce);
        if( homingForce <= 0 )  return;

        var ennemyToGoTo = GetClosestEnnemy();
        if( ennemyToGoTo == null )  return;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        Vector2 vectorToEnnemy = ennemyToGoTo.transform.position - transform.position;
        var desiredVector = vectorToEnnemy.normalized * mOriginalWeapon.mStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponSpeed);

        Vector2 correctedVector = Vector2.Lerp( rb.velocity, desiredVector, deltaTime * homingForce );
        rb.velocity = correctedVector;
    }


    private GameObject GetClosestEnnemy()
    {
        float closest = -1;
        GameObject closestEnnemy = null;
        float threshold = mOriginalWeapon.mStats.GetFinalStat( cStatsDescriptor.eStatsNames.WeaponHomingDistance );
        threshold *= threshold; // Square it to compare with squared distances

        var pfWorld = GameObject.Find("PlateFormeWorld/AllEnnemies").gameObject;
        foreach( Transform group in pfWorld.transform )
        {
            foreach (Transform child in group.transform)
            {
                var ennemiComp = child.gameObject.GetComponent<Enemy>();
                if( ennemiComp == null ) continue;

                Vector2 vector = ennemiComp.transform.position - transform.position;
                float distance = vector.sqrMagnitude;
                if( distance > threshold ) continue;

                if( closest == -1 || distance < closest )
                {
                    closest = distance;
                    closestEnnemy = child.gameObject;
                }
            }
        }

        return  closestEnnemy;
    }
}
