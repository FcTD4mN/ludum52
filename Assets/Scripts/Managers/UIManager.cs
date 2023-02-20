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
    private cBuildMenu mBuildMenu;

    // Info panel
    private GameObject mInfoPanel;
    private TextMeshProUGUI mInfoPanelText;

    // Master Control Panel
    private cMasterControlPanel mMasterControlPanel;

    // Tooltip panel
    private GameObject mTooltipPanel;
    private TextMeshProUGUI mTooltipPanelText;
    private Coroutine mTooltipCloseCoroutine;

    private GameObject mObjectToBuildTo;
    private GameObject mBuildButtonClicked;
    private GameObject mHoverButton; // Button being visible due to hovering
    private Hoverable mHoverUIView; // Button being visible due to hovering
    private GameObject mHoveredObject; // The object the mouse is currently hovering
    private bool mDidFindHoveredButton = false;


    public List<GameObject> mBuildableObjects;

                // UIButton, BuildableAssociated (could be null)
    private List<(GameObject, GameObject)> mAllUIFloatingButtons; // All UI buttons to click to open build menu


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
        mBuildMenu = new cBuildMenu( mCanvas.gameObject, "BuildMenu" );
        Rect screenRect = Camera.main.pixelRect;
        mBuildMenu.SetFrame(new Rect(0, 0, 500, 500));
        mBuildMenu.SetCenter(screenRect.center);
        mBuildMenu.mGameObject.SetActive( false );
        BuildBuildMenu();

        // Info Panel
        mInfoPanel = mCanvas.transform.Find("InfoPanel")?.gameObject;
        mInfoPanelText = GameObject.Find("InfoPanelText")?.gameObject.GetComponent<TextMeshProUGUI>();
        BuildInfoPanel();

        // Info Panel
        mTooltipPanel = mCanvas.transform.Find("ToolTipPanel")?.gameObject;
        mTooltipPanelText = mTooltipPanel.transform.Find("text")?.gameObject.GetComponent<TextMeshProUGUI>();
        mTooltipPanelText.text = "ok";
        mTooltipPanel.SetActive( false );

        mAllUIFloatingButtons = new List<(GameObject, GameObject)>();

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

        mBuildMenu?.UpdateBuildMenu();
        mMasterControlPanel?.Update();
        UpdateHovering();
    }


    public void UpdateHovering()
    {
        GameObject previousHovered = mHoveredObject;
        mDidFindHoveredButton = false;

        // If nothing is under the pointer, just clear everything and refresh infoPanel
        // if (!EventSystem.current.IsPointerOverGameObject()) // Can't use that to optimize, because you want to hover over WORLD objects as well
        //                                                                      And this only looks at UI elements

        // General UI hover tests
        var raycasts = RaycastMouse();
        var previousHoveredUI = mHoverUIView;
        foreach (var result in raycasts)
        {
            var hoverable = result.gameObject.GetComponent<Hoverable>();
            if (hoverable != null)
            {
                mHoverUIView = hoverable;
                hoverable.mOnHoverAction();
                mDidFindHoveredButton = true;
                break;
            }
        }

        // We didn't find any view in UI, so if there was a hoveredUIView, we call hoverEnded then null the pointer
        if( !mDidFindHoveredButton || previousHoveredUI != mHoverUIView )
        {
            previousHoveredUI?.mOnHoverEndedAction?.Invoke();
            if( !mDidFindHoveredButton ) mHoverUIView = null;
        }

        // Test to prevent hovering on elements while UI windows are open
        if( !mBuildMenu.mGameObject.activeSelf && mMasterControlPanel == null )
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
        mBuildMenu.mGameObject.SetActive(false);
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
        GameObject newButton = CreateButtonOverObject( "ButtonBuild", obj );
        mAllUIFloatingButtons.Add( (newButton, obj ) );
        newButton.GetComponent<Button>().onClick.AddListener( () => {

            mObjectToBuildTo = obj;
            mBuildButtonClicked = newButton;
            ShowBuildMenu();

        });
    }


    private void CreatePauseButtonOverBuilding( GameObject building )
    {
        mHoverButton = CreateButtonOverObject( "ButtonPause", building );
        GameObject text = mHoverButton.transform.Find("text")?.gameObject;

        ProductionBuilding prodBuilding = building.GetComponent<ProductionBuilding>();
        if( building.GetComponent<Receiver>() != null )
        {
            prodBuilding = building.GetComponent<Receiver>().mAssociatedHarvester.GetComponent<ProductionBuilding>();
        }

        text.GetComponent<TextMeshProUGUI>().text = prodBuilding.IsPaused() ? "Resume" : "Pause";

        GameObject deleteButton = mHoverButton.transform.Find("Delete")?.gameObject;
        deleteButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            GameManager.mRTSManager.DestroyBuilding( building );
            DeleteUIButton( mHoverButton );

        });

        mHoverButton.GetComponent<Button>().onClick.AddListener( () => {

            prodBuilding.SetPause( !prodBuilding.IsPaused() );
            UpdateInfoPanel();
            text.GetComponent<TextMeshProUGUI>().text = prodBuilding.IsPaused() ? "Resume" : "Pause";

        });
    }


    private GameObject CreateButtonOverObject( string prefabName, GameObject obj )
    {
        GameObject buttonPrefab = Resources.Load<GameObject>("Prefabs/RTS/UI/" + prefabName);

        Rect objectBBox = Utilities.GetBBoxFromTransform(obj);
        Rect screenOjbBBox = Utilities.WorldToScreenRect( objectBBox );

        GameObject newButton = Instantiate( buttonPrefab,
                                            new Vector3( screenOjbBBox.center.x, screenOjbBBox.center.y, 0 ),
                                            Quaternion.Euler(0, 0, 0) );
        newButton.transform.SetParent( mCanvas.transform );
        float buttongPadding = 10;
        newButton.GetComponent<RectTransform>().sizeDelta = new Vector2( screenOjbBBox.width - buttongPadding, screenOjbBBox.height - buttongPadding );

        return  newButton;
    }


    public void DeleteAllUIFloatingButtons()
    {
        foreach( var pair in mAllUIFloatingButtons )
        {
            var button = pair.Item1;
            GameObject.Destroy( button );
        }

        mAllUIFloatingButtons.Clear();
    }


    public void DeleteUIButton(GameObject uiButton)
    {
        mAllUIFloatingButtons.RemoveAll((pair) => { return pair.Item1 == uiButton; });
        GameObject.Destroy(uiButton);
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

                    // If mcp is opened, put it front, because we just created new UIbuttons, they will be over mcp if it is open
                    if( mMasterControlPanel != null )
                    {
                        mMasterControlPanel.mGameObject.transform.SetAsLastSibling();
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

        if( isLocationOnTower ) mBuildMenu.ShowProdBuildingPanel();
        if( isLocationOnBuffTower ) mBuildMenu.ShowBuffBuildingPanel();
        if (isLocationOnWeaponTower) mBuildMenu.ShowWeaponBuildingPanel();

        mBuildMenu.UpdateBuildMenu();
        mBuildMenu.mGameObject.transform.SetAsLastSibling();
        mBuildMenu.mGameObject.SetActive( true );
    }


    private void BuildBuildMenu()
    {
        mBuildMenu.mOnClose = () =>
        {
            mBuildMenu.mGameObject.SetActive(false);
        };


        mBuildMenu.mOnBuildingClicked = (building)=> {

            mBuildMenu.mGameObject.SetActive(false);
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
        // Hitting P when panel is open
        if( mMasterControlPanel != null )
        {
            GameObject.Destroy( mMasterControlPanel.mGameObject );
            mMasterControlPanel = null;

            return;
        }

        mMasterControlPanel = new cMasterControlPanel( mCanvas.gameObject, "MCP" );
        mMasterControlPanel.mCloseAction = () => {
            GameObject.Destroy(mMasterControlPanel.mGameObject);
            mMasterControlPanel = null;
        };
        var screenRect = Camera.main.pixelRect;

        mMasterControlPanel.SetFrame( new Rect(0, 0, 1600, 800));
        mMasterControlPanel.SetCenter( screenRect.center );
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
        Animation anim = new Animation( this );
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
