public class Forge : ProductionBuilding
{
    override public cResourceDescriptor GetResourceDescriptor()
    {
        cResourceDescriptor output = new cResourceDescriptor();

        output.mBuildCosts[ cResourceDescriptor.eResourceNames.Gold.ToString() ] = 2000;
        output.mBuildCosts[ cResourceDescriptor.eResourceNames.Iron.ToString() ] = 100;
        output.mInputRates[ cResourceDescriptor.eResourceNames.Iron.ToString() ] = 2;
        output.mOutputRates[ cResourceDescriptor.eResourceNames.Arrows.ToString() ] = 1;

        return  output;
    }

    public override RTSManager.eBuildingList GetBuildingType()
    {
        return  RTSManager.eBuildingList.Forge;
    }

    public override string GetDescription()
    {
        return  "Builds arrows using iron";
    }
}
