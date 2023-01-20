using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            GameManager.mRTSManager.BuildReceiver( associatedReceiver, gameObject);
        }
    }


    // "Override" this method to add IsConnected check, and |TODO: extract resource from vein instead of resource
    override public void ProduceResource(float deltaTime)
    {
        if (IsPaused()) { return; }

        // Checking enough input resources are available
            mProdRatio = 1f;

        // Replace this by the vein's resource
        // foreach (string resourceName in cResourceDescriptor.mAllResourceNames)
        // {
        //     if (mResourceDescriptor.mInputRates[resourceName] == 0f)
        //     {
        //         continue;
        //     }

        //     float available = GameManager.mResourceManager.GetRessource(resourceName);
        //     float deltaInputCost = mResourceDescriptor.mInputRates[resourceName] * deltaTime;
        //     if (available < deltaInputCost)
        //     {
        //         float ratio = available / deltaInputCost;
        //         if (ratio < mProdRatio) mProdRatio = ratio;
        //     }
        // }

        // Used for stats modifiers, but eventually could apply fraction of stats as well
        UpdateDiode();

        if (mProdRatio == 0) { return; }

        foreach (string resourceName in cResourceDescriptor.mAllResourceNames)
        {
            // Remove what building consumes
            float deltaInputCost = mResourceDescriptor.mInputRates[resourceName] * deltaTime * mProdRatio;
            GameManager.mResourceManager.AddResource(resourceName, -deltaInputCost, false);

            // Add what building produces
            float deltaOutputCost = mResourceDescriptor.mOutputRates[resourceName] * deltaTime * mProdRatio;
            GameManager.mResourceManager.AddResource(resourceName, deltaOutputCost, false);
        }
    }
}
