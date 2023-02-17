using UnityEngine;
using System.Collections.Generic;
using System;



class cMCPProductionPanel :
    cMCPBuildingPanelBase
{
    public cMCPProductionPanel(GameObject parentView, string name, cMasterControlPanel master) : base(parentView, name, "Main Tower Buildings", master)
    {
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


    // TODO
    public void OnClick( GameObject buttonClicked )
    {
        var buildMenu = new cBuildMenu(mGameObject, "buildMenu");
        buildMenu.mOnBuildingClicked = (building) =>
        {

        };
    }
}
