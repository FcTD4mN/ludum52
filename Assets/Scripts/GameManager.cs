using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager mInstance;
    public static ResourceManager mResourceManager;
    public static UIManager mUIManager;
    public static RTSManager mRTSManager;
    public PortalManager mPortalManager;

    private bool mIsInRTSMode = false;
    private bool mFirstTime = true;

    // PlayerController Ref
    [HideInInspector]
    public PlayerController playerCtrler;


    static void CheckIfExists()
    {
        if (GameManager.mInstance != null) { return; }
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        // If there is already an gamemanager instance
        if (GameManager.mInstance != null) { return; }

        cResourceDescriptor.BuildResourceList();
        cStatsDescriptor.BuildStatsList();

        mInstance = this;
        mResourceManager = GameObject.Find("ResourceManager")?.gameObject.GetComponent<ResourceManager>();
        if (mResourceManager != null)
            mResourceManager.Initialize();

        mRTSManager = GameObject.Find("RTSManager")?.gameObject.GetComponent<RTSManager>();
        if (mRTSManager != null)
            mRTSManager.Initialize();

        mUIManager = GameObject.Find("UIManager")?.gameObject.GetComponent<UIManager>();
        if (mUIManager != null)
            mUIManager.Initialize();

        mPortalManager = GameObject.Find("PortalManager")?.gameObject.GetComponent<PortalManager>();
        if (mPortalManager != null)
            mPortalManager.Initialize();

        playerCtrler = GameObject.Find("Character")?.gameObject.GetComponent<PlayerController>();


        Rect screenRect = Camera.main.pixelRect;

        cButton test = new cButton( mUIManager.mCanvas.gameObject, "test" );
        test.SetFrame( new Rect( 10, 10, 100, 100 ) );
        test.SetCenter( screenRect.center );
        test.SetColor( new Color( 0.8f, 0.1f, 0f, 1f ) );
        test.AddText( "CenterButton" );

        cScrollView scroll = new cBuildMenu( mUIManager.mCanvas.gameObject, "ScrollTest" );
        scroll.SetFrame(new Rect(10, 10, 500, 500));
        scroll.SetCenter( screenRect.center );
    }

    // Update is called once per frame
    void Update()
    {
        // Faire les appels ici pour garantir l'ordre, les resources doivent être updaté avant le reste du jeu
        if (mResourceManager != null)
            mResourceManager.UpdateResources();

        // Surement en dernier, l'ui s'update
        if (mUIManager != null)
            mUIManager.UpdateUI();
    }


    public void OnSwitch(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if( mFirstTime )
            {
                mFirstTime = false;
                string text = "<color=#9eb54a>THE BASE</color>";
                text += "<br>";
                text += "<size=60%>This is where you build and manage your productions.";
                text += "<br>";
                text += "Every building costs resources to build and to operate.";
                text += "<br>";
                text += "Once built, it will produce either resources or buff some stats.";
                text += "</size>";

                mUIManager.DisplayMessage( text, 10 );
            }

            mIsInRTSMode = !mIsInRTSMode;

            CinemachineBrain cervoCamera = Camera.main.GetComponent<CinemachineBrain>();
            cervoCamera.enabled = !mIsInRTSMode;

            if (mIsInRTSMode)
            {
                Vector3 positionDeLaTour = mRTSManager.mTowers[0].transform.position;

                float camHalfHeight = Camera.main.orthographicSize;
                float towerHalfHeight = mRTSManager.mTowers[0].transform.localScale.y / 2;
                float floorHeight = 1;

                Camera.main.transform.position = new Vector3(positionDeLaTour.x,
                                                                positionDeLaTour.y + camHalfHeight - towerHalfHeight - floorHeight,
                                                                Camera.main.transform.position.z);
                mUIManager.CreateBuildButtonOnEveryBuildableObject();
            }
            else
            {
                mUIManager.ClearUIForSwitchingView();
            }
        }
    }

    // Faire ca plus propre un jour :
    public List<UnlockableAction> unlockedActions = new List<UnlockableAction>();

    public void UnlockAction(UnlockableAction action)
    {
        unlockedActions.Add(action);
    }

    public bool IsUnlockAction(UnlockableAction action)
    {
        return unlockedActions.Contains(action);
    }
}

public enum UnlockableAction { DoubleJump, WallJump, Dash };