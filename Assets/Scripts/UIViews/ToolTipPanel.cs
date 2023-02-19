using UnityEngine;
using System;


class cToolTipPanel: cPanel
{
    public enum ePosition
    {
        kBottom,
        kRight
    };
    public ePosition mShowPosition = ePosition.kBottom;
    public int mDistance = 0;


    private WeakReference<cView> mMaster;
    private cLabel mLabel;


    public cToolTipPanel(GameObject parentView, string name, cView masterView ) : base(parentView, name)
    {
        mMaster = new WeakReference<cView>( masterView );
        mLabel = new cLabel(mGameObject, "text");
        mLabel.mText.alignment = TMPro.TextAlignmentOptions.TopLeft;

        // SetColor(new Color(0.13f, 0.2f, 0.2f, 0.5f));
    }


    public void SetText( string text )
    {
        mLabel.mText.text = text;
        UpdateFrameToFit();
    }


    private void UpdateFrameToFit()
    {
        cView master;
        mMaster.TryGetTarget(out master);
        if (master == null) return;

        var parentView = mGameObject.transform.parent.gameObject;

        var frame = master.GetFrameRelativeTo(parentView);
        mLabel.mText.ForceMeshUpdate();
        var textWidth = mLabel.mText.textBounds.size.x;
        var textHeight = mLabel.mText.textBounds.size.y;

        if( mShowPosition == ePosition.kBottom )
        {
            SetFrame(new Rect(frame.xMin, frame.yMin + frame.height + mDistance, textWidth + 20, textHeight + 20));
        }
        else if( mShowPosition == ePosition.kRight )
        {
            SetFrame(new Rect(frame.xMax + mDistance, frame.yMin + (frame.height - textHeight) / 2, textWidth + 20, textHeight + 20));
        }
        mLabel.SetFrame(new Rect(10, 10, textWidth, textHeight));
    }
}