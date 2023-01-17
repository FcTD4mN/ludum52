using UnityEngine;
using System.Collections.Generic;
using System;



class cMCPBuffersPanel :
    cMCPBuildingPanelBase
{
    public cMCPBuffersPanel(GameObject parentView, string name) : base(parentView, name, "Buffer Buildings")
    {
    }

    internal override List<ProductionBuilding> GetBuildingList()
    {
        return GameManager.mRTSManager.mAllBuffBuildings.ConvertAll(new Converter<BuffBuilding, ProductionBuilding>((buff) =>
        {
            return buff.gameObject.GetComponent<ProductionBuilding>();
        }));
    }
}
