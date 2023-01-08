using System;
using System.Collections.Generic;
using UnityEngine;


using static cResourceDescriptor;

public class ResourceManager : MonoBehaviour
{
    [HideInInspector] public Dictionary<string, float> mResourcesAvailable;

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

        mResourcesAvailable[eResourceNames.Gold.ToString()] = 8000;
        mResourcesAvailable[eResourceNames.Iron.ToString()] = 2000;
        mResourcesAvailable[eResourceNames.Arrows.ToString()] = 30;
    }


    // ===================================
    // Getters to get the proper int value
    // ===================================
    public int GetGold() {
        return  (int)mResourcesAvailable[eResourceNames.Gold.ToString()];
    }

    public int GetIron() {
        return  (int)mResourcesAvailable[eResourceNames.Iron.ToString()];
    }

    public int GetFire() {
        return  (int)mResourcesAvailable[eResourceNames.Fire.ToString()];
    }

    public int GetArrows() {
        return  (int)mResourcesAvailable[eResourceNames.Arrows.ToString()];
    }

    public int GetBombs() {
        return  (int)mResourcesAvailable[eResourceNames.Bombs.ToString()];
    }


    // ===================================
    // Update
    // ===================================
    public void UpdateResources()
    {
        float deltaTime = Time.deltaTime;

        // Toutes les secondes
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


    public string PrintCompleteDescription( string name, string description )
    {
        string outputString = "";

        outputString = name + "\n" + description + "\n";

        outputString += "\n";

        outputString += "Build Cost: \n";
        bool atLeastOne = false;
        foreach( string resourceName in mAllResourceNames )
        {
            if( mBuildCosts[resourceName] == 0 ) { continue; }
            atLeastOne = true;

            outputString += "    " + resourceName + ": " + mBuildCosts[resourceName] + "\n";
        }
        if( !atLeastOne ) {
            outputString += "    None\n";
        }

        outputString += "\n";
        outputString += PrintProductionRates();

        return  outputString;
    }
}