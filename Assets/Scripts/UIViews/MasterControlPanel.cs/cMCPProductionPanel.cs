using UnityEngine;
using System.Collections.Generic;
using System;



class cMCPProductionPanel :
    cMCPBuildingPanelBase
{
    public cMCPProductionPanel(GameObject parentView, string name, cMasterControlPanel master) : base(parentView, name, "Main Tower Buildings", master)
    {
        ((pDelegateSender)GameManager.mRTSManager.mMainTower).AddDelegate(this);
    }

    internal override List<ProductionBuilding> GetBuildingList()
    {
        var producers = new List<ProductionBuilding>();

        foreach (var building in GameManager.mRTSManager.mMainTower.mBuildings)
        {
            producers.Add( building );
        }

        return  producers;
    }


    override internal void ActionOnEmptyClick( cBuildingLine line, int spotIndex )
    {
        mBuildMenu = new cBuildMenu( GameManager.mUIManager.mCanvas.gameObject, "buildMenu" );

        mBuildMenu.mOnClose = ()=> {
            GameObject.Destroy(mBuildMenu.mGameObject);
            mBuildMenu = null;
        };

        mBuildMenu.mOnBuildingClicked = (building) =>
        {
            GameManager.mRTSManager.mMainTower.BuildAtIndex( building.ToString(), spotIndex );
            GameObject.Destroy( mBuildMenu.mGameObject );
            mBuildMenu = null;
        };

        Rect screenRect = Camera.main.pixelRect;
        mBuildMenu.SetFrame(new Rect(0, 0, 500, 500));
        mBuildMenu.SetCenter(screenRect.center);

        mBuildMenu.ShowProdBuildingPanel();
    }


    override internal bool ShouldPerformAction(pDelegateSender sender)
    {
        return sender is TowerMain;
    }
}
