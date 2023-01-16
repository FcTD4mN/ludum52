using UnityEngine;
using System.Collections.Generic;
using System;

abstract class cBuildPanelBase :
    cPanel
{
    private List<RTSManager.eBuildingList> mBuildingToShow;

    public List<(RTSManager.eBuildingList, cButton)> mButtons;
    public Action<RTSManager.eBuildingList> mOnBuildingClicked;

    // UI variables
    public float mPadding = 32;
    public float mButtonSize = 128;
    public float mSpacing = 12;


    public cBuildPanelBase(GameObject parentView, string name) : base(parentView, name)
    {
        mButtons = new List<(RTSManager.eBuildingList, cButton)>();
        mBuildingToShow = GetBuildingToShowList();
        SetColor(Color.clear);
        BuildButtons();
    }


    abstract internal List<RTSManager.eBuildingList> GetBuildingToShowList();


    override public void LayoutSubviews()
    {
        Rect frame = GetFrame();

        int numberOfButtonPerRow = (int)((frame.width - mPadding * 2) / (float)(mButtonSize + mSpacing));
        if (numberOfButtonPerRow == 0) numberOfButtonPerRow = 1;

        float rowWidth = (float)numberOfButtonPerRow * mButtonSize + ((float)numberOfButtonPerRow - 1f) * mSpacing;

        int i = 0;
        Rect buttonFrame = new Rect(    (frame.width - rowWidth) / 2f,
                                        mPadding,
                                        mButtonSize,
                                        mButtonSize);
        foreach ((RTSManager.eBuildingList, cButton) pair in mButtons)
        {
            int rowIndex = i % numberOfButtonPerRow;
            int columnIndex = i / numberOfButtonPerRow;
            Vector2 offset = new Vector2(rowIndex * (mButtonSize + mSpacing),
                                          columnIndex * (mButtonSize + mSpacing));

            cButton button = pair.Item2;
            button.SetFrame(Utilities.OffsetRectBy(buttonFrame, offset));

            i++;
        }
    }


    public float RequiredHeightForWidth(float forWidth)
    {
        int numberOfButtonPerRow = (int)((forWidth - mPadding * 2) / (mButtonSize + mSpacing));
        if (numberOfButtonPerRow == 0) numberOfButtonPerRow = 1;
        int totalButtonCount = mButtons.Count;

        int numberOfRows = Mathf.CeilToInt((float)totalButtonCount / (float)numberOfButtonPerRow);
        float contentSize = numberOfRows * mButtonSize + (numberOfRows - 1) * mSpacing;

        return mPadding + contentSize + mPadding;
    }


    private void BuildButtons()
    {
        foreach (RTSManager.eBuildingList building in mBuildingToShow)
        {
            string buildingName = building.ToString();
            cButton button = new cButton(mGameObject, buildingName);
            button.AddText(buildingName);
            button.AddOnClickAction(() =>
            {

                mOnBuildingClicked?.Invoke(building);

            });

            mButtons.Add((building, button));

            // Temporary bg color before png images of buildings
            switch (building)
            {
                case RTSManager.eBuildingList.Forge:
                    button.SetColor(new Color(0.63f, 0.38f, 0.65f, 1f));
                    break;
                case RTSManager.eBuildingList.BombFactory:
                    button.SetColor(new Color(0.65f, 0.38f, 0.38f, 1f));
                    break;
                case RTSManager.eBuildingList.Workshop:
                    button.SetColor(new Color(0.38f, 0.39f, 0.65f, 1f));
                    break;
            }
        }

        LayoutSubviews();
    }


    public void UpdateButtons()
    {
        foreach ((RTSManager.eBuildingList, cButton) pair in mButtons)
        {
            cButton button = pair.Item2;
            ProductionBuilding productionBuilding = GameManager.mRTSManager.GetPrefabByType( pair.Item1 );
            button.mButton.interactable = mGameObject.activeSelf && productionBuilding.IsBuildable();
        }
    }
}

