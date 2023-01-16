using UnityEngine;
using System.Collections.Generic;

class cProductionBuildingPanel:
    cBuildPanelBase
{
    public cProductionBuildingPanel(GameObject parentView, string name) : base(parentView, name)
    {
    }

    internal override List<RTSManager.eBuildingList> GetBuildingToShowList()
    {
        List<RTSManager.eBuildingList> towerBuildings = new List<RTSManager.eBuildingList>();

        towerBuildings.Add( RTSManager.eBuildingList.Forge );
        towerBuildings.Add( RTSManager.eBuildingList.BombFactory );
        towerBuildings.Add( RTSManager.eBuildingList.Workshop );

        return  towerBuildings;
    }
}

