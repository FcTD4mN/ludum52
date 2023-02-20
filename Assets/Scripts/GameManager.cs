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


    // Delayed init
    private static List<InitializableLate> mWaitingForInitialization;
    public static void AddToInitQueue( InitializableLate obj )
    {
        if( mWaitingForInitialization == null ) mWaitingForInitialization = new List<InitializableLate>();

        mWaitingForInitialization.Add( obj );
    }


    static public bool IsLoaded()
    {
        return  GameManager.mInstance != null;
    }

    // Start is called before the first frame update
    void OnEnable()
    {
        Initialize();
    }


    public void Initialize()
    {
        // If there is already an gamemanager instance
        if (GameManager.mInstance != null) { return; }

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

        foreach( var obj in mWaitingForInitialization )
        {
            obj.Initialize();
        }

        mWaitingForInitialization.Clear();
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        float deltaTime = Time.fixedDeltaTime;
        // Faire les appels ici pour garantir l'ordre, les resources doivent être updaté avant le reste du jeu
        if (mResourceManager != null)
            mResourceManager.UpdateResources( deltaTime );

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
                Vector3 positionDeLaTour = mRTSManager.mMainTower.mFloors[0].transform.position;

                Camera.main.orthographicSize = 7;
                float camHalfHeight = Camera.main.orthographicSize;
                float towerHalfHeight = mRTSManager.mMainTower.mFloors[0].transform.localScale.y / 2;
                float floorHeight = 1;

                Camera.main.transform.position = new Vector3(positionDeLaTour.x,
                                                                positionDeLaTour.y + camHalfHeight - towerHalfHeight - floorHeight,
                                                                Camera.main.transform.position.z);

                mUIManager.CreateBuildButtonOnEveryBuildableObject();
            }
            else
            {
                Camera.main.orthographicSize = 5;
                mUIManager.ClearUIForSwitchingView();
            }
        }
    }

    public void OnOpenControlPanel(InputAction.CallbackContext context)
    {
        if (context.started)
        {
            if (mIsInRTSMode)
            {
                mUIManager.BuildControlPanel();
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