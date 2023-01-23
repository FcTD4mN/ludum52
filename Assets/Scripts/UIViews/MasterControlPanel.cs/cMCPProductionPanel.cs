using UnityEngine;
using System.Collections.Generic;
using System;



class cMCPProductionPanel :
    cMCPBuildingPanelBase
{
    public cMCPProductionPanel(GameObject parentView, string name, cMasterControlPanel master) : base(parentView, name, "Producers", master)
    {
    }

    internal override List<ProductionBuilding> GetBuildingList()
    {
        var producers = new List<ProductionBuilding>();
        foreach (var building in GameManager.mRTSManager.mAllProductionBuildings)
        {
            if (building.gameObject.GetComponent<HarvestingBuilding>() != null) continue;
            if (building.gameObject.GetComponent<BuffBuilding>() != null) continue;

            producers.Add(building);
        }

        return  producers;
    }
}
