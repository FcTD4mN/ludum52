using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSManager : MonoBehaviour
{
    private GameObject mRTSWorld;

    // ===================================
    // Building
    // ===================================
    public void Initialize()
    {
        mRTSWorld = GameObject.Find("RTSWorld")?.gameObject;
    }


    // ===================================
    // Building
    // ===================================


    public void BuildObjectAtLocation( string objectName, Vector3 location )
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/RTS/" + objectName);
        GameObject newHarvester = Instantiate( prefab,
                                                location,
                                                Quaternion.Euler(0, 0, 0) );

        newHarvester.transform.SetParent( mRTSWorld.transform );
    }
}
