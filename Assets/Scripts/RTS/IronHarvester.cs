using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronHarvester : HarvestingBuilding
{
    public static cResourceDescriptor GetResourceDescriptor()
    {
        cResourceDescriptor output = new cResourceDescriptor();

        output.mBuildCosts[ cResourceDescriptor.eResourceNames.Gold.ToString() ] = 500;
        output.mOutputRates[ cResourceDescriptor.eResourceNames.Iron.ToString() ] = 5;
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

        return  true;
    }


    override internal void Initialize()
    {
        base.Initialize();
        mResourceDescriptor = GetResourceDescriptor();
    }
}
