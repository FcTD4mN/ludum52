using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : MonoBehaviour
{
    internal cResourceDescriptor mResourceDescriptor;

    public bool mIsPaused = false;


    // ===================================
    // Internal Setup
    // ===================================
    public void OnEnable()
    {
        GameManager.mRTSManager.mAllProductionBuildings.Add( this );
        Initialize();
        BuildBuilding();
    }


    public void OnDisable()
    {
        GameManager.mRTSManager.mAllProductionBuildings.Remove( this );
    }


    virtual internal void Initialize()
    {
        mResourceDescriptor = new cResourceDescriptor();
    }


    // ===================================
    // Resource Handling
    // ===================================
    public void BuildBuilding()
    {
        foreach( string resourceName in cResourceDescriptor.mAllResourceNames )
        {
            GameManager.mResourceManager.mResourcesAvailable[resourceName] -= mResourceDescriptor.mBuildCosts[resourceName];
        }
    }


    public void ProduceResource( float deltaTime )
    {
        if( mIsPaused ) { return; }

        // Checking enough input resources are available
        bool enoughResources = true;
        foreach( string resourceName in cResourceDescriptor.mAllResourceNames )
        {
            if( mResourceDescriptor.mInputRates[resourceName] == 0f ) {
                continue;
            }

            float deltaAvailable = GameManager.mResourceManager.mResourcesAvailable[resourceName] * deltaTime;
            float deltaInputCost = mResourceDescriptor.mInputRates[resourceName] * deltaTime;
            if( deltaAvailable < deltaInputCost ) {
                enoughResources = false;
                break;
            }
        }

        if( enoughResources )
        {
            foreach( string resourceName in cResourceDescriptor.mAllResourceNames )
            {
                // Remove what building consumes
                float deltaInputCost = mResourceDescriptor.mInputRates[resourceName] * deltaTime;
                GameManager.mResourceManager.mResourcesAvailable[resourceName] -= deltaInputCost;

                // Add what building produces
                float deltaOutputCost = mResourceDescriptor.mOutputRates[resourceName] * deltaTime;
                GameManager.mResourceManager.mResourcesAvailable[resourceName] += deltaOutputCost;
            }
        }
    }
}
