public class FireHarvester : HarvestingBuilding
{
    override public cResourceDescriptor GetNewResourceDescriptor()
    {
        cResourceDescriptor output = new cResourceDescriptor();

        output.mBuildCosts[cResourceDescriptor.eResourceNames.Gold] = 1000;
        output.mBuildCosts[cResourceDescriptor.eResourceNames.Iron] = 50;

        output.mOutputRates[cResourceDescriptor.eResourceNames.Fire] = 5;

        return output;
    }

    public override RTSManager.eBuildingList GetBuildingType()
    {
        return RTSManager.eBuildingList.FireHarvester;
    }

    public override string GetDescription()
    {
        return "Harvests fire";
    }

    override public string GetAssociatedReceiver()
    {
        return  "FireReceiver";
    }
}
