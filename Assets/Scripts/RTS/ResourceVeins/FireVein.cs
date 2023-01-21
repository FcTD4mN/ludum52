using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireVein : ResourceVeinBase
{
    override internal cResourceDescriptor BuildResourceDescriptor()
    {
        cResourceDescriptor output = new cResourceDescriptor();

        output.mAvailable[cResourceDescriptor.eResourceNames.Fire] = 10000f;
        output.mInputRates[cResourceDescriptor.eResourceNames.Fire] = 1f;

        return  output;
    }
}
