using UnityEngine;
using System.Collections.Generic;


class cBuffBuildingPanelIMGUI :
    cBuildPanelBaseIMGUI
{
    public cBuffBuildingPanelIMGUI(string name) : base(name)
    {
    }

    internal override List<RTSManager.eBuildingList> GetBuildingToShowList()
    {
        List<RTSManager.eBuildingList> towerBuildings = new List<RTSManager.eBuildingList>();

        towerBuildings.Add(RTSManager.eBuildingList.BuffCooldown);
        towerBuildings.Add(RTSManager.eBuildingList.BuffSpeed);
        towerBuildings.Add(RTSManager.eBuildingList.BuffDamage);
        towerBuildings.Add(RTSManager.eBuildingList.BuffJump);

        return towerBuildings;
    }
}