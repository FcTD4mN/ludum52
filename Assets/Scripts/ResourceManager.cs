using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    [HideInInspector] public float mGoldF = 5000;
    [HideInInspector] public float mIronF = 0;
    [HideInInspector] public float mArrowsF = 30;


    [HideInInspector] public float mRateGold = 0f; // Just in case we wanna add gold harvesting
    [HideInInspector] public float mRateIron = 0f;
    [HideInInspector] public float mRateArrow = 0f;


    // ===================================
    // Building
    // ===================================
    public void Initialize()
    {

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
        mGoldF += mRateGold * deltaTime;
        mIronF += mRateIron * deltaTime;
        mArrowsF += mRateArrow * deltaTime;
    }
}
