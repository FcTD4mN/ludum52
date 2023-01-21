public class Forge : ProductionBuilding
{
    override public cResourceDescriptor GetNewResourceDescriptor()
    {
        cResourceDescriptor output = new cResourceDescriptor();

        output.mBuildCosts[ cResourceDescriptor.eResourceNames.Gold ] = 2000;
        output.mBuildCosts[ cResourceDescriptor.eResourceNames.Iron ] = 100;
        output.mInputRates[ cResourceDescriptor.eResourceNames.Iron ] = 2;
        output.mOutputRates[ cResourceDescriptor.eResourceNames.Arrows ] = 1;

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
