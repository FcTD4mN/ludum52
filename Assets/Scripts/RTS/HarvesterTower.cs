public class HarvesterTower : HarvestingBuilding
{
    public static cResourceDescriptor GetResourceDescriptor()
    {
        cResourceDescriptor output = new cResourceDescriptor();

        output.mOutputRates[cResourceDescriptor.eResourceNames.Gold.ToString()] = 10;
        output.mOutputRates[cResourceDescriptor.eResourceNames.Iron.ToString()] = 1;
        return output;
    }


    public static bool IsBuildable()
    {
        return false;
    }


    public static RTSManager.eBuildingErrors GetBuildingError()
    {
        return RTSManager.eBuildingErrors.None;
    }


    override internal void Initialize()
    {
        base.Initialize();
        mResourceDescriptor = GetResourceDescriptor();
    }
}
