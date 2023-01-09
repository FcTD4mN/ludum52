public class BombFactory : ProductionBuilding
{
    public static cResourceDescriptor GetResourceDescriptor()
    {
        cResourceDescriptor output = new cResourceDescriptor();

        output.mBuildCosts[ cResourceDescriptor.eResourceNames.Gold.ToString() ] = 3000;
        output.mBuildCosts[ cResourceDescriptor.eResourceNames.Iron.ToString() ] = 500;

        output.mInputRates[ cResourceDescriptor.eResourceNames.Iron.ToString() ] = 2;
        output.mInputRates[ cResourceDescriptor.eResourceNames.Fire.ToString() ] = 2;
        output.mOutputRates[ cResourceDescriptor.eResourceNames.Bombs.ToString() ] = 1;

        return  output;
    }


    public static bool IsBuildable()
    {
        cResourceDescriptor resourceDescriptor = GetResourceDescriptor();
        foreach( string resourceName in cResourceDescriptor.mAllResourceNames )
        {
            if( resourceDescriptor.mBuildCosts[resourceName] > GameManager.mResourceManager.GetRessource( resourceName ) ) {
                return  false;
            }
        }

        return  GameManager.mRTSManager.mUnlockedBuildings.Contains(RTSManager.eBuildingList.BombFactory);
    }


    public static RTSManager.eBuildingErrors GetBuildingError()
    {
        if (!GameManager.mRTSManager.mUnlockedBuildings.Contains(RTSManager.eBuildingList.BombFactory))
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


    public static string GetUIDescription(bool isAllowed)
    {
        string name = "Bomb Factory";
        string description = "Builds bombs using iron and fire";

        RTSManager.eBuildingErrors error = BombFactory.GetBuildingError();

        string errorMessage = "";
        switch (error)
        {
            case RTSManager.eBuildingErrors.BlueprintRequired:
                errorMessage = "Blueprint required";
                break;
            case RTSManager.eBuildingErrors.NotEnoughRessources:
                errorMessage = "Not enough resources";
                break;
            case RTSManager.eBuildingErrors.None:
                errorMessage = isAllowed ? "" : "Can't build that type of building here";
                break;
        }

        return ProductionBuilding.GetProductionBuildingUIDescription(name, description, errorMessage, GetResourceDescriptor());
    }

    override internal void Initialize()
    {
        base.Initialize();
        mResourceDescriptor = GetResourceDescriptor();
    }
}
