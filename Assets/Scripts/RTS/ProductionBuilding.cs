using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : MonoBehaviour
{

    internal float mBuildCostGold = 0f;
    internal float mBuildCostIron = 0f;


    public void OnEnable()
    {
        GameManager.mResourceManager.mAllProductionBuilding.Add( this );
        Build();
    }


    void OnDisable()
    {
        GameManager.mResourceManager.mAllProductionBuilding.Remove( this );
    }


    virtual internal void Build()
    {
        GameManager.mResourceManager.mGoldF -= mBuildCostGold;
        GameManager.mResourceManager.mIronF -= mBuildCostIron;
    }

    virtual public void ProduceResource( float deltaTime )
    {
        // Implement
    }
}
