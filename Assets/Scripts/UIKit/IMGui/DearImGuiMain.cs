using UnityEngine;
using ImGuiNET;
using System.Collections.Generic;




public class DearImGuiMain : MonoBehaviour
{
    public List<cViewIMGUI> mVisibleViews;

    public List<cViewIMGUI> mViewsToDelete;
    public List<cViewIMGUI> mViewsToAdd;

    public void Initialize()
    {
        ImGuiUn.Layout += OnLayout;
        mVisibleViews = new List<cViewIMGUI>();
        mViewsToDelete = new List<cViewIMGUI>();
        mViewsToAdd = new List<cViewIMGUI>();
    }

    void OnDisable()
    {
        ImGuiUn.Layout -= OnLayout;
        mVisibleViews.Clear();
        mViewsToDelete.Clear();
        mViewsToAdd.Clear();

    }

    void OnLayout()
    {
        ImGui.ShowDemoWindow();


        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.3f, 0.3f, 0.3f, 1f));

        foreach( var view in mVisibleViews )
        {
            view.Render();
        }

        ImGui.PopStyleColor();

        foreach (var view in mViewsToDelete)
        {
            mVisibleViews.Remove( view );
        }
        mViewsToDelete.Clear();
        foreach (var view in mViewsToAdd)
        {
            mVisibleViews.Add( view );
        }
        mViewsToAdd.Clear();
    }
}