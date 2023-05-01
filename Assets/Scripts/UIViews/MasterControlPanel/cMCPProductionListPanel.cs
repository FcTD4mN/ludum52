using System.Collections.Generic;
using UnityEngine;

class cMCPProductionListPanelIMGUI :
    cMCPBuildingListPanelIMGUI
{
    public cMCPProductionListPanelIMGUI(string name) : base( name )
    {
    }

    override public string GetListTitle()
    {
        return  "Production Buildings";
    }

    override public List<ProductionBuilding> GetBuildingList()
    {
        var producers = new List<ProductionBuilding>();

        foreach (var building in GameManager.mRTSManager.mMainTower.mBuildings)
        {
            producers.Add(building);
        }

        return producers;
    }


    protected override void ShowBuildMenu( int index )
    {
        Rect screenRect = Camera.main.pixelRect;
        var buildMenu = new cBuildMenuIMGUI("prodPanelBuildMenu", new Rect(screenRect.center.x - 250, screenRect.center.y - 250, 500, 500));
        buildMenu.mVisiblePanel = cBuildMenuIMGUI.eVisiblePanel.kProd;
        buildMenu.mOnBuildingClicked = (building) =>
        {
            GameManager.mRTSManager.mMainTower.BuildAtIndex(building.ToString(), index);
            buildMenu.DestroyWindow();
        };
    }


    protected override string GetHoverTextForBuilding(ProductionBuilding building)
    {
        return "";
    }
}