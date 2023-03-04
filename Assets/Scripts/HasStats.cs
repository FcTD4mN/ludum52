using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using static cStatsDescriptor;

public class HasStats : MonoBehaviour
{
    public cCompleteStats mStats = new cCompleteStats();


    // ===================================
    // Stats
    // ===================================
    public void SetBaseStats(cStatsDescriptor stats)
    {
        mStats.SetBaseStats( stats );
    }
    public void SetBaseStat(eStatsNames type, float value)
    {
        mStats.SetBaseStat( type, value );
    }

    // Adds values in stats into mStatsBonusAdd
    public void AddStatsAddition(cStatsDescriptor stats)
    {
        mStats.AddStatsAddition( stats );
    }

    // Adds values in stats into mStatsBonusMult
    public void AddStatsMultipliers(cStatsDescriptor stats)
    {
        mStats.AddStatsMultipliers( stats );
    }


    public float GetFinalStat( eStatsNames type )
    {
        return  mStats.GetFinalStat( type );
    }


    public float GetBaseStat(eStatsNames type)
    {
        return  mStats.GetBaseStat( type );
    }
}
