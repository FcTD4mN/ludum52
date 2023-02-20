using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static cStatsDescriptor;

public class HasStats : MonoBehaviour
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
        if( !mHasBeenInitialized ) {
            Initialize();
        }

        mStatsBase = stats;
        UpdateStats();
    }
    public void SetBaseStat(eStatsNames type, float value)
    {
        if (!mHasBeenInitialized) {
            Initialize();
        }

        mStatsBase.mStatValues[type.ToString()] = value;
        UpdateStats();
    }

    // Adds values in stats into mStatsBonusAdd
    public void AddStatsAddition(cStatsDescriptor stats)
    {
        if (!mHasBeenInitialized) {
            Initialize();
        }

        mStatsBonusAdd.CombineByAddition(stats);
        UpdateStats();
    }

    // Adds values in stats into mStatsBonusMult
    public void AddStatsMultipliers(cStatsDescriptor stats)
    {
        if (!mHasBeenInitialized) {
            Initialize();
        }

        mStatsBonusMult.CombineByAddition(stats);
        UpdateStats();
    }


    public float GetFinalStat( eStatsNames type )
    {
        if (!mHasBeenInitialized) {
            Initialize();
        }
        if( !mStatsFinalCached.mStatValues.ContainsKey(type.ToString())) return  0f;

        return  mStatsFinalCached.mStatValues[ type.ToString() ];
    }


    public float GetBaseStat(eStatsNames type)
    {
        if (!mHasBeenInitialized) {
            Initialize();
        }
        if( !mStatsBase.mStatValues.ContainsKey(type.ToString())) return  0f;

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
