public class BombFactory : ProductionBuilding
{
    override public cResourceDescriptor GetResourceDescriptor()
    {
        cResourceDescriptor output = new cResourceDescriptor();

        output.mBuildCosts[cResourceDescriptor.eResourceNames.Gold.ToString()] = 3000;
        output.mBuildCosts[cResourceDescriptor.eResourceNames.Iron.ToString()] = 500;

        output.mInputRates[cResourceDescriptor.eResourceNames.Iron.ToString()] = 2;
        output.mInputRates[cResourceDescriptor.eResourceNames.Fire.ToString()] = 2;
        output.mOutputRates[cResourceDescriptor.eResourceNames.Bombs.ToString()] = 1;

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
