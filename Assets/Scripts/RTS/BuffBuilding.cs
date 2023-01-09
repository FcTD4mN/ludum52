using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BuffBuilding : ProductionBuilding
{
    internal cStatsDescriptor mStatsModifiers;

    internal bool mAdds = true;
    private bool mStatsAreApplied = false;


    // ===================================
    // Internal Setup
    // ===================================
    public new void OnEnable()
    {
        base.OnEnable();
        GameManager.mRTSManager.mAllBuffBuildings.Add(this);
        ApplyStats();
    }


    public new void OnDisable()
    {
        base.OnDisable();
        GameManager.mRTSManager.mAllBuffBuildings.Remove(this);
        RetractStats();
    }


    override internal void Initialize()
    {
        base.Initialize();
        mResourceDescriptor = new cResourceDescriptor();
    }

    public override void SetPause( bool state )
    {
        base.SetPause( state );
        if( state )
        {
            RetractStats();
        }
        else
        {
            ApplyStats();
        }
    }

    internal void ApplyStats()
    {
        if( mStatsAreApplied ) { return; }
        mStatsAreApplied = true;

        if (GameManager.mInstance.playerCtrler == null) { return; }
        HasStats playerStats = GameManager.mInstance.playerCtrler.GetComponent<HasStats>();

        if( mAdds ) {
            playerStats.AddStatsAddition(mStatsModifiers);
        }
        else{
            playerStats.AddStatsMultipliers(mStatsModifiers);
        }
    }
    internal void RetractStats()
    {
        if (!mStatsAreApplied) { return; }
        mStatsAreApplied = false;

        if( GameManager.mInstance.playerCtrler == null ) { return; }
        HasStats playerStats = GameManager.mInstance.playerCtrler.GetComponent<HasStats>();

        cStatsDescriptor inverse = new cStatsDescriptor(mStatsModifiers);
        inverse.ApplyOnEveryStat(val => -val);

        if (mAdds) {
            playerStats.AddStatsAddition(inverse);
        }
        else {
            playerStats.AddStatsMultipliers(inverse);
        }
    }

    override internal void SetOutOfResources( bool state )
    {
        base.SetOutOfResources( state );

        if( IsOutOfResources() )
        {
            RetractStats();
        }
        else
        {
            ApplyStats();
        }
    }






    public static string GetBuffBuildingUIDescription( string name,
                                                        string description,
                                                        string errorMessage,
                                                        cResourceDescriptor resourceDescriptor,
                                                        cStatsDescriptor statsDescriptor )
    {
        string outputString = "";

        outputString = name + "\n" + description + "\n";

        outputString += "\n";
        if (errorMessage != "")
        {
            outputString += "    Issue: " + errorMessage;
            outputString += "\n";
            outputString += "\n";
            outputString += "\n";
        }

        outputString += "Build Cost: \n";
        bool atLeastOne = false;
        foreach (string resourceName in cResourceDescriptor.mAllResourceNames)
        {
            if (resourceDescriptor.mBuildCosts[resourceName] == 0) { continue; }
            atLeastOne = true;

            outputString += "    " + resourceName + ": " + resourceDescriptor.mBuildCosts[resourceName] + "\n";
        }
        if (!atLeastOne)
        {
            outputString += "    None\n";
        }

        outputString += "\n";

        // Costs
        outputString += "Inputs: \n";
        atLeastOne = false;
        foreach (string resourceName in cResourceDescriptor.mAllResourceNames)
        {
            if (resourceDescriptor.mInputRates[resourceName] == 0) { continue; }
            atLeastOne = true;

            outputString += "    " + resourceName + ": " + resourceDescriptor.mInputRates[resourceName] + "\n";
        }
        if (!atLeastOne)
        {
            outputString += "    None\n";
        }

        outputString += "\n";
        outputString += "Buffs: \n";
        atLeastOne = false;
        foreach (string statName in cStatsDescriptor.mAllStatsName)
        {
            if (statsDescriptor.mStatValues[statName] == 0) { continue; }
            atLeastOne = true;

            outputString += "    " + statName + ": " + statsDescriptor.mStatValues[statName] + "\n";
        }
        if (!atLeastOne)
        {
            outputString += "    None\n";
        }

        return outputString;
    }
}
