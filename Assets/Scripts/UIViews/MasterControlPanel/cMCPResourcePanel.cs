using UnityEngine;
using ImGuiNET;
using System;


class cMCPResourcePanelIMGUI:
    cViewIMGUI
{
    private bool mFirst = true;
    public cMCPResourcePanelIMGUI( string name ) : base()
    {
    }

    public override void Render()
    {
        base.Render();
        if( GameManager.mRTSManager == null ) return;

        ImGui.Columns( 5, "resourcesColums", true );

        if( mFirst )
        {
            float usableWidth = ImGui.GetWindowContentRegionWidth();
            float ratio1 = 0.3f;
            ImGui.SetColumnWidth( 0, usableWidth * ratio1 );

            float widthOthers = usableWidth * (1 - ratio1) / 4;
            ImGui.SetColumnWidth( 1, widthOthers );
            ImGui.SetColumnWidth( 2, widthOthers );
            ImGui.SetColumnWidth( 3, widthOthers );
            ImGui.SetColumnWidth( 4, widthOthers );
            mFirst = false;
        }

        ImGui.Text("Resource"); ImGui.NextColumn();
        ImGui.Text("Amount");   ImGui.NextColumn();
        ImGui.Text("Input");    ImGui.NextColumn();
        ImGui.Text("Output");   ImGui.NextColumn();
        ImGui.Text("Flow");     ImGui.NextColumn();


        ImGui.NewLine(); ImGui.NextColumn();
        ImGui.NewLine(); ImGui.NextColumn();
        ImGui.NewLine(); ImGui.NextColumn();
        ImGui.NewLine(); ImGui.NextColumn();
        ImGui.NewLine(); ImGui.NextColumn();


        foreach (cResourceDescriptor.eResourceNames resource in Enum.GetValues(typeof(cResourceDescriptor.eResourceNames)))
        {
            ImGui.Text( resource.ToString() ); ImGui.NextColumn();

            float inputValue = 0;
            float outputValue = 0;
            foreach (var building in GameManager.mRTSManager.mAllProductionBuildings)
            {
                inputValue += building.IsPaused() ? 0 : building.mResourceDescriptor.mInputRates[resource] * building.GetProductionRatio();
                outputValue += building.IsPaused() ? 0 : building.mResourceDescriptor.mOutputRates[resource] * building.GetProductionRatio();
            }
            float totalRate = outputValue - inputValue;

            ImGui.Text("" + (int)GameManager.mResourceManager.GetRessource(resource) ); ImGui.NextColumn();
            ImGui.TextColored( GetColorForValue(outputValue), "" + (int)outputValue ); ImGui.NextColumn();
            ImGui.TextColored( GetColorForValue(inputValue), "" + (int)inputValue ); ImGui.NextColumn();
            ImGui.TextColored( GetColorForValue(totalRate), "" + (int)totalRate ); ImGui.NextColumn();
        }

        ImGui.Columns( 1 );
    }


    private Vector4 GetColorForValue(float value)
    {
        Vector4 color = value < 0 ? new Vector4( 1f, 0.2f, 0.2f, 1f ) : new Vector4(0.2f, 1f, 0.2f, 1f);
        if (value == 0) color = new Vector4(0.8f, 0.8f, 0.8f, 1f);

        return color;
    }



    class cResourceLineIMGUI:
        cViewIMGUI
    {
        public cResourceLineIMGUI( string name ) : base()
        {

        }
    }
}