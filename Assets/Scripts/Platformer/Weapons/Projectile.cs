using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Projectile : MonoBehaviour
{
    protected cCompleteStats mOriginalWeaponProjectileStats;
    protected cCompleteStats mOriginalWeaponResolutionStats;

    protected float startingPos;
    protected uint mSpawnTime = 0;
    protected uint mDieTime = 0;
    protected int mPierceRemaining = 0;
    protected cWeapon.eProjectileResolutionType mResolutionType;


    // ===================================
    // Building
    // ===================================
    void OnEnable()
    {
        Initialize();
    }


    protected virtual void Initialize()
    {
        startingPos = transform.position.x;
        mSpawnTime = Utilities.GetCurrentTimeEpochInMS();
    }


    // ===================================
    // Setters
    // ===================================
    public void SetWeapon(cWeapon weapon)
    {
        SetWeaponStats(weapon.mProjectileStats, weapon.mResolutionStats);
        mResolutionType = weapon.mProjectileResolutionType;
    }
    virtual public void SetWeaponStats( cCompleteStats projectile, cCompleteStats resolution )
    {
        mOriginalWeaponProjectileStats = new cCompleteStats(projectile);
        mOriginalWeaponResolutionStats = new cCompleteStats(resolution);
        mPierceRemaining = (int)projectile.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponPierceAmount);
        mDieTime = mSpawnTime + (uint)(mOriginalWeaponProjectileStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponLifeTime) * 1000);
    }


    // ===================================
    // GameLoops
    // ===================================
    void FixedUpdate()
    {
        var deltaTime = Time.fixedDeltaTime;
        if (mOriginalWeaponProjectileStats == null) return;

        // Destroy arrow after certain distance if not hitting anything
        if (Mathf.Abs(transform.position.x - startingPos) > mOriginalWeaponProjectileStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponRange))
            Die();

        if( Utilities.GetCurrentTimeEpochInMS() >= mDieTime )
            Die();

        PerformHoming( deltaTime );
    }


    // ===================================
    // Physics
    // ===================================
    virtual protected void OnTriggerEnter2D(Collider2D coll)
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
        else { Resolve(); } // So that every hit will resolve
    }

    virtual protected void OnTriggerExit2D(Collider2D coll)
    {
        // Nothing here
    }

    virtual protected void HitTarget(Hitable target)
    {
        target.Hit(mOriginalWeaponProjectileStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponDamage));
    }

    public virtual void Die()
    {
        Resolve();
        DestroySelf();
    }

    protected void DestroySelf()
    {
        Destroy(gameObject);
    }


    private void Resolve()
    {
        switch (mResolutionType)
        {
            case cWeapon.eProjectileResolutionType.kNone:
                break;
            case cWeapon.eProjectileResolutionType.kExplosion:
                CreateExplosion();
                break;
            case cWeapon.eProjectileResolutionType.kDamagingAreas:
                CreateDamagingArea();
                break;

            default:
                break;
        }
    }


    private void CreateExplosion()
    {
        if( mOriginalWeaponProjectileStats == null ) return;

        // Instantiate explosion
        GameObject explosionPrefab = Resources.Load<GameObject>("Prefabs/Platformer/Projectiles/ExplosionA");
        GameObject obj = GameObject.Instantiate(explosionPrefab, transform.position, explosionPrefab.transform.rotation);

        var objectSize = mOriginalWeaponProjectileStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponSize);
        obj.transform.localScale = new Vector3(obj.transform.localScale.x * objectSize, obj.transform.localScale.y * objectSize, 1);
        obj.GetComponent<Explosive>()?.SetWeaponStats( mOriginalWeaponProjectileStats, mOriginalWeaponResolutionStats );
    }


    private void CreateDamagingArea()
    {
        var daCreator = new cDamagingAreaCreator( mOriginalWeaponProjectileStats, mOriginalWeaponResolutionStats );
        daCreator.GenerateAtLocation( transform.position );
    }


    // ===================================
    // Behaviour logics
    // ===================================
    protected void PerformHoming( float deltaTime )
    {
        if( gameObject == null ) return;

        var homingForce = mOriginalWeaponProjectileStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponHomingForce);
        if( homingForce <= 0 )  return;

        var ennemyToGoTo = GetClosestEnnemy();
        if( ennemyToGoTo == null )  return;

        Rigidbody2D rb = GetComponent<Rigidbody2D>();

        Vector2 vectorToEnnemy = ennemyToGoTo.transform.position - transform.position;
        var desiredVector = vectorToEnnemy.normalized * mOriginalWeaponProjectileStats.GetFinalStat(cStatsDescriptor.eStatsNames.WeaponSpeed);

        Vector2 correctedVector = Vector2.Lerp( rb.velocity, desiredVector, deltaTime * homingForce );
        rb.velocity = correctedVector;
    }


    private GameObject GetClosestEnnemy()
    {
        float closest = -1;
        GameObject closestEnnemy = null;
        float threshold = mOriginalWeaponProjectileStats.GetFinalStat( cStatsDescriptor.eStatsNames.WeaponHomingDistance );
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
