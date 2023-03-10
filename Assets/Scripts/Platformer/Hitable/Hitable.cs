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
        }
    }

    [SerializeField]
    private bool isInvincible = false;
    public float invincibilityCooldown = 0.25f;
    float timeSinceHit = 0;

    protected Animator animator;

    // Start is called before the first frame update
    public virtual void OnEnable()
    {
        mStats = GetComponent<HasStats>();
        if (mStats.GetBaseStat(cStatsDescriptor.eStatsNames.MaxHealth) == 0)
        {
            mStats.SetBaseStat(cStatsDescriptor.eStatsNames.MaxHealth, 100);
        }
        if (mStats.GetBaseStat(cStatsDescriptor.eStatsNames.Health) == 0)
        {
            mStats.SetBaseStat(cStatsDescriptor.eStatsNames.Health, mStats.GetBaseStat(cStatsDescriptor.eStatsNames.MaxHealth));
        }
        animator = GetComponent<Animator>();
    }

    public virtual void Update()
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

    public virtual void Hit(float damage)
    {
        if (IsAlive && !isInvincible)
        {
            GameManager.mUIManager.FloatingMessage(damage.ToString(), Color.red, transform.position);
            Health -= damage;
            isInvincible = true;
            animator.SetTrigger("Hurt");
        }
    }

    void RemoveFromScene()
    {
        Destroy(gameObject);
    }
}
