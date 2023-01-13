using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffBuildingSpeed : BuffBuilding
{
    override public cResourceDescriptor GetResourceDescriptor()
    {
        cResourceDescriptor output = new cResourceDescriptor();

        output.mBuildCosts[cResourceDescriptor.eResourceNames.Gold.ToString()] = 500;
        output.mBuildCosts[cResourceDescriptor.eResourceNames.Iron.ToString()] = 500;
        output.mInputRates[cResourceDescriptor.eResourceNames.Iron.ToString()] = 2;
        output.mInputRates[cResourceDescriptor.eResourceNames.Fire.ToString()] = 4;
        return output;
    }

    override public cStatsDescriptor GetStatsDescriptor()
    {
        cStatsDescriptor output = new cStatsDescriptor();

        output.mStatValues[cStatsDescriptor.eStatsNames.RunSpeed.ToString()] = 1;
        output.mStatValues[cStatsDescriptor.eStatsNames.AirWalkSpeed.ToString()] = 1;
        output.mStatValues[cStatsDescriptor.eStatsNames.DashSpeed.ToString()] = 1;
        output.mStatValues[cStatsDescriptor.eStatsNames.AirWallSpeed.ToString()] = 1;

        return output;
    }

    public override RTSManager.eBuildingList GetBuildingType()
    {
        return RTSManager.eBuildingList.BuffSpeed;
    }

    public override string GetDisplayName()
    {
        return "Speed Buffer";
    }

    public override string GetDescription()
    {
        return "Gives buff in exchange of resources";
    }

    override internal void Initialize()
    {
        base.Initialize();
        mAdds = true;
    }
}
