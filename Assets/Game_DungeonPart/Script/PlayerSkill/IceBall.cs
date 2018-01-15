using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceBall : StraightShot {

    public override void SetAbnormalState()
    {
        battleTarget.abnoState.freezeTurn = 5;
    }
}
