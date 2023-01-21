public class BombFactory : ProductionBuilding
{
    override public cResourceDescriptor GetNewResourceDescriptor()
    {
        cResourceDescriptor output = new cResourceDescriptor();

        output.mBuildCosts[cResourceDescriptor.eResourceNames.Gold] = 3000;
        output.mBuildCosts[cResourceDescriptor.eResourceNames.Iron] = 500;

        output.mInputRates[cResourceDescriptor.eResourceNames.Iron] = 2;
        output.mInputRates[cResourceDescriptor.eResourceNames.Fire] = 2;
        output.mOutputRates[cResourceDescriptor.eResourceNames.Bombs] = 1;

        return output;
    }

    public override RTSManager.eBuildingList GetBuildingType()
    {
        return RTSManager.eBuildingList.BombFactory;
    }

    public override string GetDescription()
    {
        return "Builds arrows using iron";
    }
}
