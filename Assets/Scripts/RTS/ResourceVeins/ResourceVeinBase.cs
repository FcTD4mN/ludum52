using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ResourceVeinBase : InitializableLate
{
    // public HarvestingBuilding mHarvester; //Needed?
    public cResourceDescriptor mResource;


    public void OnEnable()
    {
        if( !GameManager.IsLoaded() )
        {
            GameManager.AddToInitQueue( this );
            return;
        }

        Initialize();
    }


    public void OnDisable()
    {
        GameManager.mRTSManager.mAllResourceVeins.Remove(this);
    }

    public override void Initialize()
    {
        mResource = BuildResourceDescriptor();
        GameManager.mRTSManager.mAllResourceVeins.Add(this);
    }


    abstract internal cResourceDescriptor BuildResourceDescriptor();


    public void RegenerateResource( float deltaTime )
    {
        foreach( var resource in mResource.mInputRates )
        {
            mResource.mAvailable[resource.Key] += resource.Value * deltaTime;
        }
    }
}
