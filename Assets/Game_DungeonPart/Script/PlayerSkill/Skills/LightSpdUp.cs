using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LightSpdUp : StraightShot
{
    public override void SetAbnormalState()
    {
        player.abnoState.spdUpTurn = 10;
    }
}
