using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using static cResourceDescriptor;

public class ResourceManager : MonoBehaviour
{
    [HideInInspector] private Dictionary<eResourceNames, float> mResourcesAvailable;

    private Coroutine mCurrentlyRunningAnimation;
    private float mAnimationFinalValue;
    private eResourceNames mAnimationResourceName;

    // ===================================
    // Building
    // ===================================
    public void Initialize()
    {
        mResourcesAvailable = new Dictionary<eResourceNames, float>();
        foreach( eResourceNames resourceName in Enum.GetValues(typeof(eResourceNames)) )
        {
            mResourcesAvailable[resourceName] = 0f;
        }

        mResourcesAvailable[eResourceNames.Gold] = 5000;
        mResourcesAvailable[eResourceNames.Iron] = 800;
        mResourcesAvailable[eResourceNames.Arrows] = 30;
    }


    // ===================================
    // Getters to get the proper int value
    // ===================================
    public float GetRessource( eResourceNames name )
    {
        return mResourcesAvailable[name];
    }

    public void AddResource( eResourceNames name, float deltaValue, bool animated )
    {
        if (deltaValue == 0)
        {
            return;
        }

        if (mCurrentlyRunningAnimation != null && name == mAnimationResourceName)
        {
            StopCoroutine(mCurrentlyRunningAnimation);
            mCurrentlyRunningAnimation = null;
            mResourcesAvailable[mAnimationResourceName] = mAnimationFinalValue;
        }

        if (animated)
        {
            mAnimationFinalValue = mResourcesAvailable[name] + deltaValue;
            mAnimationResourceName = name;
            mCurrentlyRunningAnimation = StartCoroutine(Animation(name, mAnimationFinalValue, 1f));
        }
        else
        {
            mResourcesAvailable[name] += deltaValue;
        }
    }

    IEnumerator Animation( eResourceNames name, float newValue, float time )
    {
        float originalValue = mResourcesAvailable[name];
        float deltaValue = newValue - originalValue;

        float timer = time;
        while( timer > 0 )
        {
            float deltaTime = Time.deltaTime;
            timer -= deltaTime;
            mResourcesAvailable[name] += deltaValue * deltaTime;

            yield return null;
        }

        mResourcesAvailable[name] = newValue;
        mCurrentlyRunningAnimation = null;
    }

    // ===================================
    // Update
    // ===================================
    public void UpdateResources( float fixedDeltaTime )
    {
        foreach (ResourceVeinBase vein in GameManager.mRTSManager.mAllResourceVeins)
        {
            vein.RegenerateResource(fixedDeltaTime);
        }

        foreach( ProductionBuilding building in GameManager.mRTSManager.mAllProductionBuildings )
        {
            building.ProduceResource( fixedDeltaTime );
        }
    }
}





public class cResourceDescriptor
{
    // public static List<string> mAllResourceNames;
    public enum eResourceNames {
        Gold,
        Iron,
        Fire,
        Arrows,
        FireArrows,
        Bombs
    }
    public enum eResourceType
    {
        kBuildCost,
        kInput,
        kOutput
    }


    public Dictionary<eResourceNames, float> mAvailable;
    public Dictionary<eResourceNames, float> mBuildCosts;
    public Dictionary<eResourceNames, float> mInputRates;
    public Dictionary<eResourceNames, float> mOutputRates;

    public cResourceDescriptor()
    {
        mAvailable = new Dictionary<eResourceNames, float>();
        mBuildCosts = new Dictionary<eResourceNames, float>();
        mInputRates = new Dictionary<eResourceNames, float>();
        mOutputRates = new Dictionary<eResourceNames, float>();

        foreach( eResourceNames resourceName in Enum.GetValues(typeof(eResourceNames)) )
        {
            mAvailable[resourceName] = 0f;
            mBuildCosts[resourceName] = 0f;
            mInputRates[resourceName] = 0f;
            mOutputRates[resourceName] = 0f;
        }
    }


    public string PrintProductionRates()
    {
        string outputString = "";
        bool atLeastOne = false;

        // Costs first
        outputString += "Inputs: \n";
        foreach( eResourceNames resourceName in Enum.GetValues(typeof(eResourceNames)) )
        {
            if( mInputRates[resourceName] == 0 ) { continue; }
            atLeastOne = true;

            outputString += "    " + resourceName + ": " + mInputRates[resourceName] + "\n";
        }
        if( !atLeastOne ) {
            outputString += "    None\n";
        }
        atLeastOne = false;


        // Then outputs
        outputString += "Outputs: \n";
        foreach( eResourceNames resourceName in Enum.GetValues(typeof(eResourceNames)) )
        {
            if( mOutputRates[resourceName] == 0 ) { continue; }
            atLeastOne = true;

            outputString += "    " + resourceName + ": " + mOutputRates[resourceName] + "\n";
        }
        if( !atLeastOne ) {
            outputString += "    None\n";
        }

        return  outputString;
    }


    public string PrintCompleteDescription( string name, string description, string error )
    {
        string outputString = "";
        string redColor = "<color=#EB0800>";

        outputString = name + "\n" + description + "\n";

        outputString += "\n";
        if( error != "" )
        {
            outputString += redColor + "Issue: " + error + "</color>";
            outputString += "\n";
            outputString += "\n";
            outputString += "\n";
        }

        outputString += "Build Cost: \n";
        bool atLeastOne = false;
        foreach( eResourceNames resourceName in Enum.GetValues(typeof(eResourceNames)) )
        {
            if( mBuildCosts[resourceName] == 0 ) { continue; }
            atLeastOne = true;

            bool hasEnough = mBuildCosts[resourceName] <= GameManager.mResourceManager.GetRessource(resourceName);
            string colorBalise = hasEnough ? "" : redColor;
            string colorBaliseEnd = hasEnough ? "" : "</color>";

            outputString += "    " + colorBalise + resourceName + ": " + mBuildCosts[resourceName] + colorBaliseEnd + "\n";
        }
        if( !atLeastOne ) {
            outputString += "    None\n";
        }

        outputString += "\n";
        outputString += PrintProductionRates();

        return  outputString;
    }
}