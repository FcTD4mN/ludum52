public class Forge : ProductionBuilding
{
    public static cResourceDescriptor GetResourceDescriptor()
    {
        cResourceDescriptor output = new cResourceDescriptor();

        output.mBuildCosts[ cResourceDescriptor.eResourceNames.Gold.ToString() ] = 2000;
        output.mBuildCosts[ cResourceDescriptor.eResourceNames.Iron.ToString() ] = 100;
        output.mInputRates[ cResourceDescriptor.eResourceNames.Iron.ToString() ] = 2;
        output.mOutputRates[ cResourceDescriptor.eResourceNames.Arrows.ToString() ] = 1;

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

        return  GameManager.mRTSManager.mUnlockedBuildings.Contains(RTSManager.eBuildingList.Forge);
    }


    public static RTSManager.eBuildingErrors GetBuildingError()
    {
        if (!GameManager.mRTSManager.mUnlockedBuildings.Contains(RTSManager.eBuildingList.Forge))
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
        string name = "Forge";
        string description = "Builds arrows using iron";

        RTSManager.eBuildingErrors error = Forge.GetBuildingError();

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
