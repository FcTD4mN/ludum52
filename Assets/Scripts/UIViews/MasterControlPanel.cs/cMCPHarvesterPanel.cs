using UnityEngine;
using System.Collections.Generic;
using System;


class cMCPHarvesterPanel :
    cPanel
{
    private cLabel mTitle;

    private cLabel mLabelColumnName;
    private cLabel mLabelColumnInputs;
    private cLabel mLabelColumnOutputs;
    private cLabel mLabelColumnEfficiency;


    // UI variables
    public float mPadding = 0;
    public float mButtonSize = 128;
    public float mSpacing = 12;

    private int mTitleSize = 18;
    private int mSpaceUnderTitle = 20;

    private int mColumnLabelSize = 14;

    private int mLineHeight = 30;
    private int mLineSpacing = 5;

    private List<cBuildingLine> mAllHarvestersLines;


    public cMCPHarvesterPanel(GameObject parentView, string name) : base(parentView, name)
    {
        mTitle = new cLabel(mGameObject, "title");
        mTitle.mText.text = "Harvesters";
        mTitle.mText.fontStyle = TMPro.FontStyles.Bold;
        mTitle.mText.fontSize = mTitleSize;
        mTitle.mText.alignment = TMPro.TextAlignmentOptions.Left;


        mLabelColumnName = new cLabel(mGameObject, "columnName");
        mLabelColumnName.mText.text = "Name";
        mLabelColumnName.mText.fontStyle = TMPro.FontStyles.Bold;
        mLabelColumnName.mText.fontSize = mColumnLabelSize;
        mLabelColumnName.mText.alignment = TMPro.TextAlignmentOptions.Left;

        mLabelColumnInputs = new cLabel(mGameObject, "columnInputs");
        mLabelColumnInputs.mText.text = "Inputs";
        mLabelColumnInputs.mText.fontStyle = TMPro.FontStyles.Bold;
        mLabelColumnInputs.mText.fontSize = mColumnLabelSize;
        mLabelColumnInputs.mText.alignment = TMPro.TextAlignmentOptions.Left;

        mLabelColumnOutputs = new cLabel(mGameObject, "columnOutputs");
        mLabelColumnOutputs.mText.text = "Outputs";
        mLabelColumnOutputs.mText.fontStyle = TMPro.FontStyles.Bold;
        mLabelColumnOutputs.mText.fontSize = mColumnLabelSize;
        mLabelColumnOutputs.mText.alignment = TMPro.TextAlignmentOptions.Left;


        SetColor(Color.clear);

        mAllHarvestersLines = new List<cBuildingLine>();
        BuildHarvesters();
    }


    public void Update()
    {
        foreach (cBuildingLine line in mAllHarvestersLines)
        {
            line.Update();
        }
    }


    override public void LayoutSubviews()
    {
        var frame = GetFrame();
        int titleHeight = mTitleSize;

        Rect titleFrame = new Rect(mPadding,
                                    mPadding,
                                    frame.width - mPadding * 2,
                                    titleHeight);
        mTitle.SetFrame(titleFrame);

        Rect columnNameRect = new Rect(mPadding + 20 + mLineHeight + cBuildingLine.mBuildingLineSpacing, // Diode size
                                    titleFrame.yMax + mSpaceUnderTitle,
                                    cBuildingLine.mBuildingLineNameWidth,
                                    titleHeight);
        mLabelColumnName.SetFrame(columnNameRect);

        Rect columnInputRect = new Rect(columnNameRect.xMax + cBuildingLine.mBuildingLineSpacing,
                                    columnNameRect.yMin,
                                    cBuildingLine.mBuildingLineResourceWidth,
                                    titleHeight);
        mLabelColumnInputs.SetFrame(columnInputRect);
        Rect columnOutputRect = new Rect(columnInputRect.xMax + cBuildingLine.mBuildingLineSpacing,
                                    columnNameRect.yMin,
                                    cBuildingLine.mBuildingLineResourceWidth,
                                    titleHeight);
        mLabelColumnOutputs.SetFrame(columnOutputRect);

        var framer = new Rect(mPadding + 20,
                                columnNameRect.yMax + mSpaceUnderTitle,
                                frame.width - mPadding * 2 - 20,
                                mLineHeight);
        foreach (cBuildingLine line in mAllHarvestersLines)
        {
            line.SetFrame(framer);
            framer = Utilities.OffsetRectBy(framer, new Vector2(0, mLineHeight + mLineSpacing));
        }
    }


    public float RequiredHeightForWidth(float forWidth)
    {
        var lineCount = mAllHarvestersLines.Count;
        var totalLinesHeight = lineCount * mLineHeight + (lineCount - 1) * mLineSpacing;
        return  mPadding + mTitleSize + mSpaceUnderTitle + mTitleSize + mSpaceUnderTitle + totalLinesHeight + mPadding;
    }


    private void BuildHarvesters()
    {
        var harvesters = GameManager.mRTSManager.mAllHarvesters;
        foreach (HarvestingBuilding building in harvesters)
        {
            var newLine = new cBuildingLine(mGameObject, building.name + "line", building);
            mAllHarvestersLines.Add(newLine);
        }
    }
}
