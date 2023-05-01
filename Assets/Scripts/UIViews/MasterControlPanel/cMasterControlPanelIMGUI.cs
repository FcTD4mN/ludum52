using UnityEngine;
using ImGuiNET;


class cMasterControlPanelIMGUI :
    cWindowIMGUI
{
    private cMCPResourcePanelIMGUI mPanel;
    private cMCPHarvesterListPanelIMGUI mHarvesterPanel;
    private cMCPBuildingListPanelIMGUI mProdPanel;
    private cMCPBuffersListPanelIMGUI mBuffsPanel;

    public cMasterControlPanelIMGUI(string name, Rect frame) : base(name, frame)
    {
        mPadding = new Vector2(24, 24);
        mFlags = ImGuiWindowFlags.NoCollapse;
        mPanel = new cMCPResourcePanelIMGUI("Resources");
        mHarvesterPanel = new cMCPHarvesterListPanelIMGUI("HarvesterPanel");
        mProdPanel = new cMCPProductionListPanelIMGUI("ProdPanel");
        mBuffsPanel = new cMCPBuffersListPanelIMGUI("BuffPanel");
    }


    public override void RenderChildren()
    {
        base.RenderChildren();

        var availableWidth = ImGui.GetWindowContentRegionWidth();

        ImGui.BeginChild("resource", new Vector2(availableWidth * 0.3f, -1), true);
        mPanel.Render();
        ImGui.EndChild();

        ImGui.SameLine();

        ImGui.BeginChild("buildingPanels", new Vector2(availableWidth * 0.7f, -1), true);

        mHarvesterPanel.Render();

        ImGui.NewLine();
        ImGui.Separator();
        ImGui.NewLine();

        mProdPanel.Render();

        ImGui.NewLine();
        ImGui.Separator();
        ImGui.NewLine();

        mBuffsPanel.Render();

        ImGui.EndChild();
    }
}