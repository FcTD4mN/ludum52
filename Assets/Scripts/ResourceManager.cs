using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ResourceManager : MonoBehaviour
{
    private float mGold = 5000;
    private float mIron = 0;
    private float mArrows = 30;


    [HideInInspector] public float mRateGold = 0f; // Just in case we wanna add gold harvesting
    [HideInInspector] public float mRateIron = 0f;
    [HideInInspector] public float mRateArrow = 0f;


    // ===================================
    // Getters to get the proper int value
    // ===================================
    public int GetGold() {
        return  (int)mGold;
    }

    public int GetIron() {
        return  (int)mIron;
    }

    public int GetArrows() {
        return  (int)mArrows;
    }


    // ===================================
    // Update
    // ===================================
    public void UpdateResources()
    {
        float deltaTime = Time.deltaTime;
        mGold += mRateGold * deltaTime;
        mIron += mRateIron * deltaTime;
        mArrows += mRateArrow * deltaTime;
    }
}
