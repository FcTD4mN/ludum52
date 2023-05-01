using UnityEngine;
using ImGuiNET;


public class cWindowIMGUI :
    cViewIMGUI
{
    public cWindowIMGUI(string name, Rect frame) : base()
    {
        GameManager.mDearImGui.mViewsToAdd.Add(this);
        mFrame = frame;
        mWindowName = name;
    }
    public cWindowIMGUI(string name, Rect frame, ImGuiWindowFlags flags) : base()
    {
        GameManager.mDearImGui.mViewsToAdd.Add(this);
        mFrame = frame;
        mFlags = flags;
        mWindowName = name;
    }


    override public void Render()
    {
        if (!mIsOpen)
        { return; }

        if (!mInit)
        {
            ImGui.SetNextWindowSize(new Vector2(mFrame.width, mFrame.height));
            ImGui.SetNextWindowPos(new Vector2(mFrame.xMin, mFrame.yMin));
            mInit = true;
        }

        ImGui.GetStyle().WindowPadding = new Vector2(mPadding.x, mPadding.y);
        if (!ImGui.Begin(mWindowName, ref mIsOpen, mFlags))
        {
            return;
        }

        RenderChildren();

        ImGui.End();
    }

    virtual public void RenderChildren()
    {
    }


    public void DestroyWindow()
    {
        mIsOpen = false;
        GameManager.mDearImGui.mViewsToDelete.Add(this);
    }

    public bool mIsOpen = true;
    private bool mInit = false;

    public Rect mFrame = Rect.zero;
    public Vector2 mPadding = new Vector2(8, 8);
    public ImGuiWindowFlags mFlags = 0;
    public string mWindowName = "Untitled";
}
