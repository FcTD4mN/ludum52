using System.Collections.Generic;


class cWeaponBuildingPanelIMGUI :
    cBuildPanelBaseIMGUI
{
    public cWeaponBuildingPanelIMGUI(string name) : base(name)
    {
    }

    internal override List<RTSManager.eBuildingList> GetBuildingToShowList()
    {
        List<RTSManager.eBuildingList> towerBuildings = new List<RTSManager.eBuildingList>();

        return towerBuildings;
    }
}