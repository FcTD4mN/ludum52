using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forge : ProductionBuilding
{
    public static float mGoldCost = 2000f;
    public static float mIronCost = 100f;

    public static float mIronCostRate = 2f;
    public static float mRatePerSecond = 1f;

    override internal void Build()
    {
        mBuildCostGold = mGoldCost;
        mBuildCostIron = mIronCost;
        base.Build();
    }


    override public void ProduceResource( float deltaTime )
    {
        float deltaIronAvailable = GameManager.mResourceManager.mIronF * deltaTime;
        float deltaIronCost = mIronCostRate * deltaTime;

        if( deltaIronAvailable >= deltaIronCost )
        {
            GameManager.mResourceManager.mArrowsF += mRatePerSecond * deltaTime;
            GameManager.mResourceManager.mIronF -= deltaIronCost;
        }
    }
}
