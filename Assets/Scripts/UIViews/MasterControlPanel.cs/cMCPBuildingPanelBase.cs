using UnityEngine;
using System.Collections.Generic;
using System;


abstract class cMCPBuildingPanelBase :
    cPanel,
    pDelegateReceiver
{
    private cLabel mTitle;

    private cLabel mLabelColumnName;
    private cLabel mLabelColumnInputs;
    private cLabel mLabelColumnOutputs;
    private cLabel mLabelColumnEfficiency;

    private WeakReference<cMasterControlPanel> mMaster;
    internal cBuildMenu mBuildMenu; // Required to be member because it needs to be updated


    // UI variables
    public float mPadding = 0;
    public float mButtonSize = 128;
    public float mSpacing = 12;

    private int mTitleSize = 18;
    private int mSpaceUnderTitle = 20;

    private int mColumnLabelSize = 14;

    private int mLineHeight = 30;
    private int mLineSpacing = 5;

    internal List<cBuildingLine> mAllBuildingLines;


    public cMCPBuildingPanelBase(GameObject parentView, string name, string panelName, cMasterControlPanel master ) : base(parentView, name)
    {
        mMaster = new WeakReference<cMasterControlPanel>(master);

        mTitle = new cLabel(mGameObject, "title");
        mTitle.mText.text = panelName;
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

        mLabelColumnEfficiency = new cLabel(mGameObject, "columnEfficiency");
        mLabelColumnEfficiency.mText.text = "Efficiency";
        mLabelColumnEfficiency.mText.fontStyle = TMPro.FontStyles.Bold;
        mLabelColumnEfficiency.mText.fontSize = mColumnLabelSize;
        mLabelColumnEfficiency.mText.alignment = TMPro.TextAlignmentOptions.Left;

        SetColor(Color.clear);

        mAllBuildingLines = new List<cBuildingLine>();
        BuildMissingBuildings();
    }


    public void Update()
    {
        mBuildMenu?.UpdateBuildMenu();
        foreach (cBuildingLine line in mAllBuildingLines)
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
        Rect columnEfficiencyRect = new Rect(columnOutputRect.xMax + cBuildingLine.mBuildingLineSpacing,
                                            columnNameRect.yMin,
                                            cBuildingLine.mBuildingLineEfficiencyWidth,
                                            titleHeight);
        mLabelColumnEfficiency.SetFrame(columnEfficiencyRect);

        var framer = new Rect(mPadding + 20,
                                columnNameRect.yMax + mSpaceUnderTitle,
                                frame.width - mPadding * 2 - 20,
                                mLineHeight);
        foreach (cBuildingLine line in mAllBuildingLines)
        {
            line.SetFrame(framer);
            framer = Utilities.OffsetRectBy(framer, new Vector2(0, mLineHeight + mLineSpacing));
        }
    }


    public float RequiredHeightForWidth(float forWidth)
    {
        var lineCount = mAllBuildingLines.Count;
        var totalLinesHeight = lineCount * mLineHeight + (lineCount - 1) * mLineSpacing;
        return mPadding + mTitleSize + mSpaceUnderTitle + mTitleSize + mSpaceUnderTitle + totalLinesHeight + mPadding;
    }

    abstract internal List<ProductionBuilding> GetBuildingList();
    abstract internal void ActionOnEmptyClick( cBuildingLine line, int spotIndex );
    private void BuildMissingBuildings()
    {
        cMasterControlPanel master;
        mMaster.TryGetTarget(out master);
        if( master == null ) return;

        var buildings = GetBuildingList();
        var howManyBuildingLineToCreate = buildings.Count - mAllBuildingLines.Count;

        for (int j = buildings.Count - howManyBuildingLineToCreate; j < buildings.Count; j++)
        {
            var building = buildings[j];
            var newLine = new cBuildingLine(mGameObject, "buildingLine" + j, building, master);

            int indexHere = j; // Copy to capture in Action
            newLine.mOnClickEmptyEvent = (buildingLine) =>
            {
                ActionOnEmptyClick(buildingLine, indexHere);
            };

            mAllBuildingLines.Add(newLine);

        }
    }


    private void BuildLevelUpLines()
    {
        var buildings = GetBuildingList();

    }


    // ===================================
    // Delegate
    // ===================================

    abstract internal bool ShouldPerformAction( pDelegateSender sender );
    virtual public void Action(pDelegateSender sender, object[] args)
    {
        if( !ShouldPerformAction( sender ) ) return;

        var message = args[0];
        switch (message)
        {
            case TowerBase.eTowerMessages.kBuildingAdded:
                {
                    var building = (ProductionBuilding)args[1];
                    var index = (int)args[2];

                    var line = mAllBuildingLines[index];
                    line.BuildUIForBuilding(building);

                    break;
                }

            case TowerBase.eTowerMessages.kBuildingRemoved:
                {
                    var index = (int)args[2];

                    var line = mAllBuildingLines[index];

                    // Test because this could be called on dead panel as garbage collector takes a long time to process
                    if (line.mGameObject == null) return;

                    line.BuildUIForBuilding(null);

                    break;
                }

            case TowerBase.eTowerMessages.kLevelUp:
                {
                    cMasterControlPanel master;
                    mMaster.TryGetTarget(out master);
                    if (master == null) return;

                    BuildMissingBuildings();
                    master.LayoutSubviews();
                    break;
                }

            default:
                break;
        }
    }
}
