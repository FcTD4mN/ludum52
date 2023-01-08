using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hitable : MonoBehaviour
{
    private HasStats mStats;

    public float Health
    {
        get
        {
            return mStats.GetFinalStat(cStatsDescriptor.eStatsNames.Health);
        }
        set
        {
            mStats.SetBaseStat(cStatsDescriptor.eStatsNames.Health, value);
            if (value <= 0)
            {
                IsAlive = false;
            }
        }
    }

    private bool isAlive = true;
    public bool IsAlive
    {
        get
        {
            return isAlive;
        }
        set
        {
            isAlive = value;
            animator.SetBool("isAlive", value);
            Debug.Log("Alive valute : " + IsAlive);
        }
    }

    [SerializeField]
    private bool isInvincible = false;
    public float invincibilityCooldown = 0.25f;
    float timeSinceHit = 0;

    Animator animator;

    // Start is called before the first frame update
    void OnEnable()
    {
        animator = GetComponent<Animator>();
        mStats = GetComponent<HasStats>();
        if (mStats.GetBaseStat(cStatsDescriptor.eStatsNames.MaxHealth) == 0)
        {
            mStats.SetBaseStat(cStatsDescriptor.eStatsNames.MaxHealth, 100);
        }
        if( mStats.GetBaseStat(cStatsDescriptor.eStatsNames.Health) == 0 )
        {
            mStats.SetBaseStat( cStatsDescriptor.eStatsNames.Health, mStats.GetBaseStat(cStatsDescriptor.eStatsNames.MaxHealth) );
        }
    }

    void Update()
    {
        if (isInvincible)
        {
            // Remove invincibility after a time
            if (timeSinceHit > invincibilityCooldown)
            {
                isInvincible = false;
                timeSinceHit = 0;
            }

            timeSinceHit += Time.deltaTime;
        }
    }

    public void Hit(int damage)
    {
        // Debug.Log("We hit the object named : " + gameObject.name);
        if (IsAlive && !isInvincible)
        {
            Health -= damage;
            isInvincible = true;
            animator.SetTrigger("Hurt");
            // Debug.Log("health = " + Health);
        }
    }

    void RemoveFromScene()
    {
        Destroy(gameObject);
    }
}
