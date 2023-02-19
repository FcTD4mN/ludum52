using System;
using System.Collections.Generic;
using UnityEngine;


public class TowerBuff: TowerBase
{

    override public void Initialize()
    {
        base.Initialize();

        mTowerNode = GameObject.Find("TowerBuff")?.gameObject;
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
        newTowerFloor.transform.SetParent(mTowerNode.transform);

        mFloors.Add(newTowerFloor);
    }
}



