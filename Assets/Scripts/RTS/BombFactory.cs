public class BombFactory : ProductionBuilding
{
    public static cResourceDescriptor GetResourceDescriptor()
    {
        cResourceDescriptor output = new cResourceDescriptor();

        output.mBuildCosts[ cResourceDescriptor.eResourceNames.Gold.ToString() ] = 3000;
        output.mBuildCosts[ cResourceDescriptor.eResourceNames.Iron.ToString() ] = 500;

        output.mInputRates[ cResourceDescriptor.eResourceNames.Iron.ToString() ] = 2;
        output.mInputRates[ cResourceDescriptor.eResourceNames.Fire.ToString() ] = 2;
        output.mOutputRates[ cResourceDescriptor.eResourceNames.Bombs.ToString() ] = 1;

        return  output;
    }


    public static bool IsBuildable()
    {
        cResourceDescriptor resourceDescriptor = GetResourceDescriptor();
        foreach( string resourceName in cResourceDescriptor.mAllResourceNames )
        {
            if( resourceDescriptor.mBuildCosts[resourceName] > GameManager.mResourceManager.GetRessource( resourceName ) ) {
                return  false;
            }
        }

        return  true;
    }

    override internal void Initialize()
    {
        base.Initialize();
        mResourceDescriptor = GetResourceDescriptor();
    }
}
