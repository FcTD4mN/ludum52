using System.Collections.Generic;
using System;


class cMCPHarvesterListPanelIMGUI :
    cMCPBuildingListPanelIMGUI
{
    public cMCPHarvesterListPanelIMGUI(string name) : base(name)
    {
    }

    override public string GetListTitle()
    {
        return "Harvester Buildings";
    }

    override public List<ProductionBuilding> GetBuildingList()
    {
        return GameManager.mRTSManager.mAllHarvesters.ConvertAll(new Converter<HarvestingBuilding, ProductionBuilding>((harvester) =>
        {
            return harvester.gameObject.GetComponent<ProductionBuilding>();
        }));
    }


    protected override void ShowBuildMenu(int index)
    {
        // Nothing here
    }


    protected override string GetHoverTextForBuilding(ProductionBuilding building)
    {
        if (building == null) return  "";
        if( building.GetComponent<HarvesterTower>() != null ) return  "";

        var harvester = building.GetComponent<HarvestingBuilding>();
        if (harvester == null) return  "";

        string finalText = harvester.GetBuildingType().ToString() + " source: \n";
        foreach (var resource in harvester.mResourceVein.mResource.mAvailable)
        {
            // Uses input to figure which resources are inside the vein
            // This wouldn't work well for resources that would have to replenish capabilities
            // Best would be to not set all fields of dictionnaries to 0, but only the one relevants
            // so we would have nothing to do in order to not show every resource with a value of 0 here
            if (harvester.mResourceVein.mResource.mInputRates[resource.Key] == 0) continue;

            finalText += " " + resource.Key.ToString() + ": " + ((int)resource.Value).ToString() + "\n";
        }

        return  finalText;
    }
}