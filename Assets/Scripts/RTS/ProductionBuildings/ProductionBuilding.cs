using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class ProductionBuilding : MonoBehaviour
{
    internal cResourceDescriptor mResourceDescriptor;
    private bool mIsPaused = false;
    private bool mIsOutOfResources = false;
    private float mProdRatio = 1f;

    public GameObject mDiode;

    abstract public RTSManager.eBuildingList GetBuildingType();
    abstract public cResourceDescriptor GetResourceDescriptor();
    virtual public string GetDisplayName()
    {
        return GetBuildingType().ToString();
    }
    abstract public string GetDescription();




    // ===================================
    // Internal Setup
    // ===================================
    public void OnEnable()
    {
        Initialize();

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

        BuildBuilding();
    }


    public void OnDisable()
    {
        GameManager.mRTSManager.mAllProductionBuildings.Remove( this );
    }


    virtual internal void Initialize()
    {
        mResourceDescriptor = GetResourceDescriptor();
    }


    public bool IsPaused()
    {
        return mIsPaused;
    }

    virtual public void SetPause(bool state)
    {
        mIsPaused = state;
        UpdateDiode();
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
        mProdRatio = 1f;
        foreach( string resourceName in cResourceDescriptor.mAllResourceNames )
        {
            if( mResourceDescriptor.mInputRates[resourceName] == 0f ) {
                continue;
            }

            float available = GameManager.mResourceManager.GetRessource(resourceName);
            float deltaInputCost = mResourceDescriptor.mInputRates[resourceName] * deltaTime;
            if( available < deltaInputCost ) {
                float ratio = available / deltaInputCost;
                if( ratio < mProdRatio ) mProdRatio = ratio;
            }
        }

        // Used for stats modifiers, but eventually could apply fraction of stats as well
        SetOutOfResources( mProdRatio < 1 );
        UpdateDiode();

        if( mProdRatio == 0 ) { return; }

        foreach( string resourceName in cResourceDescriptor.mAllResourceNames )
        {
            // Remove what building consumes
            float deltaInputCost = mResourceDescriptor.mInputRates[resourceName] * deltaTime * mProdRatio;
            GameManager.mResourceManager.AddResource( resourceName, -deltaInputCost, false );

            // Add what building produces
            float deltaOutputCost = mResourceDescriptor.mOutputRates[resourceName] * deltaTime * mProdRatio;
            GameManager.mResourceManager.AddResource( resourceName, deltaOutputCost, false );
        }
    }


    public float GetProductionRatio()
    {
        return  mProdRatio;
    }




    public bool IsBuildable()
    {
        foreach (string resourceName in cResourceDescriptor.mAllResourceNames)
        {
            if (mResourceDescriptor.mBuildCosts[resourceName] > GameManager.mResourceManager.GetRessource(resourceName))
            {
                return false;
            }
        }

        return GameManager.mRTSManager.mUnlockedBuildings.Contains( GetBuildingType() );
    }


    // ===================================
    // Diode
    // ===================================
    public void AddDiode()
    {
        if( transform.parent == null ) return;

        GameObject prefab = Resources.Load<GameObject>("Prefabs/RTS/Diode");
        mDiode = Instantiate( prefab,
                                transform.position + new Vector3( transform.localScale.x/2.5f, transform.localScale.y/2.5f, -1 ),
                                Quaternion.Euler(0, 0, 0) );

        mDiode.transform.SetParent( gameObject.transform );
    }


    public void UpdateDiode()
    {
        if( mDiode == null ) return;

        SpriteRenderer sprite = mDiode.GetComponent<SpriteRenderer>();

        if( mIsPaused )
        {
            sprite.material.SetColor("_ColorA", new Color( 1, 0.5f, 0, 1 ));
            sprite.material.SetColor("_ColorB", new Color( 1, 0.6f, 0.1f, 1));
            return;
        }

        if( mProdRatio < 1 )
        {
            sprite.material.SetColor( "_ColorA", Color.red );
            sprite.material.SetColor( "_ColorB", Color.black );
            return;
        }

        sprite.material.SetColor("_ColorA", Color.green);
        sprite.material.SetColor("_ColorB", new Color(0.1f, 1, 0.1f, 1));
    }



    // ===================================
    // UI Stuff
    // ===================================
    public RTSManager.eBuildingErrors GetBuildingError()
    {
        if (!GameManager.mRTSManager.mUnlockedBuildings.Contains( GetBuildingType() ))
        {
            return RTSManager.eBuildingErrors.BlueprintRequired;
        }

        foreach (string resourceName in cResourceDescriptor.mAllResourceNames)
        {
            if (mResourceDescriptor.mBuildCosts[resourceName] > GameManager.mResourceManager.GetRessource(resourceName))
            {
                return RTSManager.eBuildingErrors.NotEnoughRessources;
            }
        }

        return RTSManager.eBuildingErrors.None;
    }




    public string GetUIDescription(bool isAllowed)
    {
        string name = GetBuildingType().ToString();
        string description = GetDescription();

        RTSManager.eBuildingErrors error = GetBuildingError();

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

        return ProductionBuilding.GetProductionBuildingUIDescription( name, description, errorMessage, mResourceDescriptor );
    }







    public static string GetProductionBuildingUIDescription(string name, string description, string errorMessage, cResourceDescriptor resourceDescriptor)
    {
        return resourceDescriptor.PrintCompleteDescription(name, description, errorMessage);
    }
}
