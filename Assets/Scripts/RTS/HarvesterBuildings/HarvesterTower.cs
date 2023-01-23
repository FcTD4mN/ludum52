public class HarvesterTower : HarvestingBuilding
{
    override public cResourceDescriptor GetNewResourceDescriptor()
    {
        cResourceDescriptor output = new cResourceDescriptor();

        output.mOutputRates[cResourceDescriptor.eResourceNames.Gold] = 10;
        output.mOutputRates[cResourceDescriptor.eResourceNames.Iron] = 1;

        return output;
    }

    public override RTSManager.eBuildingList GetBuildingType()
    {
        return RTSManager.eBuildingList.Other;
    }


    internal override void Initialize()
    {
        mAddDiode = false;
        base.Initialize();
    }


    public override string GetDisplayName()
    {
        return "Main Tower";
    }

    public override string GetDescription()
    {
        return "This is the main Tower";
    }

    public new bool IsBuildable()
    {
        return false;
    }


    override public bool IsConnected()
    {
        return true;
    }

    public new RTSManager.eBuildingErrors GetBuildingError()
    {
        return RTSManager.eBuildingErrors.None;
    }

    override public string GetAssociatedReceiver()
    {
        return "";
    }
}
