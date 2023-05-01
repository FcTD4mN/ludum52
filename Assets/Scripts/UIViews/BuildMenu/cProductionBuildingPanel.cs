using System.Collections.Generic;


class cProductionBuildingPanelIMGUI :
    cBuildPanelBaseIMGUI
{
    public cProductionBuildingPanelIMGUI( string name ): base( name )
    {
    }

    internal override List<RTSManager.eBuildingList> GetBuildingToShowList()
    {
        List<RTSManager.eBuildingList> towerBuildings = new List<RTSManager.eBuildingList>();

        towerBuildings.Add(RTSManager.eBuildingList.Forge);
        towerBuildings.Add(RTSManager.eBuildingList.BombFactory);
        towerBuildings.Add(RTSManager.eBuildingList.Workshop);

        return towerBuildings;
    }
}