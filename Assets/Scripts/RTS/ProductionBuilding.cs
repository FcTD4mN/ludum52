using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : MonoBehaviour
{
    internal cResourceDescriptor mResourceDescriptor;
    private bool mIsPaused = false;
    private bool mIsOutOfResources = false;


    // ===================================
    // Internal Setup
    // ===================================
    public void OnEnable()
    {
        GameManager.mRTSManager.mAllProductionBuildings.Add( this );

        // Sorts so that prod building that are not buffs are first in the list, so resources are first used by regular buildings, then by buffers
        GameManager.mRTSManager.mAllProductionBuildings.Sort( delegate( ProductionBuilding lhs, ProductionBuilding rhs )
        {
            bool lhsIsBuff = lhs.gameObject.GetComponent<BuffBuilding>() != null;
            bool lhsIsHarv = lhs.gameObject.GetComponent<HarvestingBuilding>() != null;

            bool rhsIsBuff = rhs.gameObject.GetComponent<BuffBuilding>() != null;
            bool rhsIsHarv = rhs.gameObject.GetComponent<HarvestingBuilding>() != null;

            // This puts buff building last
            if( lhsIsBuff && rhsIsBuff ) { return  0; }
            if( lhsIsBuff && !rhsIsBuff ) { return  1; }
            if( !lhsIsBuff && rhsIsBuff ) { return  -1; }

            // This puts harvesters first
            if( lhsIsHarv && rhsIsHarv ) { return  0; }
            if( lhsIsHarv ) { return  -1; } // If lhs is harvester, it's >
            if( !lhsIsHarv ) { return  1; } // If lhs is not harvester, it's <

            return  0;

        });

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


    public bool IsPaused()
    {
        return mIsPaused;
    }

    virtual public void SetPause(bool state)
    {
        mIsPaused = state;
    }


    public bool IsOutOfResources()
    {
        return  mIsOutOfResources;
    }


    virtual internal void SetOutOfResources( bool state )
    {
        mIsOutOfResources = state;
    }


    // ===================================
    // Resource Handling
    // ===================================
    public void BuildBuilding()
    {
        foreach( string resourceName in cResourceDescriptor.mAllResourceNames )
        {
            GameManager.mResourceManager.AddResource( resourceName, -mResourceDescriptor.mBuildCosts[resourceName], false );

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

            float deltaAvailable = GameManager.mResourceManager.GetRessource(resourceName) * deltaTime;
            float deltaInputCost = mResourceDescriptor.mInputRates[resourceName] * deltaTime;
            if( deltaAvailable < deltaInputCost ) {
                enoughResources = false;
                break;
            }
        }

        SetOutOfResources( !enoughResources );

        if( enoughResources )
        {
            foreach( string resourceName in cResourceDescriptor.mAllResourceNames )
            {
                // Remove what building consumes
                float deltaInputCost = mResourceDescriptor.mInputRates[resourceName] * deltaTime;
                GameManager.mResourceManager.AddResource( resourceName, -deltaInputCost, false );

                // Add what building produces
                float deltaOutputCost = mResourceDescriptor.mOutputRates[resourceName] * deltaTime;
                GameManager.mResourceManager.AddResource( resourceName, deltaOutputCost, false );
            }
        }
    }





    public static string GetProductionBuildingUIDescription(string name, string description, string errorMessage, cResourceDescriptor resourceDescriptor)
    {
        return resourceDescriptor.PrintCompleteDescription(name, description, errorMessage);
    }
}
