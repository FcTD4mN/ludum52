using System;
using System.Collections.Generic;
using UnityEngine;

using static cStatsDescriptor;

public class cStatsDescriptor
{
    public enum eStatsNames
    {
        RunSpeed,
        AirWalkSpeed,
        AirWallSpeed,
        DashSpeed,
        JumpImpulse,
        CoolDownDash,
        Health,
        MaxHealth,
        // Weapon
        WeaponCooldown,
        WeaponDamage,
        WeaponSpeed,
        WeaponLifeTime,
        WeaponRange,
        WeaponSize,
        WeaponHomingForce, // Strength of homing correction towards ennemies, 0 = no homing
        WeaponHomingDistance, // How far should the ennemy be before homing toward it
        WeaponProjectileCount,
        WeaponPierceAmount
    }


    public Dictionary<string, float> mStatValues;

    public cStatsDescriptor()
    {
        mStatValues = new Dictionary<string, float>();
    }

    public cStatsDescriptor( cStatsDescriptor other )
    {
        mStatValues = new Dictionary<string, float>();

        foreach (var pair in other.mStatValues)
        {
            mStatValues[pair.Key] = pair.Value;
        }
    }


    // ===================================
    // Manipulation
    // ===================================
    public void CombineByAddition( cStatsDescriptor rhs )
    {
        foreach (eStatsNames stat in Enum.GetValues(typeof(eStatsNames)))
        {
            var statName = stat.ToString();
            if( !mStatValues.ContainsKey(statName) && !rhs.mStatValues.ContainsKey(statName) ) continue;
            else if( mStatValues.ContainsKey(statName) && !rhs.mStatValues.ContainsKey(statName) ) continue;
            else if( !mStatValues.ContainsKey(statName) && rhs.mStatValues.ContainsKey(statName) ) mStatValues[statName] = rhs.mStatValues[statName];
            else mStatValues[statName] += rhs.mStatValues[statName];
        }
    }

    public void CombineByMultiplication( cStatsDescriptor rhs )
    {
        foreach (eStatsNames stat in Enum.GetValues(typeof(eStatsNames)))
        {
            var statName = stat.ToString();
            if (mStatValues.ContainsKey(statName) && rhs.mStatValues.ContainsKey(statName))
                mStatValues[statName] *= rhs.mStatValues[statName];
        }
    }

    public void ApplyOnEveryStat(Func<float, float> action)
    {
        foreach (eStatsNames stat in Enum.GetValues(typeof(eStatsNames)))
        {
            var statName = stat.ToString();

            mStatValues[statName] = action( mStatValues.ContainsKey(statName) ? mStatValues[statName] : 0f );
        }
    }

    public void ApplyOnEveryPresentStat(Func<float, float> action)
    {
        foreach (eStatsNames stat in Enum.GetValues(typeof(eStatsNames)))
        {
            var statName = stat.ToString();
            if (!mStatValues.ContainsKey(statName)) continue;

            mStatValues[statName] = action(mStatValues[statName]);
        }
    }


    public void DEBUGLog()
    {
        foreach (eStatsNames stat in Enum.GetValues(typeof(eStatsNames)))
        {
            var statName = stat.ToString();
            Debug.Log( statName + ": " + mStatValues[statName] );
        }

    }
}


public class cCompleteStats
{
    public cStatsDescriptor mStatsBase;         // Changing requires to update finalStatsCached
    private cStatsDescriptor mStatsBonusAdd;    // Changing requires to update finalStatsCached
    private cStatsDescriptor mStatsBonusMult;   // Changing requires to update finalStatsCached
    private cStatsDescriptor mStatsFinalCached;

    private bool mHasBeenInitialized = false;

    private void Initialize()
    {
        BuildStats();
        mHasBeenInitialized = true;
    }


    // ===================================
    // Stats
    // ===================================
    public void SetBaseStats(cStatsDescriptor stats)
    {
        if (!mHasBeenInitialized)
        {
            Initialize();
        }

        mStatsBase = stats;
        UpdateStats();
    }
    public void SetBaseStat(eStatsNames type, float value)
    {
        if (!mHasBeenInitialized)
        {
            Initialize();
        }

        mStatsBase.mStatValues[type.ToString()] = value;
        UpdateStats();
    }

    // Adds values in stats into mStatsBonusAdd
    public void AddStatsAddition(cStatsDescriptor stats)
    {
        if (!mHasBeenInitialized)
        {
            Initialize();
        }

        mStatsBonusAdd.CombineByAddition(stats);
        UpdateStats();
    }

    // Adds values in stats into mStatsBonusMult
    public void AddStatsMultipliers(cStatsDescriptor stats)
    {
        if (!mHasBeenInitialized)
        {
            Initialize();
        }

        mStatsBonusMult.CombineByAddition(stats);
        UpdateStats();
    }


    public float GetFinalStat(eStatsNames type)
    {
        if (!mHasBeenInitialized)
        {
            Initialize();
        }
        if (!mStatsFinalCached.mStatValues.ContainsKey(type.ToString())) return 0f;

        return mStatsFinalCached.mStatValues[type.ToString()];
    }


    public float GetBaseStat(eStatsNames type)
    {
        if (!mHasBeenInitialized)
        {
            Initialize();
        }
        if (!mStatsBase.mStatValues.ContainsKey(type.ToString())) return 0f;

        return mStatsBase.mStatValues[type.ToString()];
    }


    private void BuildStats()
    {
        mStatsBase = new cStatsDescriptor();
        mStatsBonusAdd = new cStatsDescriptor();
        mStatsBonusMult = new cStatsDescriptor();

        mStatsBonusAdd.ApplyOnEveryStat(val => 0); // (val) => {return  0}. Sets all values to 0
        mStatsBonusMult.ApplyOnEveryStat(val => 1); // All to 1

        UpdateStats();
    }


    private cStatsDescriptor GetFinalStats()
    {
        cStatsDescriptor output = new cStatsDescriptor();
        output.CombineByAddition(mStatsBase);
        output.CombineByAddition(mStatsBonusAdd);
        output.CombineByMultiplication(mStatsBonusMult);

        return output;
    }


    private void UpdateStats()
    {
        mStatsFinalCached = GetFinalStats();
    }
}