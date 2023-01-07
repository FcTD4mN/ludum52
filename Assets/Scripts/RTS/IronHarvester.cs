using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronHarvester : ProductionBuilding
{
    public static float mRatePerSecond = 5f;
    public static float mGoldCost = 500f;


    override internal void Build()
    {
        GameManager.mResourceManager.mGoldF -= mGoldCost;
    }


    override public void GenerateResource()
    {
        GameManager.mResourceManager.mIronF += mRatePerSecond;
    }
}
