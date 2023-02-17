using System;
using System.Collections.Generic;
using UnityEngine;


public class TowerBuff: TowerBase
{
    public GameObject mTowerBuffNode;


    override public void Initialize()
    {
        base.Initialize();

        mTowerBuffNode = GameObject.Find("TowerBuff")?.gameObject;
        mFloors.Add(GameObject.Find("FloorBuff")?.gameObject);
    }

    override public int GetTotalPossibleBuildingCountPerFloor()
    {
        return  1;
    }


    // ===================================
    // Private
    // ===================================


    public override void BuildFloor()
    {
        GameObject lastTowerFloor = mFloors[mFloors.Count - 1];

        Vector3 upVector = new Vector3(0, lastTowerFloor.transform.localScale.y, 0);
        GameObject prefab = Resources.Load<GameObject>("Prefabs/RTS/FloorBuff");
        GameObject newTowerFloor = GameObject.Instantiate(  prefab,
                                                            lastTowerFloor.transform.position + upVector,
                                                            Quaternion.Euler(0, 0, 0));
        newTowerFloor.transform.SetParent(mTowerBuffNode.transform);

        foreach (Transform gg in newTowerFloor.transform)
        {
            GameManager.mUIManager.mBuildableObjects.Add(gg.gameObject);
            GameManager.mUIManager.CreateBuildButtonOverObject(gg.gameObject);
        }

        mFloors.Add(newTowerFloor);
    }
}



