using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronHarvester : MonoBehaviour
{
    public float mRatePerSecond = 5f;
    public float mGoldCost = 500f;


    void OnEnable()
    {
        GameManager.mResourceManager.mGoldF -= mGoldCost;
        GameManager.mResourceManager.mRateIron += mRatePerSecond;
    }

    void OnDisable()
    {
        GameManager.mResourceManager.mRateIron -= mRatePerSecond;
    }
}
