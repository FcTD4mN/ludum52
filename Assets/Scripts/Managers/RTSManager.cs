using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RTSManager : MonoBehaviour
{
    public GameObject mRTSWorld;
    public List<GameObject> mTowers;

    public int mTowerLevel = 1;
    public int mHarvesterSlots = 2;

    public List<HarvestingBuilding> mAllHarvesters;
    public List<ProductionBuilding> mAllProductionBuildings;


    public List<GameObject> mProductionBuildingSpots;
    public List<GameObject> mBuffBuildingSpots;





    // ===================================
    // Building
    // ===================================
    public void Initialize()
    {
        mRTSWorld = GameObject.Find("RTSWorld")?.gameObject;
        mTowers = new List<GameObject>();
        mTowers.Add( GameObject.Find("Tower")?.gameObject );

        mProductionBuildingSpots.Add( mTowers[0].transform.Find("EmptyBuildingSpot1")?.gameObject );
        mProductionBuildingSpots.Add( mTowers[0].transform.Find("EmptyBuildingSpot2")?.gameObject );

        mAllProductionBuildings = new List<ProductionBuilding>();
        mAllHarvesters = new List<HarvestingBuilding>();
    }


    // ===================================
    // Buildings
    // ===================================
    public void BuildObjectAtLocation( string objectName, GameObject objectToBuildOver )
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/RTS/" + objectName);
        GameObject newBuilding = Instantiate( prefab,
                                                objectToBuildOver.transform.position,
                                                Quaternion.Euler(0, 0, 0) );

        newBuilding.transform.SetParent( mRTSWorld.transform );

        if( CanLevelUp() )
        {
            LevelUp();
        }
    }


    public void DestroyBuilding( GameObject building )
    {
        ProductionBuilding prod = building.GetComponent<ProductionBuilding>();
        if( prod != null )
        {
            mAllProductionBuildings.Remove( prod );
            if( building.GetComponent<HarvestingBuilding>() != null )
            {
                mAllHarvesters.Remove( building.GetComponent<HarvestingBuilding>() );
            }
        }

        GameObject.Destroy( building );
    }


    public bool CanBuildHarvester()
    {
        return  mAllHarvesters.Count < mHarvesterSlots;
    }


    // ===================================
    // Tower
    // ===================================
    public void LevelUp()
    {
        mTowerLevel += 1;
        mHarvesterSlots += 2;

        GameObject lastTowerFloor = mTowers[mTowers.Count - 1];
        Vector3 upVector = new Vector3( 0, lastTowerFloor.transform.localScale.y, 0 );

        GameObject prefab = Resources.Load<GameObject>("Prefabs/RTS/Tower");
        GameObject newTowerFloor = Instantiate( prefab,
                                                lastTowerFloor.transform.position + upVector,
                                                Quaternion.Euler(0, 0, 0));
        newTowerFloor.transform.SetParent( mRTSWorld.transform );

        foreach( Transform gg in newTowerFloor.transform )
        {
            GameManager.mUIManager.mBuildableObjects.Add( gg.gameObject );
            GameManager.mUIManager.CreateBuildButtonOverObject( gg.gameObject );
        }

        mTowers.Add( newTowerFloor );
    }


    public bool CanLevelUp()
    {
        int nonHarvesterCount = mAllProductionBuildings.Count - mAllHarvesters.Count;
        return  nonHarvesterCount == mTowerLevel*2;
    }
}
