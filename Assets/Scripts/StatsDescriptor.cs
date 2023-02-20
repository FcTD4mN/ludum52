using System;
using System.Collections.Generic;
using UnityEngine;

public class cStatsDescriptor
{
    public enum eStatsNames
    {
        RunSpeed,
        AirWalkSpeed,
        AirWallSpeed,
        DashSpeed,
        JumpImpulse,
        CoolDownAttack,
        CoolDownDash,
        Damage,
        Health,
        MaxHealth
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
            if (mStatValues.ContainsKey(statName) && rhs.mStatValues.ContainsKey(statName)) mStatValues[statName] += rhs.mStatValues[statName];
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