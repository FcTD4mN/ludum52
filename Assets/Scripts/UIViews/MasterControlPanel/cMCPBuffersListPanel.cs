using System.Collections.Generic;
using UnityEngine;

class cMCPBuffersListPanelIMGUI :
    cMCPBuildingListPanelIMGUI
{
    public cMCPBuffersListPanelIMGUI(string name) : base(name)
    {
    }

    override public string GetListTitle()
    {
        return "Buffer Buildings";
    }

    override public List<ProductionBuilding> GetBuildingList()
    {
        var producers = new List<ProductionBuilding>();

        foreach (var building in GameManager.mRTSManager.mBuffTower.mBuildings)
        {
            producers.Add(building);
        }

        return producers;
    }


    protected override void ShowBuildMenu(int index)
    {
        Rect screenRect = Camera.main.pixelRect;
        var buildMenu = new cBuildMenuIMGUI("buffPanelBuildMenu", new Rect(screenRect.center.x - 250, screenRect.center.y - 250, 500, 500));
        buildMenu.mVisiblePanel = cBuildMenuIMGUI.eVisiblePanel.kBuff;
        buildMenu.mOnBuildingClicked = (building) =>
        {
            GameManager.mRTSManager.mBuffTower.BuildAtIndex(building.ToString(), index);
            buildMenu.DestroyWindow();
        };
    }


    protected override string GetHoverTextForBuilding( ProductionBuilding building )
    {
        if (building == null) return "";

        var buffBuilding = building.GetComponent<BuffBuilding>();
        if( buffBuilding == null ) return  "";

        string finalText = "Stats buffed: \n";
        foreach (var resource in buffBuilding.mStatsModifiers.mStatValues)
        {
            if (resource.Value == 0) continue;

            finalText += " " + resource.Key.ToString() + ": " + ((int)resource.Value).ToString() + "\n";
        }

        return  finalText;
    }
}