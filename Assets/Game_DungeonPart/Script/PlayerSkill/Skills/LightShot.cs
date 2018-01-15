using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightShot : StraightShot
{
    public override void SetAbnormalState()
    {
        if (battleTarget)
        battleTarget.abnoState.paralizeTurn = 6;
    }
}
