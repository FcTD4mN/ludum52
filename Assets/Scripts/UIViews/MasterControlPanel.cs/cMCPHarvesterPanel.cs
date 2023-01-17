using UnityEngine;
using System.Collections.Generic;
using System;


class cMCPHarvesterPanel :
    cMCPBuildingPanelBase
{
    public cMCPHarvesterPanel(GameObject parentView, string name) : base(parentView, name, "Harvesters")
    {
    }

    internal override List<ProductionBuilding> GetBuildingList()
    {
        return  GameManager.mRTSManager.mAllHarvesters.ConvertAll( new Converter<HarvestingBuilding, ProductionBuilding>( (harvester) => {
            return harvester.gameObject.GetComponent<ProductionBuilding>();
            }));
    }
}
