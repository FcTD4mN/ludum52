using UnityEngine;
using System.Collections.Generic;
using System;
using ImGuiNET;


abstract class cMCPBuildingListPanelIMGUI :
    cViewIMGUI
{
    // UI variables


    public cMCPBuildingListPanelIMGUI(string name) : base()
    {
    }


    abstract public string GetListTitle();
    abstract public List<ProductionBuilding> GetBuildingList();
    abstract protected void ShowBuildMenu( int index );
    abstract protected string GetHoverTextForBuilding( ProductionBuilding building );


    public override void Render()
    {
        base.Render();

        string title = GetListTitle();

        ImGui.Text( title );
        ImGui.Indent();

        ImGui.Columns( 5, "colums" + title, true );

        ImGui.Text( "Name" ); ImGui.NextColumn();
        ImGui.Text( "Inputs" ); ImGui.NextColumn();
        ImGui.Text( "Outputs" ); ImGui.NextColumn();
        ImGui.Text( "Efficiency" ); ImGui.NextColumn();
        ImGui.Text( "" ); ImGui.NextColumn();

        ImGui.NewLine();ImGui.NextColumn();
        ImGui.NewLine();ImGui.NextColumn();
        ImGui.NewLine();ImGui.NextColumn();
        ImGui.NewLine();ImGui.NextColumn();
        ImGui.NewLine();ImGui.NextColumn();

        int tt = 0;
        foreach( var building in GetBuildingList() )
        {
            // Column 0
            if( building == null )
            {
                if( ImGui.Button( "<Empty>##" + title + tt ) )
                {
                    ShowBuildMenu( tt );
                }
            }
            else
            {
                ImGui.Text( building.GetDisplayName() );
                if( ImGui.IsItemHovered() )
                {
                    string hoverText = GetHoverTextForBuilding( building );
                    if( hoverText != "" )
                    {
                        ImGui.PushStyleVar( ImGuiStyleVar.WindowPadding, new Vector2(24, 24) );
                        ImGui.BeginTooltip();
                        ImGui.PushTextWrapPos(450.0f);
                        ImGui.Text( hoverText );
                        ImGui.PopTextWrapPos();
                        ImGui.EndTooltip();
                        ImGui.PopStyleVar();
                    }
                }
            }
            ImGui.NextColumn();


            // Column 1
            RenderInputsForBuilding( building ); ImGui.NextColumn();


            // Column 2
            RenderOutputsForBuilding(building); ImGui.NextColumn();


            // Column 3
            if( building != null )
            {
                float prodRatio = building.IsPaused() ? 0 : building.GetProductionRatio();
                var prodColor = Color.Lerp(Color.red, Color.green, prodRatio);
                ImGui.TextColored( new Vector4(prodColor.r, prodColor.g, prodColor.b, prodColor.a ), (int)(prodRatio * 100f) + "%%" );
            }
            ImGui.NextColumn();


            // Column 4
            if (building != null)
                if( ImGui.Button( (building.IsPaused() ? "Resume##" : "Pause##") + title + tt, new Vector2( -1, 30)) )
                {
                    if (building == null) { return; }

                    building.SetPause(!building.IsPaused());
                }

            ImGui.NextColumn();
            ++tt;
        }

        ImGui.Columns( 1 );
        ImGui.Unindent();
    }


    private void RenderInputsForBuilding( ProductionBuilding building )
    {
        if (building == null) { return; }

        var buildingResourceDescriptor = building.GetResourceDescriptor();
        foreach (var resource in buildingResourceDescriptor.mInputRates)
        {
            if (resource.Value == 0) continue;

            float buildingRatio = building.GetProductionRatio();

            ImGui.Text( "" + resource.Key.ToString()[0] );

            float value = building.IsPaused() ? 0 : GetResourceValue( building, resource.Key, cResourceDescriptor.eResourceType.kInput) * buildingRatio;
            Vector4 color = value == 0 ? new Vector4(0.8f, 0.8f, 0.8f, 1f) : new Vector4(1f, 0.1f, 0.1f, 1f);

            ImGui.SameLine();
            ImGui.TextColored( color, ((int)value).ToString() );
            ImGui.SameLine();
        }

        ImGui.NewLine();
    }

    private void RenderOutputsForBuilding( ProductionBuilding building )
    {
        if (building == null) { return; }

        var buildingResourceDescriptor = building.GetResourceDescriptor();
        foreach (var resource in buildingResourceDescriptor.mOutputRates)
        {
            if (resource.Value == 0) continue;

            float buildingRatio = building.GetProductionRatio();

            ImGui.Text( "" + resource.Key.ToString()[0] );

            float value = building.IsPaused() ? 0 : GetResourceValue( building, resource.Key, cResourceDescriptor.eResourceType.kOutput) * buildingRatio;
            Vector4 color = value == 0 ? new Vector4(0.8f, 0.8f, 0.8f, 1f) : new Vector4(0.1f, 1f, 0.1f, 1f);

            ImGui.SameLine();
            ImGui.TextColored( color, ((int)value).ToString() );
            ImGui.SameLine();
        }

        ImGui.NewLine();
    }


    private float GetResourceValue( ProductionBuilding building, cResourceDescriptor.eResourceNames resourceName, cResourceDescriptor.eResourceType resourceType )
    {
        var resourceDescriptor = building.GetResourceDescriptor();
        switch (resourceType)
        {
            case cResourceDescriptor.eResourceType.kBuildCost:
                return resourceDescriptor.mBuildCosts[resourceName];
            case cResourceDescriptor.eResourceType.kInput:
                return resourceDescriptor.mInputRates[resourceName];
            case cResourceDescriptor.eResourceType.kOutput:
                return resourceDescriptor.mOutputRates[resourceName];
        }

        return -1f;
    }
}