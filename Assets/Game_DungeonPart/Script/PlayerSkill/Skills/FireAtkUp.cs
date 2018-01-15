using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireAtkUp : StraightShot {

    public override void SetAbnormalState()
    {
        player.abnoState.atkUpTurn = 10;
    }
}
