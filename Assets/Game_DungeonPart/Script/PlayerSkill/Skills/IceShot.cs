using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceShot : StraightShot
{
    public override void SetAbnormalState()
    {
        if (battleTarget)
        battleTarget.abnoState.freezeTurn = 3;
    }
}
