using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IronVein : ResourceVeinBase
{
    override internal cResourceDescriptor BuildResourceDescriptor()
    {
        cResourceDescriptor output = new cResourceDescriptor();

        output.mAvailable[cResourceDescriptor.eResourceNames.Iron] = 100f;
        output.mInputRates[cResourceDescriptor.eResourceNames.Iron] = 1f;

        return output;
    }
}
