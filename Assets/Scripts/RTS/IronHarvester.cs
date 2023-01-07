using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronHarvester : ProductionBuilding
{
    public static float mRatePerSecond = 5f;
    public static float mGoldCost = 500f;


    override internal void Build()
    {
        mBuildCostGold = mGoldCost;
        base.Build();
    }


    override public void ProduceResource( float deltaTime )
    {
        GameManager.mResourceManager.mIronF += mRatePerSecond * deltaTime;
    }
}
