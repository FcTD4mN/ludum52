using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HarvestingBuilding : ProductionBuilding
{

    // ===================================
    // Internal Setup
    // ===================================
    public new void OnEnable()
    {
        GameManager.mRTSManager.mAllHarvesters.Add( this );
        base.OnEnable();
    }


    public new void OnDisable()
    {
        GameManager.mRTSManager.mAllHarvesters.Remove( this );
        base.OnDisable();
    }





}
