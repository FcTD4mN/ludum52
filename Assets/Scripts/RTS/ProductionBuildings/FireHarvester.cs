public class FireHarvester : HarvestingBuilding
{
    override public cResourceDescriptor GetResourceDescriptor()
    {
        cResourceDescriptor output = new cResourceDescriptor();

        output.mBuildCosts[cResourceDescriptor.eResourceNames.Gold.ToString()] = 1000;
        output.mBuildCosts[cResourceDescriptor.eResourceNames.Iron.ToString()] = 50;

        output.mOutputRates[cResourceDescriptor.eResourceNames.Fire.ToString()] = 5;

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
}
