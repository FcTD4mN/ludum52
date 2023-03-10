using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Receiver : MonoBehaviour
{
    public HarvestingBuilding mAssociatedHarvester;

    public int mLocationIndex = -1;

    public void OnEnable()
    {
        GameManager.mRTSManager.mMainTower.mAllReceivers.Add( this );
    }

    public void OnDisable()
    {
        GameManager.mRTSManager.mMainTower.mAllReceivers.Remove(this);
    }
}
