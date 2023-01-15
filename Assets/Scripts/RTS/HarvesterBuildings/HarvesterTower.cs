public class HarvesterTower : HarvestingBuilding
{
    override public cResourceDescriptor GetResourceDescriptor()
    {
        cResourceDescriptor output = new cResourceDescriptor();

        output.mOutputRates[cResourceDescriptor.eResourceNames.Gold.ToString()] = 10;
        output.mOutputRates[cResourceDescriptor.eResourceNames.Iron.ToString()] = 1;

        return output;
    }

    public override RTSManager.eBuildingList GetBuildingType()
    {
        return RTSManager.eBuildingList.Other;
    }

    public override string GetDescription()
    {
        return "This is the main Tower";
    }

    public new bool IsBuildable()
    {
        return false;
    }

    public new RTSManager.eBuildingErrors GetBuildingError()
    {
        return RTSManager.eBuildingErrors.None;
    }
}
