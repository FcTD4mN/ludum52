using System.Collections.Generic;
using UnityEngine;

class cMCPWeaponsListPanelIMGUI :
    cMCPBuildingListPanelIMGUI
{
    public cMCPWeaponsListPanelIMGUI(string name) : base(name)
    {
    }

    override public string GetListTitle()
    {
        return "Weapon Buildings";
    }

    override public List<ProductionBuilding> GetBuildingList()
    {
        var producers = new List<ProductionBuilding>();

        foreach (var building in GameManager.mRTSManager.mTowerWeapon.mBuildings)
        {
            producers.Add(building);
        }

        return producers;
    }


    protected override void ShowBuildMenu(int index)
    {
        Rect screenRect = Camera.main.pixelRect;
        var buildMenu = new cBuildMenuIMGUI("weaponPanelBuildMenu", new Rect(screenRect.center.x - 250, screenRect.center.y - 250, 500, 500));
        buildMenu.mVisiblePanel = cBuildMenuIMGUI.eVisiblePanel.kWeapon;
        buildMenu.mOnBuildingClicked = (building) =>
        {
            GameManager.mRTSManager.mTowerWeapon.BuildAtIndex(building.ToString(), index);
            buildMenu.DestroyWindow();
        };
    }


    protected override string GetHoverTextForBuilding(ProductionBuilding building)
    {
        return "";
    }
}