using UnityEngine;
using System.Collections.Generic;

class cWeaponBuildingPanel :
    cBuildPanelBase
{
    public cWeaponBuildingPanel(GameObject parentView, string name) : base(parentView, name)
    {
    }

    internal override List<RTSManager.eBuildingList> GetBuildingToShowList()
    {
        List<RTSManager.eBuildingList> towerBuildings = new List<RTSManager.eBuildingList>();

        return towerBuildings;
    }
}

