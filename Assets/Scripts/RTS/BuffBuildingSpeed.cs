using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffBuildingSpeed : BuffBuilding
{
    public static cResourceDescriptor GetResourceDescriptor()
    {
        cResourceDescriptor output = new cResourceDescriptor();

        output.mBuildCosts[cResourceDescriptor.eResourceNames.Gold.ToString()] = 500;
        output.mBuildCosts[cResourceDescriptor.eResourceNames.Iron.ToString()] = 500;
        output.mInputRates[cResourceDescriptor.eResourceNames.Iron.ToString()] = 2;
        output.mInputRates[cResourceDescriptor.eResourceNames.Fire.ToString()] = 4;
        return output;
    }

    public static cStatsDescriptor GetStatsDescriptor()
    {
        cStatsDescriptor output = new cStatsDescriptor();

        output.mStatValues[cStatsDescriptor.eStatsNames.RunSpeed.ToString()] = 1;
        output.mStatValues[cStatsDescriptor.eStatsNames.AirWalkSpeed.ToString()] = 1;
        output.mStatValues[cStatsDescriptor.eStatsNames.DashSpeed.ToString()] = 1;
        output.mStatValues[cStatsDescriptor.eStatsNames.AirWallSpeed.ToString()] = 1;

        return output;
    }











    public static bool IsBuildable()
    {
        cResourceDescriptor resourceDescriptor = GetResourceDescriptor();
        foreach (string resourceName in cResourceDescriptor.mAllResourceNames)
        {
            if (resourceDescriptor.mBuildCosts[resourceName] > GameManager.mResourceManager.GetRessource(resourceName))
            {
                return false;
            }
        }

        return GameManager.mRTSManager.mUnlockedBuildings.Contains(RTSManager.eBuildingList.BuffSpeed);
    }


    public static RTSManager.eBuildingErrors GetBuildingError()
    {
        cResourceDescriptor resourceDescriptor = GetResourceDescriptor();
        foreach (string resourceName in cResourceDescriptor.mAllResourceNames)
        {
            if (resourceDescriptor.mBuildCosts[resourceName] > GameManager.mResourceManager.GetRessource(resourceName))
            {
                return RTSManager.eBuildingErrors.NotEnoughRessources;
            }
        }

        if (!GameManager.mRTSManager.mUnlockedBuildings.Contains(RTSManager.eBuildingList.BuffSpeed))
        {
            return RTSManager.eBuildingErrors.BlueprintRequired;
        }

        return RTSManager.eBuildingErrors.None;
    }


    public static string GetUIDescription(bool isAllowed)
    {
        string name = "Speed buff";
        string description = "Gives buff in exchange of resources";

        RTSManager.eBuildingErrors error = BuffBuildingSpeed.GetBuildingError();

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

        return BuffBuilding.GetBuffBuildingUIDescription(name, description, errorMessage, GetResourceDescriptor(), GetStatsDescriptor());
    }


    override internal void Initialize()
    {
        base.Initialize();
        mResourceDescriptor = GetResourceDescriptor();
        mStatsModifiers = GetStatsDescriptor();
        mAdds = true;
    }
}
