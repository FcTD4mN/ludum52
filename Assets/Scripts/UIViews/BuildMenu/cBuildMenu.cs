using UnityEngine;
using System;
using UnityEngine.EventSystems;
using ImGuiNET;

public class cBuildMenuIMGUI:
    cWindowIMGUI
{
    public Action<RTSManager.eBuildingList> mOnBuildingClicked;

    private cProductionBuildingPanelIMGUI mProdBuildings;
    private cBuffBuildingPanelIMGUI mBuffBuildings;

    public enum eVisiblePanel
    {
        kProd,
        kBuff,
        kWeapon
    }
    public eVisiblePanel mVisiblePanel = eVisiblePanel.kProd;

    public cBuildMenuIMGUI( string name, Rect frame ): base( name, frame )
    {
        mPadding = new Vector2( 24, 24 );
        mProdBuildings = new cProductionBuildingPanelIMGUI( "Production Buildings" );
        mProdBuildings.mOnBuildingClicked = (building) => { mOnBuildingClicked?.Invoke(building); };
        mBuffBuildings = new cBuffBuildingPanelIMGUI( "Buff Buildings");
        mBuffBuildings.mOnBuildingClicked = (building) => { mOnBuildingClicked?.Invoke(building); };

        mFlags = ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoResize;
    }


    public override void RenderChildren()
    {
        base.RenderChildren();

        if (mVisiblePanel == eVisiblePanel.kProd)
            mProdBuildings.Render();
        else if (mVisiblePanel == eVisiblePanel.kBuff)
            mBuffBuildings.Render();
    }
}