using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffBuildingDamage : BuffBuilding
{
    override public cResourceDescriptor GetNewResourceDescriptor()
    {
        cResourceDescriptor output = new cResourceDescriptor();

        output.mBuildCosts[cResourceDescriptor.eResourceNames.Gold.ToString()] = 500;
        output.mBuildCosts[cResourceDescriptor.eResourceNames.Iron.ToString()] = 500;
        output.mInputRates[cResourceDescriptor.eResourceNames.Iron.ToString()] = 2;
        output.mInputRates[cResourceDescriptor.eResourceNames.Fire.ToString()] = 1;
        return output;
    }

    override public cStatsDescriptor GetStatsDescriptor()
    {
        cStatsDescriptor output = new cStatsDescriptor();

        output.mStatValues[cStatsDescriptor.eStatsNames.Damage.ToString()] = 10;

        return output;
    }

    public override RTSManager.eBuildingList GetBuildingType()
    {
        return RTSManager.eBuildingList.BuffDamage;
    }

    public override string GetDisplayName()
    {
        return "Damage Buffer";
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
