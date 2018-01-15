using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireDig2 : StraightShot {
    public override void HitAndParamChange()
    {
        foreach (Vector2 ran in range)
        {
            Vector3 judgePos = targetExistPos - new Vector3(ran.x, 0, ran.y);
            int chara = mapMn.GetCharaExist((int)judgePos.x, (int)judgePos.z);
            int map = mapMn.GetDungeonInfo((int)judgePos.x, (int)judgePos.z);
            if (chara != 0)
            {
                BattleParticipant target = eneMn.GetEnemy(chara);
                if (target) target.DamageParameter((int)calcPower);
            }
            if (map == -1) // 壁なら
            {
                mapMn.ChangeMapChip((int)judgePos.z, (int)judgePos.x, -1, 0);
            }
        }
        battleTarget = null;
        base.HitAndParamChange();
    }
}
