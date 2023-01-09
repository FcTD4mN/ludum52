using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using UnityEngine.EventSystems;

public class UIManager : MonoBehaviour
{
    private ResourceManager mResourceManager;
    public Canvas mCanvas;

    private TextMeshProUGUI mLabelGold;
    private TextMeshProUGUI mLabelIron;
    private TextMeshProUGUI mLabelFire;
    private TextMeshProUGUI mLabelArrows;
    private TextMeshProUGUI mLabelBombs;

    // BuildMenu
    private GameObject mBuildMenu;

    private Button mButtonForge;
    private Button mButtonBombFactory;
    private Button mButtonDamage;
    private Button mButtonCooldown;
    private Button mButtonSpeed;
    private Button mButtonJump;

    // Info panel
    private GameObject mInfoPanel;
    private TextMeshProUGUI mInfoPanelText;

    // Tooltip panel
    private GameObject mTooltipPanel;
    private TextMeshProUGUI mTooltipPanelText;
    private Coroutine mTooltipCloseCoroutine;

    private GameObject mObjectToBuildTo;
    private GameObject mBuildButtonClicked;
    private GameObject mHoverButton; // Button being visible due to hovering
    private GameObject mHoveredObject; // The object the mouse is currently hovering
    public List<GameObject> mBuildableObjects;
    private List<GameObject> mBuildButtons; // All UI buttons to click to build buildings
    private List<GameObject> mAllUIFloatingButtons; // All UI buttons to click to open build menu


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
        BuildBuildableList();

        // Build Menu
        mBuildMenu = mCanvas.transform.Find("BuildMenu")?.gameObject;
        mButtonDamage = GameObject.Find("ButtonDamage")?.gameObject.GetComponent<Button>();
        mButtonCooldown = GameObject.Find("ButtonCooldown")?.gameObject.GetComponent<Button>();
        mButtonSpeed = GameObject.Find("ButtonSpeed")?.gameObject.GetComponent<Button>();
        mButtonJump = GameObject.Find("ButtonJump")?.gameObject.GetComponent<Button>();
        mButtonForge = GameObject.Find("ButtonForge")?.gameObject.GetComponent<Button>();
        mButtonBombFactory = GameObject.Find("ButtonBombFactory")?.gameObject.GetComponent<Button>();
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

        mBuildButtons = new List<GameObject>();
        mBuildButtons.Add( mButtonDamage.gameObject );
        mBuildButtons.Add( mButtonCooldown.gameObject );
        mBuildButtons.Add( mButtonSpeed.gameObject );
        mBuildButtons.Add( mButtonJump.gameObject );
        mBuildButtons.Add( mButtonForge.gameObject );
        mBuildButtons.Add( mButtonBombFactory.gameObject );

