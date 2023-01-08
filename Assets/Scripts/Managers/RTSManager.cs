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
    public List<(GameObject, int)> mIronReceivers;
    public List<(GameObject, int)> mFireReceivers;
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

        mIronReceivers = new List<(GameObject, int)>();
        mFireReceivers = new List<(GameObject, int)>();
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

        Rect buildingSpotBBox = Utilities.GetBBoxFromTransform( objectToBuildOver );
        Rect newBuildingSpotBBox = Utilities.GetBBoxFromTransform( newBuilding );
        newBuilding.transform.localScale = new Vector3( buildingSpotBBox.width / newBuildingSpotBBox.width, buildingSpotBBox.height / newBuildingSpotBBox.height, 1 );

        if( newBuilding.gameObject.GetComponent<HarvestingBuilding>() != null )
        {
            if (newBuilding.gameObject.GetComponent<IronHarvester>() != null)
            {
                BuildReceiver("IronReceiver");
            }
            else if (newBuilding.gameObject.GetComponent<FireMaker>() != null)
            {
                BuildReceiver("FireReceiver");
            }
        }

        if( CanLevelUp() )
        {
            LevelUp();
        }
    }


    private void BuildReceiver( string type )
    {
        int spawnIndex = GetAvailableSlotIndex();
        int towerFloorIndex = spawnIndex / 2;
        GameObject towerFloor = mTowers[towerFloorIndex];

        bool createOnTheRight = spawnIndex % 2 == 1;

        GameObject prefab = Resources.Load<GameObject>("Prefabs/RTS/" + type);
        GameObject newBuilding = Instantiate(prefab,
                                                Vector3.zero,
                                                Quaternion.Euler(0, 0, 0));


        Vector3 spawnLocation = new Vector3();
        float towerHalfSize = towerFloor.transform.localScale.x / 2;
        float receiverHalfSize = newBuilding.transform.localScale.x / 2;
        if (createOnTheRight)
        {
            spawnLocation.Set(towerFloor.transform.position.x + towerHalfSize + receiverHalfSize, towerFloor.transform.position.y, 0);
        }
        else
        {
            spawnLocation.Set(towerFloor.transform.position.x - towerHalfSize - receiverHalfSize, towerFloor.transform.position.y, 0);
        }
        newBuilding.transform.position = spawnLocation;
        newBuilding.transform.SetParent(towerFloor.transform);

        GameObject cable = newBuilding.transform.Find("Cable").gameObject;
        float floorMultiplier = (towerFloorIndex + 1) * (mTowers[0].transform.localScale.y + newBuilding.transform.localScale.y / 2);

        cable.transform.localScale = new Vector3( cable.transform.localScale.x, floorMultiplier, 1 );
        cable.transform.position = new Vector3( cable.transform.position.x, cable.transform.position.y - ((floorMultiplier-1)/4), 0 );

        // Add to lists
        if( newBuilding.GetComponent<IronReceiver>() != null )
        {
            mIronReceivers.Add( (newBuilding, spawnIndex ) );
        }
        else if (newBuilding.GetComponent<FireReceiver>() != null)
        {
            mFireReceivers.Add((newBuilding, spawnIndex));
        }
    }


    private int GetAvailableSlotIndex()
    {
        List<int> mAllIndices = new List<int>();
        foreach( (GameObject, int) receiver in mIronReceivers )
        {
            mAllIndices.Add( receiver.Item2 );
        }
        foreach ((GameObject, int) receiver in mFireReceivers)
        {
            mAllIndices.Add(receiver.Item2);
        }

        mAllIndices.Sort();

        if( mAllIndices.Count == 0 )
        {
            return  0;
        }

        if( mAllIndices.Count == 1 )
        {
            return  mAllIndices[0] == 0 ? 1 : 0;
        }

        if( mAllIndices[0] != 0 )
        {
            return  0;
        }

        for( int i = 0; i < mAllIndices.Count - 1; i++ )
        {
            int delta = mAllIndices[i+1] - mAllIndices[i];
            if( delta > 1 ) {
                return  mAllIndices[i] + 1;
            }
        }

        return  mAllIndices[mAllIndices.Count - 1] + 1;
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

                if (building.GetComponent<IronHarvester>() != null)
                {
                    (GameObject, int) lastIronReceiver = mIronReceivers.FindLast( (element)=>{ return  element.Item1.GetComponent<IronReceiver>() != null; } );
                    mIronReceivers.Remove( lastIronReceiver );
                    GameObject.Destroy( lastIronReceiver.Item1 );
                }
                else if (building.GetComponent<FireMaker>() != null)
                {
                    (GameObject, int) lastFireReceiver = mFireReceivers.FindLast((element) => { return element.Item1.GetComponent<FireReceiver>() != null; });
                    mFireReceivers.Remove(lastFireReceiver);
                    GameObject.Destroy(lastFireReceiver.Item1);
                }
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
