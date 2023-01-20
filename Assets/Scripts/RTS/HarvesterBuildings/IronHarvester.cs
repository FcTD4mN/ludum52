using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronHarvester : HarvestingBuilding
{
    override public cResourceDescriptor GetNewResourceDescriptor()
    {
        cResourceDescriptor output = new cResourceDescriptor();

        output.mBuildCosts[cResourceDescriptor.eResourceNames.Gold.ToString()] = 500;
        output.mOutputRates[cResourceDescriptor.eResourceNames.Iron.ToString()] = 5;

        return output;
    }

    public override RTSManager.eBuildingList GetBuildingType()
    {
        return RTSManager.eBuildingList.IronHarvester;
    }

    public override string GetDescription()
    {
        return "Harvests Iron";
    }

    override public string GetAssociatedReceiver()
    {
        return "IronReceiver";
    }
}
