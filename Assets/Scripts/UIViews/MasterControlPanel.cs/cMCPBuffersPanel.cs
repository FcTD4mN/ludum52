using UnityEngine;
using System.Collections.Generic;
using System;



class cMCPBuffersPanel :
    cMCPBuildingPanelBase
{
    public cMCPBuffersPanel(GameObject parentView, string name, cMasterControlPanel master) : base(parentView, name, "Buffer Buildings", master)
    {
    }

    internal override List<ProductionBuilding> GetBuildingList()
    {
        var producers = new List<ProductionBuilding>();

        foreach (var building in GameManager.mRTSManager.mBuffTower.mBuildings)
        {
            producers.Add(building);
        }

        return producers;
    }
}
