using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Forge : ProductionBuilding
{
    public static float mGoldCost = 2000f;
    public static float mIronCost = 2f;
    public static float mRatePerSecond = 1f;

    override internal void Build()
    {
        GameManager.mResourceManager.mGoldF -= mGoldCost;
    }


    override public void GenerateResource()
    {
        if( GameManager.mResourceManager.mIronF > mIronCost )
        {
            GameManager.mResourceManager.mArrowsF += mRatePerSecond;
            GameManager.mResourceManager.mIronF -= mIronCost;
        }
    }
}
