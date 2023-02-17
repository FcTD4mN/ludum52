using System;
using System.Collections.Generic;
using UnityEngine;


public class TowerMain : TowerBase
{
    public int mReceiverCountPerLevel = 2;
    public GameObject mMainTowerNode;

    // Receivers
    public List<Receiver> mAllReceivers;


    override public void Initialize()
    {
        base.Initialize();
        mAllReceivers = new List<Receiver>();

        mMainTowerNode = GameObject.Find("TowerMain")?.gameObject;

        mFloors.Add(GameObject.Find("FloorMain")?.gameObject);
        mFloors[0].AddComponent(typeof(HarvesterTower));
    }


    // ===================================
    // Query
    // ===================================

    override public int GetTotalPossibleBuildingCountPerFloor()
    {
        return  2;
    }

    public int GetTotalPossibleReceiverCount()
    {
        return mLevel * mReceiverCountPerLevel;
    }


    // ===================================
    // Building
    // ===================================


    public GameObject BuildReceiver(string type, GameObject associatedHarvester)
    {
        int spawnIndex = GetAvailableSlotIndex();
        int towerFloorIndex = spawnIndex / 2;
        GameObject towerFloor = GetFloorAtIndex(towerFloorIndex);

        bool createOnTheRight = spawnIndex % 2 == 1;

        GameObject prefab = Resources.Load<GameObject>("Prefabs/RTS/" + type);
        GameObject newBuilding = GameObject.Instantiate(prefab,
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
        float floorMultiplier = (towerFloorIndex + 1) * (mFloors[0].transform.localScale.y + newBuilding.transform.localScale.y / 2);

        cable.transform.localScale = new Vector3(cable.transform.localScale.x, floorMultiplier, 1);
        cable.transform.position = new Vector3(cable.transform.position.x, cable.transform.position.y - ((floorMultiplier - 1) / 4), 0);

        // Add to lists
        var receiverComp = newBuilding.GetComponent<Receiver>();
        var associatedHarvesterComp = associatedHarvester.GetComponent<HarvestingBuilding>();
        if (receiverComp != null && associatedHarvesterComp != null)
        {
            receiverComp.mLocationIndex = spawnIndex;
            receiverComp.mAssociatedHarvester = associatedHarvesterComp;
            associatedHarvesterComp.mReceiver = receiverComp;
        }

        return newBuilding;
    }


    public override void BuildFloor()
    {
        GameObject lastTowerFloor = mFloors[mFloors.Count - 1];

        Vector3 upVector = new Vector3(0, lastTowerFloor.transform.localScale.y, 0);
        GameObject prefab = Resources.Load<GameObject>("Prefabs/RTS/FloorMain");
        GameObject newTowerFloor = GameObject.Instantiate(  prefab,
                                                            lastTowerFloor.transform.position + upVector,
                                                            Quaternion.Euler(0, 0, 0));
        newTowerFloor.transform.SetParent(mMainTowerNode.transform);

        foreach (Transform gg in newTowerFloor.transform)
        {
            GameManager.mUIManager.mBuildableObjects.Add(gg.gameObject);
            GameManager.mUIManager.CreateBuildButtonOverObject(gg.gameObject);
        }

        mFloors.Add(newTowerFloor);
    }


    // ===================================
    // Private
    // ===================================


    private int GetAvailableSlotIndex()
    {
        List<int> mAllIndices = new List<int>();
        foreach (Receiver receiver in mAllReceivers)
        {
            mAllIndices.Add(receiver.mLocationIndex);
        }

        mAllIndices.Sort();

        if (mAllIndices.Count == 0)
        {
            return 0;
        }

        if (mAllIndices.Count == 1)
        {
            return mAllIndices[0] == 0 ? 1 : 0;
        }

        if (mAllIndices[0] != 0)
        {
            return 0;
        }

        for (int i = 0; i < mAllIndices.Count - 1; i++)
        {
            int delta = mAllIndices[i + 1] - mAllIndices[i];
            if (delta > 1)
            {
                return mAllIndices[i] + 1;
            }
        }

        return mAllIndices[mAllIndices.Count - 1] + 1;
    }
}