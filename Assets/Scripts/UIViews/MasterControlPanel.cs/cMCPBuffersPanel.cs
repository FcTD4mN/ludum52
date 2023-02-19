using UnityEngine;
using System.Collections.Generic;
using System;



class cMCPBuffersPanel :
    cMCPBuildingPanelBase
{
    public cMCPBuffersPanel(GameObject parentView, string name, cMasterControlPanel master) : base(parentView, name, "Buffer Buildings", master)
    {
        ((pDelegateSender)GameManager.mRTSManager.mBuffTower).AddDelegate(this);
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


    override internal void ActionOnEmptyClick(cBuildingLine line, int spotIndex)
    {
        mBuildMenu = new cBuildMenu(GameManager.mUIManager.mCanvas.gameObject, "buildMenu");

        mBuildMenu.mOnClose = () =>
        {
            GameObject.Destroy(mBuildMenu.mGameObject);
            mBuildMenu = null;
        };

        mBuildMenu.mOnBuildingClicked = (building) =>
        {
            GameManager.mRTSManager.mBuffTower.BuildAtIndex(building.ToString(), spotIndex);
            GameObject.Destroy(mBuildMenu.mGameObject);
            mBuildMenu = null;
        };

        Rect screenRect = Camera.main.pixelRect;
        mBuildMenu.SetFrame(new Rect(0, 0, 500, 500));
        mBuildMenu.SetCenter(screenRect.center);

        mBuildMenu.ShowBuffBuildingPanel();
    }


    override internal bool ShouldPerformAction(pDelegateSender sender)
    {
        return sender is TowerBuff;
    }
}
