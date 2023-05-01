using UnityEngine;
using System;
using ImGuiNET;



public class cFloatingButton:
    cWindowIMGUI
{
    public cFloatingButton( string label, Rect frame ) :base( mID.ToString(), frame )
    {
        ++mID;
        mPadding = Vector2.zero;
        mFlags = ImGuiWindowFlags.NoTitleBar | ImGuiWindowFlags.NoResize | ImGuiWindowFlags.NoMove | ImGuiWindowFlags.NoCollapse | ImGuiWindowFlags.NoScrollbar
                    | ImGuiWindowFlags.NoFocusOnAppearing | ImGuiWindowFlags.NoBringToFrontOnFocus;
        mButtonLabel = label;
    }


    public override void RenderChildren()
    {
        base.RenderChildren();

        if( ImGui.Button( mButtonLabel, new Vector2( mFrame.width, mFrame.height ) ) )
        {
            mOnButtonClicked?.Invoke();
        }
        if( ImGui.IsItemHovered( ImGuiHoveredFlags.None ) && mHoverText != "" )
        {
            ImGui.GetStyle().WindowPadding = new Vector2( 24, 24 );
            ImGui.BeginTooltip();
            ImGui.PushTextWrapPos(450.0f);
            ImGui.Text( mHoverText );
            ImGui.PopTextWrapPos();
            ImGui.EndTooltip();
            ImGui.GetStyle().WindowPadding = mPadding;
        }
    }


    public Action mOnButtonClicked;
    public string mButtonLabel = "None";
    public string mHoverText = "";
    static int mID = 1;
}




public class cFloatingButtonPauseResume :
    cFloatingButton
{
    public cFloatingButtonPauseResume(string label, Rect frame) : base(label, frame)
    {
    }


    public override void RenderChildren()
    {
        float closeSize = 24f;
        ImGui.SetCursorPos( new Vector2( mFrame.width - closeSize, 0 ) );
        ImGui.PushStyleColor( ImGuiCol.Button, new Vector4( 0.8f, 0.1f, 0.1f, 1f ) );
        if (ImGui.Button("X", new Vector2(closeSize, closeSize)))
        {
            mOnDeleteClicked?.Invoke();
        }
        ImGui.PopStyleColor();

        if (ImGui.IsItemHovered())
        {
            ImGui.GetStyle().WindowPadding = new Vector2(closeSize, closeSize);
            ImGui.BeginTooltip();
            ImGui.PushTextWrapPos(450.0f);
            ImGui.Text("Remove Building");
            ImGui.PopTextWrapPos();
            ImGui.EndTooltip();
            ImGui.GetStyle().WindowPadding = mPadding;
        }


        float pauseButtonHeight = mFrame.height - closeSize * 2;
        ImGui.SetCursorPos(new Vector2(0, mFrame.height - pauseButtonHeight));
        ImGui.PushStyleColor(ImGuiCol.Button, new Vector4(0.1f, 0.1f, 0.1f, 0.2f));
        ImGui.PushStyleColor(ImGuiCol.ButtonHovered, new Vector4(0.3f, 0.3f, 0.3f, 0.2f));

        if (ImGui.Button(mButtonLabel, new Vector2(mFrame.width, pauseButtonHeight)))
        {
            mOnButtonClicked?.Invoke();
        }
        ImGui.PopStyleColor(2);

        if( ImGui.IsItemHovered() )
        {
            ImGui.GetStyle().WindowPadding = new Vector2(closeSize, closeSize);
            ImGui.BeginTooltip();
            ImGui.PushTextWrapPos(450.0f);
            ImGui.Text("Pause/Resume");
            ImGui.PopTextWrapPos();
            ImGui.EndTooltip();
            ImGui.GetStyle().WindowPadding = mPadding;
        }

        if (ImGui.IsWindowHovered())
        {
            mOnHover?.Invoke();
        }
    }

    public Action mOnDeleteClicked;
    public Action mOnHover;
}