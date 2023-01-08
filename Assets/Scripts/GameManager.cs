using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using Cinemachine;

public class GameManager : MonoBehaviour
{
    // Singleton
    public static GameManager mInstance;
    public static ResourceManager mResourceManager;
    public static UIManager mUIManager;
    public static RTSManager mRTSManager;

    private bool mIsInRTSMode = false;

    // PlayerController Ref
    [HideInInspector]
    public PlayerController playerCtrler;

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

        playerCtrler = GameObject.Find("Character")?.gameObject.GetComponent<PlayerController>();
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
        if (context.started )
        {
            mIsInRTSMode = !mIsInRTSMode;

            CinemachineBrain cervoCamera = Camera.main.GetComponent<CinemachineBrain>();
            cervoCamera.enabled = !mIsInRTSMode;

            if( mIsInRTSMode )
            {
                Vector3 positionDeLaTour = mRTSManager.mTowers[0].transform.position;
                Camera.main.transform.position = new Vector3( positionDeLaTour.x, positionDeLaTour.y, Camera.main.transform.position.z ) ;
                mUIManager.CreateBuildButtonOnEveryBuildableObject();
            }
            else
            {
                mUIManager.ClearUIForSwitchingView();
            }
        }
    }
}
