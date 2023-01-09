using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


using static cResourceDescriptor;

public class ResourceManager : MonoBehaviour
{
    [HideInInspector] private Dictionary<string, float> mResourcesAvailable;

    private Coroutine mCurrentlyRunningAnimation;
    private float mAnimationFinalValue;
    private string mAnimationResourceName;

    // ===================================
    // Building
    // ===================================
    public void Initialize()
    {
        mResourcesAvailable = new Dictionary<string, float>();
        foreach( string resourceName in mAllResourceNames )
        {
            mResourcesAvailable[resourceName] = 0f;
        }

        mResourcesAvailable[eResourceNames.Gold.ToString()] = 1900;
        mResourcesAvailable[eResourceNames.Iron.ToString()] = 800;
        mResourcesAvailable[eResourceNames.Arrows.ToString()] = 30;
    }


    // ===================================
    // Getters to get the proper int value
    // ===================================
    public float GetRessource( eResourceNames type )
    {
        return GetRessource( type.ToString() );
    }
    public float GetRessource( string name )
    {
        return mResourcesAvailable[name];
    }

    public void AddResource( string name, float deltaValue, bool animated )
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

    IEnumerator Animation( string name, float newValue, float time )
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
    public void UpdateResources()
    {
        float deltaTime = Time.deltaTime;
        foreach( ProductionBuilding building in GameManager.mRTSManager.mAllProductionBuildings )
        {
            building.ProduceResource( deltaTime );
        }
    }
}





public class cResourceDescriptor
{
    public static List<string> mAllResourceNames;
    public enum eResourceNames {
        Gold,
        Iron,
        Fire,
        Arrows,
        FireArrows,
        Bombs
    }

    public static void BuildResourceList()
    {
        mAllResourceNames = new List<string>();

        foreach (string name in Enum.GetNames(typeof(eResourceNames)))
        {
            mAllResourceNames.Add( name );
        }
    }


    public Dictionary<string, float> mBuildCosts;
    public Dictionary<string, float> mInputRates;
    public Dictionary<string, float> mOutputRates;

    public cResourceDescriptor()
    {
        mBuildCosts = new Dictionary<string, float>();
        mInputRates = new Dictionary<string, float>();
        mOutputRates = new Dictionary<string, float>();

        if( mAllResourceNames == null )
        {
            BuildResourceList();
        }

        foreach( string resourceName in mAllResourceNames )
        {
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
        foreach( string resourceName in mAllResourceNames )
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
        foreach( string resourceName in mAllResourceNames )
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
        foreach( string resourceName in mAllResourceNames )
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