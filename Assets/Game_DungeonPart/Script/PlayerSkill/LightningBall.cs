using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightningBall : StraightShot {

    public override void SetAbnormalState()
    {
        battleTarget.abnoState.paralizeTurn = 10;
    }
}
