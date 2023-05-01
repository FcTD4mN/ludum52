using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour,
                        pDelegateReceiver
{
    private ResourceManager mResourceManager;
    public Canvas mCanvas;

    private TextMeshProUGUI mLabelGold;
    private TextMeshProUGUI mLabelIron;
    private TextMeshProUGUI mLabelFire;
    private TextMeshProUGUI mLabelArrows;
    private TextMeshProUGUI mLabelBombs;

    // BuildMenu
    private cBuildMenuIMGUI mBuildMenu;

    // Info panel
    private GameObject mInfoPanel;
    private TextMeshProUGUI mInfoPanelText;

    // Master Control Panel
    private cMasterControlPanelIMGUI mMasterControlPanel;

    // Tooltip panel
    private GameObject mTooltipPanel;
    private TextMeshProUGUI mTooltipPanelText;
    private Coroutine mTooltipCloseCoroutine;

    private GameObject mObjectToBuildTo;
    private cFloatingButton mBuildButtonClicked;
    private cFloatingButton mHoverButton; // Button being visible due to hovering
    // private Hoverable mHoverUIView; // Button being visible due to hovering
    private GameObject mHoveredObject; // The object the mouse is currently hovering
    private bool mDidFindHoveredButton = false;


    public List<GameObject> mBuildableObjects;

                // UIButton, BuildableAssociated (could be null)
    private List<(cFloatingButton, GameObject)> mAllUIFloatingButtons; // All UI buttons to click to open build menu


    private enum eInfoPanelDisplayType
    {
        kShowStatsOfExistingBuilding,
        kShowPrefabStats
    }
    private eInfoPanelDisplayType mInfoPanelDisplay;

    // ===================================
    // Building
    // ===================================
    public void Initialize()
    {
        mResourceManager = GameManager.mResourceManager;

        mLabelGold = GameObject.Find("LabelGold")?.gameObject.GetComponent<TextMeshProUGUI>();
        mLabelIron = GameObject.Find("LabelIron")?.gameObject.GetComponent<TextMeshProUGUI>();
        mLabelFire = GameObject.Find("LabelFire")?.gameObject.GetComponent<TextMeshProUGUI>();
        mLabelArrows = GameObject.Find("LabelArrows")?.gameObject.GetComponent<TextMeshProUGUI>();
        mLabelBombs = GameObject.Find("LabelBombs")?.gameObject.GetComponent<TextMeshProUGUI>();

        mCanvas = GameObject.Find("UI-RTS")?.gameObject.GetComponent<Canvas>();

        mBuildableObjects = new List<GameObject>();
        foreach( var tower in GameManager.mRTSManager.mAllTowers )
        {
            BuildBuildableList( tower.mTowerNode );
            ((pDelegateSender)tower).AddDelegate(this);
        }

        // Build Menu
        Rect screenRect = Camera.main.pixelRect;
        mBuildMenu = new cBuildMenuIMGUI( "BuildMenu", new Rect(screenRect.center.x - 250, screenRect.center.y - 250, 500, 500) );
        mBuildMenu.mIsOpen = false;
        BuildBuildMenu();

        mMasterControlPanel = new cMasterControlPanelIMGUI("MCP", new Rect(screenRect.center.x - 800, screenRect.center.y - 400, 1600, 800));
        mMasterControlPanel.mIsOpen = false;

        // Info Panel
        mInfoPanel = mCanvas.transform.Find("InfoPanel")?.gameObject;
        mInfoPanelText = GameObject.Find("InfoPanelText")?.gameObject.GetComponent<TextMeshProUGUI>();
        BuildInfoPanel();

        // Info Panel
        mTooltipPanel = mCanvas.transform.Find("ToolTipPanel")?.gameObject;
        mTooltipPanelText = mTooltipPanel.transform.Find("text")?.gameObject.GetComponent<TextMeshProUGUI>();
        mTooltipPanelText.text = "ok";
        mTooltipPanel.SetActive( false );

        mAllUIFloatingButtons = new List<(cFloatingButton, GameObject)>();

    }


    private void BuildBuildableList( GameObject root )
    {
        foreach( Transform child in root.transform )
        {
            foreach (Transform subChild in child )
            {
                if( subChild.tag != "Buildable" ) {
                    continue;
                }

                mBuildableObjects.Add( subChild.gameObject );
            }

            if( child.tag != "Buildable" ) {
                continue;
            }

            mBuildableObjects.Add( child.gameObject );
        }
    }


    // ===================================
    // Update
    // ===================================
    public void UpdateUI()
    {
        mLabelGold.text = ((int)mResourceManager.GetRessource(cResourceDescriptor.eResourceNames.Gold)).ToString();
        mLabelIron.text = ((int)mResourceManager.GetRessource(cResourceDescriptor.eResourceNames.Iron)).ToString();
        mLabelFire.text = ((int)mResourceManager.GetRessource(cResourceDescriptor.eResourceNames.Fire)).ToString();
        mLabelArrows.text = ((int)mResourceManager.GetRessource(cResourceDescriptor.eResourceNames.Arrows)).ToString();
        mLabelBombs.text = ((int)mResourceManager.GetRessource(cResourceDescriptor.eResourceNames.Bombs)).ToString();

        UpdateHovering();
    }


    public void UpdateHovering()
    {
        GameObject previousHovered = mHoveredObject;
        mDidFindHoveredButton = false;

        // Test to prevent hovering on elements while UI windows are open
        if( !mBuildMenu.mIsOpen && !mMasterControlPanel.mIsOpen )
        {
            Vector3 mousePosScreen = Input.mousePosition;
            Vector3 mousePosWorld;
            RectTransformUtility.ScreenPointToWorldPointInRectangle(mCanvas.transform as RectTransform, mousePosScreen, Camera.main, out mousePosWorld);

            // PROD BUILDINGS (world buildings, can't be tested through raycast above)
            if (!mDidFindHoveredButton)
            {
                foreach( ProductionBuilding building in GameManager.mRTSManager.mAllProductionBuildings )
                {
                    if( building.gameObject.GetComponent<HarvestingBuilding>() != null ) { continue; }

                    Rect bbox = Utilities.GetBBoxFromTransform( building.gameObject );
                    if( bbox.Contains( mousePosWorld ) )
                    {
                        HoveringBuilding( building );
                        mDidFindHoveredButton = true;
                        break;
                    }
                }
            }

            // RECEIVERS (world buildings, can't be tested through raycast above)
            if( !mDidFindHoveredButton )
            {
                foreach (Receiver receiver in GameManager.mRTSManager.mMainTower.mAllReceivers)
                {
                    Rect bbox = Utilities.GetBBoxFromTransform(receiver.gameObject);
                    if (bbox.Contains(mousePosWorld))
                    {
                        HoveringReceiver(receiver.gameObject);
                        mDidFindHoveredButton = true;
                        break;
                    }
                }
            }
        }


        if( !mDidFindHoveredButton ) mHoveredObject = null;
        if( mHoveredObject == null && mHoverButton != null )
        {
            DeleteUIButton( mHoverButton );
            mHoverButton = null;
        }

        UpdateInfoPanel();
    }


    // ===================================
    // UI Stuff
    // ===================================
    public void ClearUIForSwitchingView()
    {
        DeleteAllUIFloatingButtons();
        mBuildMenu.mIsOpen = false;
        mInfoPanel.SetActive(false);

        if (mHoverButton != null) DeleteUIButton(mHoverButton);
    }


    public void CreateBuildButtonOnEveryBuildableObject()
    {
        foreach( GameObject buildable in mBuildableObjects )
        {
            if( !buildable.activeSelf ) { continue; }

            CreateBuildButtonOverObject( buildable );
        }
    }


    public void CreateBuildButtonOverObject( GameObject obj )
    {
        cFloatingButton newButton = CreateButtonOverObject( "Build", obj );
        mAllUIFloatingButtons.Add( (newButton, obj ) );
    }


    private void CreatePauseButtonOverBuilding( GameObject building )
    {
        ProductionBuilding prodBuilding = building.GetComponent<ProductionBuilding>();
        if (building.GetComponent<Receiver>() != null)
        {
            prodBuilding = building.GetComponent<Receiver>().mAssociatedHarvester.GetComponent<ProductionBuilding>();
        }

        Rect objectBBox = Utilities.GetBBoxFromTransform(building);
        Rect screenOjbBBox = Utilities.WorldToScreenRect(objectBBox);
        screenOjbBBox.position = new Vector2(screenOjbBBox.x, Camera.main.pixelHeight - screenOjbBBox.y - screenOjbBBox.height);

        var newButton = new cFloatingButtonPauseResume( "Pause", screenOjbBBox);
        newButton.mButtonLabel = prodBuilding.IsPaused() ? "Resume" : "Pause";
        newButton.mHoverText = prodBuilding.IsPaused() ? "Paused" : prodBuilding.mResourceDescriptor.PrintProductionRates();

        newButton.mOnButtonClicked = () => {
            prodBuilding.SetPause(!prodBuilding.IsPaused());
            newButton.mButtonLabel = prodBuilding.IsPaused() ? "Resume" : "Pause";
            newButton.mHoverText = prodBuilding.IsPaused() ? "Paused" : prodBuilding.mResourceDescriptor.PrintProductionRates();
        };
        newButton.mOnDeleteClicked = () => {
            GameManager.mRTSManager.DestroyBuilding( building );
            DeleteUIButton( mHoverButton );
        };
        newButton.mOnHover = () =>
        {
            mInfoPanelDisplay = eInfoPanelDisplayType.kShowStatsOfExistingBuilding;
            UpdateInfoPanel();
        };

        mHoverButton = newButton;
    }


    private cFloatingButton CreateButtonOverObject( string buttonLabel, GameObject obj )
    {
        Rect objectBBox = Utilities.GetBBoxFromTransform(obj);
        Rect screenOjbBBox = Utilities.WorldToScreenRect( objectBBox );
        screenOjbBBox.position = new Vector2( screenOjbBBox.x, Camera.main.pixelHeight - screenOjbBBox.y - screenOjbBBox.height ) ;

        var button = new cFloatingButton( buttonLabel, screenOjbBBox );
        button.mOnButtonClicked = () => {
            mObjectToBuildTo = obj;
            mBuildButtonClicked = button;
            ShowBuildMenu();
        };

        return  button;
    }


    public void DeleteAllUIFloatingButtons()
    {
        foreach( var pair in mAllUIFloatingButtons )
        {
            var button = pair.Item1;
            button.DestroyWindow();
        }

        mAllUIFloatingButtons.Clear();
    }


    public void DeleteUIButton(cFloatingButton uiButton)
    {
        mAllUIFloatingButtons.RemoveAll((pair) => { return pair.Item1 == uiButton; });
        uiButton?.DestroyWindow();
    }

    public void DeleteUIButtonAssociatedToBuildable(GameObject buildableObject)
    {
        var pair = mAllUIFloatingButtons.Find( (pair) => { return pair.Item2 == buildableObject; } );
        DeleteUIButton( pair.Item1 );
    }


    // ===================================
    // Delegate
    // ===================================
    public void Action( pDelegateSender sender, object[] args )
    {
        if( sender is TowerBase towerBase )
        {
            TowerMessage( towerBase, args );
        }
    }


    private void TowerMessage( TowerBase sender, object[] args)
    {
        var message = args[0];
        switch (message)
        {
            case TowerBase.eTowerMessages.kBuildingAdded:
                {
                    var buildableObject = (GameObject)args[3];
                    DeleteUIButtonAssociatedToBuildable( buildableObject );
                    break;
                }

            case TowerBase.eTowerMessages.kBuildingRemoved:
                {
                    var building = (ProductionBuilding)args[1];
                    if (building.GetComponent<Receiver>() == null)
                    {
                        // We reactivate the buildable object + reshow a build button
                        foreach (GameObject obj in mBuildableObjects)
                        {
                            if (obj.transform.position == building.transform.position)
                            {
                                CreateBuildButtonOverObject(obj);
                                break;
                            }
                        }
                    }

                    break;
                }

            case TowerBase.eTowerMessages.kLevelUp:
                {
                    var addedFloor = sender.mFloors[ sender.mLevel - 1 ];
                    foreach (Transform gg in addedFloor.transform)
                    {
                        mBuildableObjects.Add(gg.gameObject);
                        CreateBuildButtonOverObject(gg.gameObject);
                    }
                    break;
                }

            default:
                break;
        }
    }


    // ===================================
    // Mouse Hovering
    // ===================================
    private void HoveringBuilding(ProductionBuilding building)
    {
        if (mHoveredObject == building.gameObject && mHoverButton != null) return;
        if (mHoveredObject != building.gameObject) DeleteUIButton(mHoverButton);

        mHoveredObject = building.gameObject;
        mInfoPanelDisplay = eInfoPanelDisplayType.kShowStatsOfExistingBuilding;

        CreatePauseButtonOverBuilding(building.gameObject);
    }


    private void HoveringReceiver(GameObject receiver)
    {
        GameObject associatedReceiverObject = receiver.GetComponent<Receiver>().mAssociatedHarvester.gameObject;
        if (mHoveredObject == associatedReceiverObject && mHoverButton != null) return;
        if (mHoveredObject != associatedReceiverObject) DeleteUIButton(mHoverButton);

        mHoveredObject = associatedReceiverObject;
        mInfoPanelDisplay = eInfoPanelDisplayType.kShowStatsOfExistingBuilding;

        CreatePauseButtonOverBuilding( receiver );
    }


    // ===================================
    // Build Menu
    // ===================================
    public void ShowBuildMenu()
    {
        bool isLocationOnTower = GameManager.mRTSManager.mMainTower.mFloors.Contains(mObjectToBuildTo.transform.parent.gameObject);
        bool isLocationOnBuffTower = GameManager.mRTSManager.mBuffTower.mFloors.Contains(mObjectToBuildTo.transform.parent.gameObject);
        bool isLocationOnWeaponTower = GameManager.mRTSManager.mTowerWeapon.mFloors.Contains(mObjectToBuildTo.transform.parent.gameObject);

        mBuildMenu.mIsOpen = true;

        if( isLocationOnTower ) mBuildMenu.mVisiblePanel = cBuildMenuIMGUI.eVisiblePanel.kProd;
        if( isLocationOnBuffTower ) mBuildMenu.mVisiblePanel = cBuildMenuIMGUI.eVisiblePanel.kBuff;
        if (isLocationOnWeaponTower) mBuildMenu.mVisiblePanel = cBuildMenuIMGUI.eVisiblePanel.kWeapon;
    }


    private void BuildBuildMenu()
    {
        mBuildMenu.mOnBuildingClicked = (building)=> {

            mBuildMenu.mIsOpen = false;
            GameManager.mRTSManager.BuildObjectAtLocation( building.ToString(), mObjectToBuildTo);

        };
    }

    // ===================================
    // Info Panel
    // ===================================
    private void BuildInfoPanel()
    {
        mInfoPanel.SetActive( false );
    }

    private void UpdateInfoPanel()
    {
        mInfoPanel.SetActive(mHoveredObject != null);

        if( mHoveredObject != null )
        {
            // PROD BUILDING
            ProductionBuilding prodBuilding = mHoveredObject.GetComponent<ProductionBuilding>();
            if( prodBuilding == null ) return;

            switch (mInfoPanelDisplay)
            {
                case eInfoPanelDisplayType.kShowStatsOfExistingBuilding:
                    mInfoPanelText.text = prodBuilding.IsPaused() ? "Paused" : prodBuilding.mResourceDescriptor.PrintProductionRates();
                    break;

                case eInfoPanelDisplayType.kShowPrefabStats:
                    mInfoPanelText.text = prodBuilding.GetUIDescription( true );
                    break;

                default:
                    break;
            }
        }
    }


    // ===================================
    // Master control panel
    // ===================================
    public void BuildControlPanel()
    {
        mMasterControlPanel.mIsOpen = !mMasterControlPanel.mIsOpen;
    }


    public void DisplayMessage( string message, float duration )
    {
        if( mTooltipCloseCoroutine != null ){
            StopCoroutine( mTooltipCloseCoroutine );
        }
        mTooltipCloseCoroutine = null;

        mTooltipPanelText.text = message;
        mTooltipPanel.SetActive( true );

        mTooltipCloseCoroutine = StartCoroutine( Utilities.ExecuteAfter( duration, () => {
            mTooltipPanel.SetActive( false );
            mTooltipCloseCoroutine = null;
         } ));
    }

    public void FloatingMessage( string message, Color color, Vector3 position )
    {
        cAnimation anim = new cAnimation( this );
        anim.DisplayAnimatedText( position, message, color, 2, 30, mCanvas.transform );
    }

    public List<RaycastResult> RaycastMouse()
    {
        var graphicRaycaster = mCanvas.GetComponent<GraphicRaycaster>();
        var eventSystem = mCanvas.GetComponent<EventSystem>();
        PointerEventData pointerData = new PointerEventData(eventSystem);
        pointerData.position = Input.mousePosition;

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerData, results);
        return results;
    }
}
