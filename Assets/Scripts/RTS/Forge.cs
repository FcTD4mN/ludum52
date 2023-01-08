public class Forge : ProductionBuilding
{
    public static cResourceDescriptor GetResourceDescriptor()
    {
        cResourceDescriptor output = new cResourceDescriptor();

        output.mBuildCosts[ cResourceDescriptor.eResourceNames.Gold.ToString() ] = 2000;
        output.mBuildCosts[ cResourceDescriptor.eResourceNames.Iron.ToString() ] = 100;
        output.mInputRates[ cResourceDescriptor.eResourceNames.Iron.ToString() ] = 2;
        output.mOutputRates[ cResourceDescriptor.eResourceNames.Arrows.ToString() ] = 1;

        return  output;
    }


    public static bool IsBuildable()
    {
        cResourceDescriptor resourceDescriptor = GetResourceDescriptor();
        foreach( string resourceName in cResourceDescriptor.mAllResourceNames )
        {
            if( resourceDescriptor.mBuildCosts[resourceName] > GameManager.mResourceManager.GetRessource(resourceName) ) {
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
