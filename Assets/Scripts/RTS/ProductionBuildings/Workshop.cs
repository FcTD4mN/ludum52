using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Workshop : ProductionBuilding
{
    override public cResourceDescriptor GetNewResourceDescriptor()
    {
        cResourceDescriptor output = new cResourceDescriptor();

        output.mBuildCosts[cResourceDescriptor.eResourceNames.Gold] = 3000;
        output.mBuildCosts[cResourceDescriptor.eResourceNames.Iron] = 1000;

        output.mInputRates[cResourceDescriptor.eResourceNames.Iron] = 1;
        output.mInputRates[cResourceDescriptor.eResourceNames.Fire] = 1;
        output.mOutputRates[cResourceDescriptor.eResourceNames.FireArrows] = 1;

        return output;
    }

    public override RTSManager.eBuildingList GetBuildingType()
    {
        return RTSManager.eBuildingList.Workshop;
    }

    public override string GetDescription()
    {
        return "Builds fire arrows using arrows and fire";
    }
}