        mAllUIFloatingButtons = new List<GameObject>();
    }


    private void BuildBuildableList()
    {
        mBuildableObjects = new List<GameObject>();
        foreach( Transform child in GameManager.mRTSManager.mRTSWorld.transform )
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

        UpdateBuildMenu();
        UpdateMousePosition();
    }


    public void UpdateMousePosition()
    {
        Vector3 mousePosScreen = Input.mousePosition;
        Vector3 mousePosWorld;
        RectTransformUtility.ScreenPointToWorldPointInRectangle( mCanvas.transform as RectTransform, mousePosScreen, Camera.main, out mousePosWorld );

        // PROD BUILDINGS
        foreach( ProductionBuilding building in GameManager.mRTSManager.mAllProductionBuildings )
        {
            if( building.gameObject.GetComponent<HarvestingBuilding>() != null ) { continue; }

            Rect bbox = Utilities.GetBBoxFromTransform( building.gameObject );
            if( bbox.Contains( mousePosWorld ) )
            {
                HoveringBuilding( building );
                return;
            }
        }

        // RECEIVERS
        foreach ((GameObject, int) pack in GameManager.mRTSManager.mAllReceivers)
        {
            GameObject receiver = pack.Item1;
            Rect bbox = Utilities.GetBBoxFromTransform(receiver);
            if (bbox.Contains(mousePosWorld))
            {
                HoveringReceiver(receiver);
                return;
            }
        }

        // BUILDABLE
        foreach( GameObject buildable in mBuildableObjects )
        {
            Rect bbox = Utilities.GetBBoxFromTransform( buildable );
            if( bbox.Contains( mousePosWorld ) )
            {
                HoveringBuildable( buildable );
                return;
            }
        }

        // BUILDINGS IN BUILD MENU
        if( EventSystem.current.IsPointerOverGameObject() )
        {
            PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
            eventDataCurrentPosition.position = new Vector2(mousePosScreen.x, mousePosScreen.y);
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(eventDataCurrentPosition, results);

            for (int i = 0; i < results.Count; i++)
            {
                if ( mBuildButtons.Contains( results[i].gameObject ) )
                {
                    HoveringBuildingDescription( results[i].gameObject );
                    return;
                }
            }
        }

        if( mHoverButton != null ) {
            DeleteUIButton( mHoverButton );
        }

        mInfoPanel.SetActive( false );
    }


    // ===================================
    // UI Stuff
    // ===================================
    public void ClearUIForSwitchingView()
    {
        DeleteAllUIFloatingButtons();
        mBuildMenu.SetActive(false);
        mInfoPanel.SetActive(false);

        if (mHoverButton != null)
        {
            DeleteUIButton(mHoverButton);
        }
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
        mAllUIFloatingButtons.Add( newButton );
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

        bool isReceiver = false;
        ProductionBuilding prodBuilding = building.GetComponent<ProductionBuilding>();
        if( building.GetComponent<Receiver>() != null )
        {
            isReceiver = true;
            prodBuilding = building.GetComponent<Receiver>().mAssociatedHarvester.GetComponent<ProductionBuilding>();
        }

        text.GetComponent<TextMeshProUGUI>().text = prodBuilding.IsPaused() ? "Resume" : "Pause";

        GameObject deleteButton = mHoverButton.transform.Find("Delete")?.gameObject;
        deleteButton.GetComponent<Button>().onClick.AddListener(() =>
        {

            if( !isReceiver ) // We reactivate the buildable object + reshow a build button
            {
                foreach( GameObject obj in mBuildableObjects )
                {
                    if( obj.transform.position == prodBuilding.transform.position )
                    {
                        CreateBuildButtonOverObject( obj );
                        break;
                    }
                }
            }
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
        foreach( GameObject button in mAllUIFloatingButtons )
        {
            GameObject.Destroy( button );
        }

        mAllUIFloatingButtons.Clear();
    }


    public void DeleteUIButton( GameObject obj )
    {
        mAllUIFloatingButtons.Remove( obj );
        GameObject.Destroy( obj );
    }


    // ===================================
    // Mouse Hovering
    // ===================================
    private void HoveringBuilding(ProductionBuilding building)
    {
        if (mHoveredObject == building.gameObject && mHoverButton != null)
        {
            return;
        }

        if (mHoveredObject != building.gameObject)
        {
            DeleteUIButton(mHoverButton);
        }

        mHoveredObject = building.gameObject;
        CreatePauseButtonOverBuilding(building.gameObject);
        UpdateInfoPanel();
    }


    private void HoveringReceiver(GameObject receiver)
    {
        GameObject associatedReceiverObject = receiver.GetComponent<Receiver>().mAssociatedHarvester.gameObject;
        if (mHoveredObject == associatedReceiverObject && mHoverButton != null)
        {
            return;
        }

        if (mHoveredObject != associatedReceiverObject)
        {
            DeleteUIButton(mHoverButton);
        }

        mHoveredObject = associatedReceiverObject;
        CreatePauseButtonOverBuilding( receiver );
        UpdateInfoPanel();
    }


    private void HoveringBuildable( GameObject resource )
    {
        if( mHoveredObject == resource && mHoverButton != null ) {
            return;
        }

        if( mHoveredObject != resource ) {
            DeleteUIButton(mHoverButton);
        }

        mHoveredObject = resource;
    }


    private void HoveringBuildingDescription( GameObject button )
    {
        if( mHoveredObject == button && mHoverButton != null ) {
            return;
        }

        if( mHoveredObject != button ) {
            DeleteUIButton(mHoverButton);
        }

        mHoveredObject = button;
        UpdateInfoPanel();
    }


    // ===================================
    // Build Menu
    // ===================================
    public void ShowBuildMenu()
    {
        UpdateBuildMenu();
        mBuildMenu.transform.SetAsLastSibling();
        mBuildMenu.SetActive( true );
    }


    private void UpdateBuildMenu()
    {
        if( !mBuildMenu.activeSelf ) { return; }

        bool isLocationOnTower = GameManager.mRTSManager.mTowers.Contains(mObjectToBuildTo.transform.parent.gameObject );
        bool isLocationOnBuffTower = GameManager.mRTSManager.mBuffTower.Contains(mObjectToBuildTo.transform.parent.gameObject);

        mButtonDamage.interactable = BuffBuildingDamage.IsBuildable() && isLocationOnBuffTower;
        mButtonCooldown.interactable = BuffBuildingCooldown.IsBuildable() && isLocationOnBuffTower;

        mButtonForge.interactable = Forge.IsBuildable() && isLocationOnTower;
        mButtonBombFactory.interactable = BombFactory.IsBuildable() && isLocationOnTower;
    }


    private void BuildBuildMenu()
    {
        mBuildMenu.SetActive( false );

        mBuildMenu.transform.Find("Close")?.gameObject.GetComponent<Button>().onClick.AddListener( () => {
            mBuildMenu.SetActive( false );
        });

        mButtonDamage.onClick.AddListener( () => {
            mBuildMenu.SetActive( false );
            GameManager.mRTSManager.BuildBuffBuildingAtLocation( "BuffBuildingDamage", mObjectToBuildTo );
            DeleteUIButton(mBuildButtonClicked);
        });

        mButtonCooldown.onClick.AddListener( () => {
            mBuildMenu.SetActive( false );
            GameManager.mRTSManager.BuildBuffBuildingAtLocation( "BuffBuildingCooldown", mObjectToBuildTo );
            DeleteUIButton(mBuildButtonClicked);
        });

        mButtonForge.onClick.AddListener( () => {
            mBuildMenu.SetActive( false );
            GameManager.mRTSManager.BuildObjectAtLocation( "BuildingForge", mObjectToBuildTo );
            DeleteUIButton(mBuildButtonClicked);
        });

        mButtonBombFactory.onClick.AddListener( () => {
            mBuildMenu.SetActive( false );
            GameManager.mRTSManager.BuildObjectAtLocation( "BuildingBombFactory", mObjectToBuildTo );
            DeleteUIButton(mBuildButtonClicked);
        });

        mButtonSpeed.onClick.AddListener(() =>
        {
            mBuildMenu.SetActive(false);
            GameManager.mRTSManager.BuildObjectAtLocation("BuffBuildingSpeed", mObjectToBuildTo);
            DeleteUIButton(mBuildButtonClicked);
        });

        mButtonJump.onClick.AddListener(() =>
        {
            mBuildMenu.SetActive(false);
            GameManager.mRTSManager.BuildObjectAtLocation("BuffBuildingJump", mObjectToBuildTo);
            DeleteUIButton(mBuildButtonClicked);
        });

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
        mInfoPanel.SetActive( false );
        if( mHoveredObject == null ) { return; }

        // PROD BUILDING
        ProductionBuilding prodBuilding = mHoveredObject.GetComponent<ProductionBuilding>();
        if( prodBuilding != null )
        {
            mInfoPanelText.text = prodBuilding.IsPaused() ? "Paused" : prodBuilding.mResourceDescriptor.PrintProductionRates();
            mInfoPanel.SetActive( true );
            return;
        }

        // UI BUTTON
        if( mHoveredObject == mButtonDamage.gameObject )
        {
            mInfoPanelText.text = BuffBuildingDamage.GetUIDescription(mButtonDamage.interactable);
            mInfoPanel.SetActive( true );
        }
        else if( mHoveredObject == mButtonCooldown.gameObject )
        {
            mInfoPanelText.text = BuffBuildingCooldown.GetUIDescription( mButtonCooldown.interactable );
            mInfoPanel.SetActive( true );
        }
        else if( mHoveredObject == mButtonForge.gameObject )
        {
            mInfoPanelText.text = Forge.GetUIDescription(mButtonForge.interactable);
            mInfoPanel.SetActive( true );
        }
        else if( mHoveredObject == mButtonBombFactory.gameObject )
        {
            mInfoPanelText.text = BombFactory.GetUIDescription(mButtonBombFactory.interactable);
            mInfoPanel.SetActive( true );
        }
        else if (mHoveredObject == mButtonSpeed.gameObject)
        {
            mInfoPanelText.text = BuffBuildingSpeed.GetUIDescription(mButtonBombFactory.interactable);
            mInfoPanel.SetActive(true);
        }
        else if (mHoveredObject == mButtonJump.gameObject)
        {
            mInfoPanelText.text = BuffBuildingJump.GetUIDescription(mButtonBombFactory.interactable);
            mInfoPanel.SetActive(true);
        }
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
}
