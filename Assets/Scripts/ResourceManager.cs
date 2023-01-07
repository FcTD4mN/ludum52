using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [HideInInspector] public float mGoldF;
    [HideInInspector] public float mIronF;
    [HideInInspector] public float mArrowsF;

    public List<ProductionBuilding> mAllProductionBuilding;
    // ===================================
    // Building
    // ===================================
    public void Initialize()
    {
        mAllProductionBuilding = new List<ProductionBuilding>();

        mGoldF = 4000;
        mIronF = 200;
        mArrowsF = 30;
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
        float deltaTime = Time.deltaTime;

        // Toutes les secondes
        foreach( ProductionBuilding building in mAllProductionBuilding )
        {
            building.ProduceResource( deltaTime );
        }
    }
}
