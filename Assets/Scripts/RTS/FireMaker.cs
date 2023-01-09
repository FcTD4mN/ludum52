public class FireMaker : HarvestingBuilding
{
    public static cResourceDescriptor GetResourceDescriptor()
    {
        cResourceDescriptor output = new cResourceDescriptor();

        output.mBuildCosts[ cResourceDescriptor.eResourceNames.Gold.ToString() ] = 1000;
        output.mBuildCosts[ cResourceDescriptor.eResourceNames.Iron.ToString() ] = 50;

        output.mOutputRates[ cResourceDescriptor.eResourceNames.Fire.ToString() ] = 5;

        return  output;
    }


    public static bool IsBuildable()
    {
        cResourceDescriptor resourceDescriptor = GetResourceDescriptor();
        foreach( string resourceName in cResourceDescriptor.mAllResourceNames )
        {
            if( resourceDescriptor.mBuildCosts[resourceName] > GameManager.mResourceManager.GetRessource(resourceName) ) {
                return  false;
            }
        }

        return  GameManager.mRTSManager.mUnlockedBuildings.Contains(RTSManager.eBuildingList.FireMine);
    }


    public static RTSManager.eBuildingErrors GetBuildingError()
    {
        if (!GameManager.mRTSManager.mUnlockedBuildings.Contains(RTSManager.eBuildingList.FireMine))
        {
            return RTSManager.eBuildingErrors.BlueprintRequired;
        }

        cResourceDescriptor resourceDescriptor = GetResourceDescriptor();
        foreach (string resourceName in cResourceDescriptor.mAllResourceNames)
        {
            if (resourceDescriptor.mBuildCosts[resourceName] > GameManager.mResourceManager.GetRessource(resourceName))
            {
                return RTSManager.eBuildingErrors.NotEnoughRessources;
            }
        }

        return RTSManager.eBuildingErrors.None;
    }

    override internal void Initialize()
    {
        base.Initialize();
        mResourceDescriptor = GetResourceDescriptor();
    }
}
