using UnityEngine;
using System.Collections.Generic;
using System;
using ImGuiNET;


abstract class cBuildPanelBaseIMGUI :
    cViewIMGUI
{
    private List<RTSManager.eBuildingList> mBuildingToShow;

    public Action<RTSManager.eBuildingList> mOnBuildingClicked;

    // UI variables
    public float mPadding = 8;
    public float mButtonSize = 128;
    public float mMinSpacing = 16;

    public string mListName = "";


    public cBuildPanelBaseIMGUI( string name ): base()
    {
        mBuildingToShow = GetBuildingToShowList();
        mListName = name;
    }


    abstract internal List<RTSManager.eBuildingList> GetBuildingToShowList();
    public override void Render()
    {
        float listWidth = ImGui.GetWindowContentRegionWidth();
        // ImGui.GetStyle().WindowPadding = new Vector2 (mPadding, mPadding);
        ImGui.Text(mListName + ":");

        int numberOfButtonPerRow = (int)((listWidth) / (float)(mButtonSize + mMinSpacing));

        ImGui.BeginChild( "test" + mListName );
        ImGui.Columns( numberOfButtonPerRow, "column"+mListName, false );

        int i = 0;
        foreach (RTSManager.eBuildingList building in mBuildingToShow)
        {
            string buildingName = building.ToString();

            int rowIndex = i / numberOfButtonPerRow;
            int columnIndex = i % numberOfButtonPerRow;

            ProductionBuilding productionBuilding = GameManager.mRTSManager.GetPrefabByType(building);
            if (productionBuilding.IsBuildable())
            {
                // Temporary bg color before png images of buildings
                switch (building)
                {
                    case RTSManager.eBuildingList.Forge:
                        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.63f, 0.38f, 0.65f, 1f));
                        break;
                    case RTSManager.eBuildingList.BombFactory:
                        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.65f, 0.38f, 0.38f, 1f));
                        break;
                    case RTSManager.eBuildingList.Workshop:
                        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.38f, 0.39f, 0.65f, 1f));
                        break;
                    default:
                        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.38f, 0.39f, 0.01f, 1f));
                        break;
                }

                if (ImGui.Button(buildingName, new Vector2(mButtonSize, mButtonSize)))
                {
                    mOnBuildingClicked?.Invoke(building);
                }

                if (ImGui.IsItemHovered())
                {
                    ProductionBuilding prod = GameManager.mRTSManager.GetPrefabByType((RTSManager.eBuildingList)building);
                    ImGui.BeginTooltip();
                    ImGui.PushTextWrapPos(450.0f);
                    ImGui.Text(prod.GetUIDescription(true));
                    ImGui.PopTextWrapPos();
                    ImGui.EndTooltip();
                }
            }
            else
            {
                ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.38f, 0.38f, 0.38f, 0.5f));
                ImGui.Button(buildingName, new Vector2(mButtonSize, mButtonSize));

                if (ImGui.IsItemHovered())
                {
                    ImGui.BeginTooltip();
                    ImGui.PushTextWrapPos(450.0f);
                    ImGui.Text("Unavailable");
                    ImGui.PopTextWrapPos();
                    ImGui.EndTooltip();
                }
            }

            ImGui.NextColumn();
            ImGui.PopStyleColor();
            ++i;
        }

        ImGui.Columns( 1 );
        ImGui.EndChild();
    }
}