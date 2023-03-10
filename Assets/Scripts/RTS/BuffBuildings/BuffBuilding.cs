using System;
using System.Collections.Generic;
using UnityEngine;

using static cResourceDescriptor;

public abstract class BuffBuilding : ProductionBuilding
{
    internal cStatsDescriptor mStatsModifiers;

    internal bool mAdds = true;
    private bool mStatsAreApplied = false;
    protected cCompleteStats mObjectToApplyStatsTo;

    abstract public cStatsDescriptor GetStatsDescriptor();


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
        mResourceDescriptor = GetNewResourceDescriptor();
        mStatsModifiers = GetStatsDescriptor();
        mObjectToApplyStatsTo = GameManager.mInstance.playerCtrler?.GetComponent<HasStats>()?.mStats;
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

        if (mObjectToApplyStatsTo == null) { return; }

        if( mAdds ) {
            mObjectToApplyStatsTo.AddStatsAddition(mStatsModifiers);
        }
        else{
            mObjectToApplyStatsTo.AddStatsMultipliers(mStatsModifiers);
        }
    }
    internal void RetractStats()
    {
        if (!mStatsAreApplied) { return; }
        mStatsAreApplied = false;

        if( mObjectToApplyStatsTo == null ) { return; }

        cStatsDescriptor inverse = new cStatsDescriptor(mStatsModifiers);
        inverse.ApplyOnEveryStat(val => -val);

        if (mAdds) {
            mObjectToApplyStatsTo.AddStatsAddition(inverse);
        }
        else {
            mObjectToApplyStatsTo.AddStatsMultipliers(inverse);
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





    public new string GetUIDescription( bool isAllowed )
    {
        string name = GetDisplayName();
        string description = GetDescription();

        RTSManager.eBuildingErrors error = GetBuildingError();

        string errorMessage = "";
        switch (error)
        {
            case RTSManager.eBuildingErrors.BlueprintRequired:
                errorMessage = "Blueprint required";
                break;
            case RTSManager.eBuildingErrors.NotEnoughRessources:
                errorMessage = "Not enough resources";
                break;
            case RTSManager.eBuildingErrors.None:
                errorMessage = isAllowed ? "" : "Can't build that type of building here";
                break;
        }

        return BuffBuilding.GetBuffBuildingUIDescription(name, description, errorMessage, GetNewResourceDescriptor(), GetStatsDescriptor());
    }


    public static string GetBuffBuildingUIDescription( string name,
                                                        string description,
                                                        string errorMessage,
                                                        cResourceDescriptor resourceDescriptor,
                                                        cStatsDescriptor statsDescriptor )
    {
        string outputString = "";
        string redColor = "<color=#EB0800>";

        outputString = name + "\n" + description + "\n";

        outputString += "\n";
        if (errorMessage != "")
        {
            outputString += redColor + "Issue: " + errorMessage + "</color>";
            outputString += "\n";
            outputString += "\n";
            outputString += "\n";
        }

        outputString += "Build Cost: \n";
        bool atLeastOne = false;
        foreach (eResourceNames resourceName in Enum.GetValues(typeof(eResourceNames)))
        {
            if (resourceDescriptor.mBuildCosts[resourceName] == 0) { continue; }
            atLeastOne = true;

            bool hasEnough = resourceDescriptor.mBuildCosts[resourceName] <= GameManager.mResourceManager.GetRessource(resourceName);
            string colorBalise = hasEnough ? "" : redColor;
            string colorBaliseEnd = hasEnough ? "" : "</color>";

            outputString += "    " + colorBalise + resourceName + ": " + resourceDescriptor.mBuildCosts[resourceName] + colorBaliseEnd + "\n";
        }
        if (!atLeastOne)
        {
            outputString += "    None\n";
        }

        outputString += "\n";

        // Costs
        outputString += "Inputs: \n";
        atLeastOne = false;
        foreach (eResourceNames resourceName in Enum.GetValues(typeof(eResourceNames)))
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
        foreach (cStatsDescriptor.eStatsNames stat in Enum.GetValues(typeof(cStatsDescriptor.eStatsNames)))
        {
            var statName = stat.ToString();
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
