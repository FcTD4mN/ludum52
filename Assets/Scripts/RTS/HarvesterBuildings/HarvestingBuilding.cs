using System;
using System.Collections.Generic;
using UnityEngine;

using static cResourceDescriptor;

public abstract class HarvestingBuilding : ProductionBuilding
{
    public Receiver mReceiver;
    public ResourceVeinBase mResourceVein;

    abstract public string GetAssociatedReceiver();

    // ===================================
    // Internal Setup
    // ===================================
    public new void OnEnable()
    {
        GameManager.mRTSManager.mAllHarvesters.Add( this );
        base.OnEnable();
    }


    public new void OnDisable()
    {
        GameManager.mRTSManager.mAllHarvesters.Remove( this );
        base.OnDisable();
    }


    virtual public bool IsConnected()
    {
        return  mReceiver != null;
    }


    override public bool IsPaused()
    {
        return  base.IsPaused() || !IsConnected();
    }

    override public void SetPause(bool state)
    {
        base.SetPause( state );

        var associatedReceiver = GetAssociatedReceiver();
        if( associatedReceiver == "" ) return;

        if (state)
        {
            GameManager.mRTSManager.DestroyBuilding(mReceiver.gameObject);
        }
        else
        {
            GameManager.mRTSManager.mMainTower.BuildReceiver( associatedReceiver, gameObject);
        }
    }


    // "Override" this method to add IsConnected check, and |TODO: extract resource from vein instead of resource
    override public void ProduceResource(float deltaTime)
    {
        if (IsPaused()) { return; }

        // Checking enough input resources are available
        mProdRatio = 1f;

        // Testing here, so that you can have building like tower, that doesn't pump from anything, and just produces free resources
        if( mResourceVein != null )
        {
            foreach (eResourceNames resourceName in Enum.GetValues(typeof(eResourceNames)))
            {
                if (mResourceDescriptor.mOutputRates[resourceName] == 0f) continue;

                // README: This will only make pumping from vein possible, not vein + tower at the same time
                // It uses outputRates to pump from vein available
                // Could then use inputRates as inputs that are pumped from base
                float available = mResourceVein.mResource.mAvailable[resourceName];
                float deltaInputCost = mResourceDescriptor.mOutputRates[resourceName] * deltaTime;
                if (available < deltaInputCost)
                {
                    float ratio = available / deltaInputCost;
                    if (ratio < mProdRatio) mProdRatio = ratio;
                }
            }
        }

        // Used for stats modifiers, but eventually could apply fraction of stats as well
        UpdateDiode();

        if (mProdRatio == 0) { return; }

        foreach (eResourceNames resourceName in Enum.GetValues(typeof(eResourceNames)))
        {
            if (mResourceVein != null)
            {
                // Remove what building consumes
                float deltaInputCost = mResourceDescriptor.mOutputRates[resourceName] * deltaTime * mProdRatio;
                mResourceVein.mResource.mAvailable[resourceName] -= deltaInputCost;
            }

            // Add what building produces
            float deltaOutputCost = mResourceDescriptor.mOutputRates[resourceName] * deltaTime * mProdRatio;
            GameManager.mResourceManager.AddResource(resourceName, deltaOutputCost, false);
        }
    }
}
