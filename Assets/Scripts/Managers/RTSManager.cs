using System;
using System.Collections.Generic;
using UnityEngine;

public class RTSManager : MonoBehaviour
{
    public GameObject mRTSWorld;
    public TowerMain mMainTower;
    public TowerBuff mBuffTower;

    // All Buildings
    public List<HarvestingBuilding> mAllHarvesters;
    public List<ProductionBuilding> mAllProductionBuildings;
    public List<BuffBuilding> mAllBuffBuildings;

    // Built building that have been removed
    public Dictionary<eBuildingList, int> mAvailableBuildings;

    // Resource Veins
    public List<ResourceVeinBase> mAllResourceVeins;


    public List<(GameObject, GameObject)> mBuildingToBuildableRelations;

    public enum eBuildingErrors
    {
        None,
        NotEnoughRessources,
        BlueprintRequired
    }

    public enum eBuildingList
    {
        Other,
        IronHarvester,
        FireHarvester,
        Forge,
        BombFactory,
        Workshop,
        BuffDamage,
        BuffCooldown,
        BuffSpeed,
        BuffJump
    }
    public List<eBuildingList> mUnlockedBuildings;


    // ===================================
    // Building
    // ===================================
    public void Initialize()
    {
        mAllProductionBuildings = new List<ProductionBuilding>();
        mAllHarvesters = new List<HarvestingBuilding>();

        mAvailableBuildings = new Dictionary<eBuildingList, int>();
        foreach (eBuildingList type in Enum.GetValues(typeof(eBuildingList)))
        {
            mAvailableBuildings[type] = 0;
        }

        mBuildingToBuildableRelations = new List<(GameObject, GameObject)>();

        mUnlockedBuildings = new List<eBuildingList>();
        mUnlockedBuildings.Add(eBuildingList.IronHarvester);
        mUnlockedBuildings.Add(eBuildingList.FireHarvester);
        mUnlockedBuildings.Add(eBuildingList.Forge);
        mUnlockedBuildings.Add(eBuildingList.BuffJump);


        mRTSWorld = GameObject.Find("RTSWorld")?.gameObject;
        mMainTower = new TowerMain();
        mMainTower.Initialize();
        mBuffTower = new TowerBuff();
        mBuffTower.Initialize();
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

        var harvester = newBuilding.gameObject.GetComponent<HarvestingBuilding>();
        if( harvester != null ) // If we built harvester
        {
            var resourceVein = objectToBuildOver.GetComponent<ResourceVeinBase>();
            if( resourceVein == null ) return;

            harvester.mResourceVein = resourceVein;

            if( mMainTower.mAllReceivers.Count < mMainTower.GetTotalPossibleReceiverCount() ) // and slots are left for receiver
            {
                GameObject createdReceiver = null;
                if (newBuilding.gameObject.GetComponent<IronHarvester>() != null)
                {
                    createdReceiver = mMainTower.BuildReceiver("IronReceiver", newBuilding);
                }
                else if (newBuilding.gameObject.GetComponent<FireHarvester>() != null)
                {
                    createdReceiver = mMainTower.BuildReceiver("FireReceiver", newBuilding);
                }
            }
        }
        else // Regular building
        {
            mBuildingToBuildableRelations.Add((newBuilding, objectToBuildOver));
            objectToBuildOver.SetActive( false );

            var parentFloor = objectToBuildOver.transform.parent;
            var relatedTower = GetTowerByFloor( parentFloor.gameObject );
            if( relatedTower != null )
            {
                var floorIndex = parentFloor.GetSiblingIndex();
                var index = floorIndex * relatedTower.GetTotalPossibleBuildingCountPerFloor() + objectToBuildOver.transform.GetSiblingIndex();
                relatedTower.AddBuildingAtIndex( newBuilding.GetComponent<ProductionBuilding>(), index );
            }
        }

        if( CanLevelUp() )
        {
            LevelUp();
        }
    }


    public void DestroyBuilding( GameObject building )
    {
        List<(GameObject, GameObject)> relations = mBuildingToBuildableRelations.FindAll((element) => { return element.Item1 == building; });
        foreach ((GameObject, GameObject) relation in relations)
        {
            relation.Item2.SetActive(true);
            mBuildingToBuildableRelations.Remove(relation);
        }

        // If we destroy a receiver, just disconnect the harvester from it, don't destroy the harvester
        var receiver = building.GetComponent<Receiver>();
        if( receiver != null )
        {
            receiver.mAssociatedHarvester.mReceiver = null;
            GameObject.Destroy( building );
            return;
        }

        // Using destroy method for production buildings
        building.GetComponent<ProductionBuilding>()?.DeleteBuilding();
    }


    public void LevelUp()
    {
        mMainTower.LevelUp();
        mBuffTower.LevelUp();
    }


    public bool CanLevelUp()
    {
        int nonHarvesterCount = mAllProductionBuildings.Count - mAllHarvesters.Count - mAllBuffBuildings.Count;
        return  nonHarvesterCount == mMainTower.GetTotalPossibleBuildingCount();
    }


    public ProductionBuilding GetPrefabByType(eBuildingList type)
    {
        GameObject prefab = Resources.Load<GameObject>("Prefabs/RTS/" + type.ToString());
        ProductionBuilding prod = prefab.GetComponent<ProductionBuilding>();
        prod.Initialize();
        return prod;
    }


    public TowerBase GetTowerByFloor( GameObject floor )
    {
        if( mMainTower.mFloors.Contains( floor ) ) return  mMainTower;
        if( mBuffTower.mFloors.Contains( floor ) ) return  mBuffTower;

        return  null;
    }
}




