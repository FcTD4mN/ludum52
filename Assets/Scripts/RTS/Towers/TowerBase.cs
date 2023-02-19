using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class TowerBase: pDelegateSender
{
    public GameObject mTowerNode;

    public List<GameObject> mFloors;
    public List<ProductionBuilding> mBuildings;

    public int mLevel = 1;


    // Messaging
    private List<WeakReference<pDelegateReceiver>> _mDelegates;
    public List<WeakReference<pDelegateReceiver>> mDelegates {
        get { return  _mDelegates; }
    }
    public enum eTowerMessages
    {
        kBuildingAdded,
        kBuildingRemoved,
        kLevelUp
    };


    // ===================================
    // Initialization
    // ===================================
    virtual public void Initialize()
    {
        _mDelegates = new List<WeakReference<pDelegateReceiver>>();
        mFloors = new List<GameObject>();
        mBuildings = new List<ProductionBuilding>( GetTotalPossibleBuildingCountPerFloor() );
        for( int i = 0; i < GetTotalPossibleBuildingCountPerFloor(); ++i )
        {
            mBuildings.Add( null );
        }
    }


    // ===================================
    // Query
    // ===================================
    public GameObject GetFloorAtIndex(int index)
    {
        return mFloors[index];
    }


    public int GetTotalPossibleBuildingCount()
    {
        return  mLevel * GetTotalPossibleBuildingCountPerFloor();
    }


    abstract public int GetTotalPossibleBuildingCountPerFloor();


    // ===================================
    // Buildings
    // ===================================
    public void AddBuildingAtIndex( ProductionBuilding building, int index )
    {
        building.mAssociatedTower = this;
        mBuildings[ index ] = building;
        var buildableObject = GetBuildableBlockAtIndex(index);

        ((pDelegateSender)this).CallAction( new object[] { eTowerMessages.kBuildingAdded, building, index, buildableObject } );
    }


    public void RemoveBuilding( ProductionBuilding building )
    {
        building.mAssociatedTower = null;

        var index = mBuildings.IndexOf( building );
        mBuildings[index] = null;
        ((pDelegateSender)this).CallAction(new object[] { eTowerMessages.kBuildingRemoved, building, index });
    }


    public void BuildAtIndex(string prefab, int index)
    {
        var buildableObject = GetBuildableBlockAtIndex( index );
        if (buildableObject.tag != "Buildable") { return; }

        GameManager.mRTSManager.BuildObjectAtLocation(prefab, buildableObject.gameObject);
    }


    public GameObject GetBuildableBlockAtIndex( int index )
    {
        int floorIndex = index / GetTotalPossibleBuildingCountPerFloor();
        var floor = GetFloorAtIndex(floorIndex);

        var buildableObject = floor.transform.GetChild(index % GetTotalPossibleBuildingCountPerFloor());
        return  buildableObject.gameObject;
    }


    // ===================================
    // Level Up
    // ===================================
    public void LevelUp()
    {
        mLevel += 1;
        mBuildings.Capacity += GetTotalPossibleBuildingCountPerFloor();
        for (int i = 0; i < GetTotalPossibleBuildingCountPerFloor(); ++i)
        {
            mBuildings.Add(null);
        }

        BuildFloor();
        ((pDelegateSender)this).CallAction(new object[] { eTowerMessages.kLevelUp });
    }


    // ===================================
    // Abstracts
    // ===================================
    abstract public void BuildFloor();
}