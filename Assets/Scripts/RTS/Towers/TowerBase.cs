using System;
using System.Collections.Generic;
using UnityEngine;


public abstract class TowerBase
{
    public List<GameObject> mFloors;
    public List<ProductionBuilding> mBuildings;

    public int mLevel = 1;


    // ===================================
    // Initialization
    // ===================================
    virtual public void Initialize()
    {
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
    }


    public void RemoveBuilding( ProductionBuilding building )
    {
        building.mAssociatedTower = null;

        var index = mBuildings.IndexOf( building );
        mBuildings[index] = null;
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
    }


    // ===================================
    // Abstracts
    // ===================================
    abstract public void BuildFloor();
}