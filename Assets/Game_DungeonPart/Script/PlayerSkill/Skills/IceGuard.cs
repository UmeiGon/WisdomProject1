using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IceGuard : StraightShot {

    public override void SetAbnormalState()
    {
        player.abnoState.invincibleTurn = 5;
    }
}
