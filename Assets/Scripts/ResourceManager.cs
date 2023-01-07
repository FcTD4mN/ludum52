using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [HideInInspector] public float mGoldF = 5000;
    [HideInInspector] public float mIronF = 0;
    [HideInInspector] public float mArrowsF = 30;


    // [HideInInspector] public float mRateGold = 0f; // Just in case we wanna add gold harvesting
    // [HideInInspector] public float mRateIron = 0f;
    // [HideInInspector] public float mRateArrow = 0f;

    private float mTime = 0f;


    public List<ProductionBuilding> mAllProductionBuilding;
    // ===================================
    // Building
    // ===================================
    public void Initialize()
    {
        mAllProductionBuilding = new List<ProductionBuilding>();
    }


    // ===================================
    // Getters to get the proper int value
    // ===================================
    public int GetGold() {
        return  (int)mGoldF;
    }

    public int GetIron() {
        return  (int)mIronF;
    }

    public int GetArrows() {
        return  (int)mArrowsF;
    }


    // ===================================
    // Update
    // ===================================
    public void UpdateResources()
    {
        mTime += Time.deltaTime;

        if( mTime < 1.0 ) {
            return;
        }
        mTime = 1 - mTime;

        // Toutes les secondes
        foreach( ProductionBuilding building in mAllProductionBuilding )
        {
            building.GenerateResource();
        }
    }
}
