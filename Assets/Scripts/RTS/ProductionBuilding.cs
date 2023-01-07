using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ProductionBuilding : MonoBehaviour
{
    public void OnEnable()
    {
        GameManager.mResourceManager.mAllProductionBuilding.Add( this );
        Build();
    }


    void OnDisable()
    {
        GameManager.mResourceManager.mAllProductionBuilding.Remove( this );
    }


    virtual internal void Build()
    {
        // Implement
    }

    virtual public void GenerateResource()
    {
        // Implement
    }

}
