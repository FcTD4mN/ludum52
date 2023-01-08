using System;
using System.Collections.Generic;

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

        foreach (string statName in mAllStatsName)
        {
            mStatValues[statName] = 0f;
        }
    }
}