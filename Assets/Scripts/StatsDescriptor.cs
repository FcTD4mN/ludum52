using System;
using System.Collections.Generic;
using UnityEngine;

public class cStatsDescriptor
{
    public static List<string> mAllStatsName;
    public enum eStatsNames
    {
        RunSpeed,
        AirWalkSpeed,
        AirWallSpeed,
        DashSpeed,
        JumpImpulse,
        CoolDownAttack,
        CoolDownDash,
        Damage
    }

    public static void BuildStatsList()
    {
        mAllStatsName = new List<string>();

        foreach (string name in Enum.GetNames(typeof(eStatsNames)))
        {
            mAllStatsName.Add(name);
        }
    }


    public Dictionary<string, float> mStatValues;

    public cStatsDescriptor()
    {
        mStatValues = new Dictionary<string, float>();

        if (mAllStatsName == null)
        {
            BuildStatsList();
        }

        foreach (string statName in mAllStatsName)
        {
            mStatValues[statName] = 0f;
        }
    }


    // ===================================
    // Manipulation
    // ===================================
    public void CombineByAddition( cStatsDescriptor rhs )
    {
        foreach (string statName in mAllStatsName)
        {
            mStatValues[statName] += rhs.mStatValues[statName];
        }
    }

    public void CombineByMultiplication( cStatsDescriptor rhs )
    {
        foreach (string statName in mAllStatsName)
        {
            mStatValues[statName] *= rhs.mStatValues[statName];
        }
    }

    public void ApplyOnEveryStat( Func<float, float> action )
    {
        foreach (string statName in mAllStatsName)
        {
            mStatValues[statName] = action( mStatValues[statName] );
        }
    }


    public void DEBUGLog()
    {
        foreach (string statName in mAllStatsName)
        {
            Debug.Log( statName + ": " + mStatValues[statName] );
        }

    }
}