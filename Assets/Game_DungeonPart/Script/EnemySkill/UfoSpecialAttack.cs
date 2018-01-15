using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UfoSpecialAttack : NPCSkill
{
    private BattleParticipant targetChara = null;


    public void FindLightMonster()
    {
        targetChara = parent.GetComponentInChildren<Player>();
        foreach (Enemy ene in eneMn.enemys)
        {
            if (ene.type == EnemyType.LIGHT)
            {
                // 光るモンスターが近いならそっちをターゲットする
                int d_x = Mathf.Abs((int)(ene.pos.x - thisChara.pos.x));
                int d_z = Mathf.Abs((int)(ene.pos.z - thisChara.pos.z));
                if (d_x < AImove.searchTargetRange)
                {
                    if (d_z < AImove.searchTargetRange)
                    {
                        targetChara = ene;
                        break;
                    }
                }
                // 光るモンスターが同じ部屋にいるならそっちをターゲットする
                if (0 <= thisChara.existRoomNum && thisChara.existRoomNum <= mapMn.max_room)
                {
                    if (ene.existRoomNum == thisChara.existRoomNum)
                    {
                        targetChara = ene;
                        break;
                    }
                }
            }
        }
    }

    public override void Prepare()
    {
        if ( targetChara == null )
        {
            FindLightMonster();
        }

        int rnd = 0;
        int pointX = 0;
        int pointZ = 0;
        int enemyX = 0;
        int enemyZ = 0;
        int thisX = 0;
        int thisZ = 0;
        int checkCount = 1;

        foreach (Enemy ene in eneMn.enemys)
        {
            int d_x = Mathf.Abs((int)(ene.sPos.x - thisChara.pos.x));
            int d_z = Mathf.Abs((int)(ene.sPos.z - thisChara.pos.z));
            if (d_x == 0)
                if (d_z == 0)
                    continue;

            bool canCarry = false;
            for (int i = -1; i < 2; ++i)
            {
                canCarry = false;
                if (i == -1)
                {
                    if ((int)ene.pos.x == 0)
                        continue;
                }
                else if (i == 1)
                    if ((int)ene.pos.x == MapManager.DUNGEON_WIDTH - 1)
                        continue;
                for (int k = -1; k < 2; ++k)
                {
                    if (k == -1)
                    {
                        if ((int)ene.pos.z == 0)
                            continue;
                    }
                    else if (k == 1)
                        if ((int)ene.pos.z == MapManager.DUNGEON_HEIGHT - 1)
                            continue;

                    int x = (int)ene.pos.x + i;
                    int z = (int)ene.pos.z + k;
                    if (mapMn.dung_2D[z, x] < 0) continue;
                    if (mapMn.chara_exist2D[z, x] > -1) continue;

                    bool top = (z != MapManager.DUNGEON_HEIGHT - 1);
                    bool bottom = (z != 0);
                    bool right = (x != MapManager.DUNGEON_WIDTH - 1);
                    bool left = (x != 0);

                    if (top)
                    {
                        if (mapMn.dung_2D[z + 1, x] > -1)
                            if (mapMn.chara_exist2D[z + 1, x] < 0)
                            {
                                canCarry = true;
                                break;
                            }
                        if (left)
                            if (mapMn.dung_2D[z + 1, x - 1] > -1)
                                if (mapMn.chara_exist2D[z + 1, x - 1] < 0)
                                {
                                    canCarry = true;
                                    break;
                                }
                        if (right)
                            if (mapMn.dung_2D[z + 1, x + 1] > -1)
                                if (mapMn.chara_exist2D[z + 1, x + 1] < 0)
                                {
                                    canCarry = true;
                                    break;
                                }
                    }
                    if (bottom)
                    {
                        if (mapMn.dung_2D[z - 1, x] > -1)
                            if (mapMn.chara_exist2D[z - 1, x] < 0)
                            {
                                canCarry = true;
                                break;
                            }
                        if (left)
                            if (mapMn.dung_2D[z - 1, x - 1] > -1)
                                if (mapMn.chara_exist2D[z - 1, x - 1] < 0)
                                {
                                    canCarry = true;
                                    break;
                                }
                        if (right)
                            if (mapMn.dung_2D[z - 1, x + 1] > -1)
                                if (mapMn.chara_exist2D[z - 1, x + 1] < 0)
                                {
                                    canCarry = true;
                                    break;
                                }
                    }
                    {
                        if (left)
                            if (mapMn.dung_2D[z, x - 1] > -1)
                                if (mapMn.chara_exist2D[z, x - 1] < 0)
                                {
                                    canCarry = true;
                                    break;
                                }
                        if (right)
                            if (mapMn.dung_2D[z, x + 1] > -1)
                                if (mapMn.chara_exist2D[z, x + 1] < 0)
                                {
                                    canCarry = true;
                                    break;
                                }
                    }
                }
                if (canCarry)
                    break;
            }
            if (!canCarry) continue;

            rnd = Random.Range(0, checkCount);
            if (rnd == 0)
            {
                enemyX = (int)ene.pos.x;
                enemyZ = (int)ene.pos.z;
            }
            checkCount++;
        }

        checkCount = 1;
        for (int i = -1; i < 2; ++i)
        {
            if (i == -1)
            {
                if (enemyX == 0) continue;
            }
            else if (i == 1)
                if (enemyX == MapManager.DUNGEON_WIDTH - 1) continue;
            for (int k = -1; k < 2; ++k)
            {
                if (k == -1)
                {
                    if (enemyZ == 0) continue;
                }
                else if (k == 1)
                    if (enemyZ == MapManager.DUNGEON_HEIGHT - 1) continue;


                int x = enemyX + i;
                int z = enemyZ + k;
                if (mapMn.dung_2D[z, x] < 0) continue;
                if (mapMn.chara_exist2D[z, x] > -1) continue;

                bool top = (z != MapManager.DUNGEON_HEIGHT - 1);
                bool bottom = (z != 0);
                bool right = (x != MapManager.DUNGEON_WIDTH - 1);
                bool left = (x != 0);
                bool canCarry = false;

                if (top)
                {
                    if (mapMn.dung_2D[z + 1, x] > -1)
                        if (mapMn.chara_exist2D[z + 1, x] < 0)
                            canCarry = true;
                    if (left)
                        if (mapMn.dung_2D[z + 1, x - 1] > -1)
                            if (mapMn.chara_exist2D[z + 1, x - 1] < 0)
                                canCarry = true;
                    if (right)
                        if (mapMn.dung_2D[z + 1, x + 1] > -1)
                            if (mapMn.chara_exist2D[z + 1, x + 1] < 0)
                                canCarry = true;
                }
                if (bottom)
                {
                    if (mapMn.dung_2D[z - 1, x] > -1)
                        if (mapMn.chara_exist2D[z - 1, x] < 0)
                            canCarry = true;
                    if (left)
                        if (mapMn.dung_2D[z - 1, x - 1] > -1)
                            if (mapMn.chara_exist2D[z - 1, x - 1] < 0)
                                canCarry = true;
                    if (right)
                        if (mapMn.dung_2D[z - 1, x + 1] > -1)
                            if (mapMn.chara_exist2D[z - 1, x + 1] < 0)
                                canCarry = true;
                }
                {
                    if (left)
                        if (mapMn.dung_2D[z, x - 1] > -1)
                            if (mapMn.chara_exist2D[z, x - 1] < 0)
                                canCarry = true;
                    if (right)
                        if (mapMn.dung_2D[z, x + 1] > -1)
                            if (mapMn.chara_exist2D[z, x + 1] < 0)
                                canCarry = true;
                }

                if (canCarry)
                {
                    rnd = Random.Range(0, checkCount);
                    if (rnd == 0)
                    {
                        pointX = x;
                        pointZ = z;
                    }
                    checkCount++;
                }
            }
        }

        checkCount = 1;
        for (int i = -1; i < 2; ++i)
        {
            if (i == -1)
            {
                if (pointX == 0) continue;
            }
            else if (i == 1)
                if (pointX == MapManager.DUNGEON_WIDTH - 1) continue;
            for (int k = -1; k < 2; ++k)
            {
                if (i == k && i == 0) continue;
                if (k == -1)
                {
                    if (pointZ == 0) continue;
                }
                else if (k == 1)
                    if (pointZ == MapManager.DUNGEON_HEIGHT - 1) continue;


                int x = pointX + i;
                int z = pointZ + k;
                if (mapMn.dung_2D[z, x] < 0) continue;
                if (mapMn.chara_exist2D[z, x] > -1) continue;
                if (x == enemyX && z == enemyZ) continue;

                rnd = Random.Range(0, checkCount);
                if (rnd == 0)
                {
                    thisX = x;
                    thisZ = z;
                }
                checkCount++;
            }
        }

        //pointX と pointZ にプレイヤーをthisX と thisZ にUFOを移動させる
        {
            targetChara.sPos = new Vector3(pointX, 0, pointZ);
            mapMn.SetCharaExistInfo(targetChara.pos);
            mapMn.SetCharaExistInfo(targetChara.sPos, targetChara.idNum, true);

            thisChara.sPos = new Vector3(thisX, 0, thisZ);
            mapMn.SetCharaExistInfo(thisChara.pos);
            mapMn.SetCharaExistInfo(thisChara.sPos, thisChara.idNum, true);

            int d_x = (int)(target.sPos.x - thisChara.pos.x);
            int d_z = (int)(target.sPos.z - thisChara.pos.z);
            if (d_x != 0) d_x = (d_x > 0) ? 1 : -1;
            if (d_z != 0) d_z = (d_z > 0) ? 1 : -1;
            thisChara.charaDir = new Vector3(d_x, 0, d_z);
        }
    }

    void Update () {

        // ↓anim が null だと 行動完了に到達しなくなるので、削除
        //if (anim == null) return;
        //if (targetChara == null)
        //{
        //    FindLightMonster();
        //}

        if (!actionStart) return;

        // アニメーションやオブジェクトの位置など、見た目の処理
        targetChara.pos = targetChara.sPos;
        targetChara.gameObject.transform.position = targetChara.sPos;

        thisChara.pos = thisChara.sPos;
        thisChara.gameObject.transform.position = thisChara.sPos;
        thisChara.SetObjectDir();

        thisChara.ActEnd();
        Destroy(gameObject);
    }

    IEnumerator Coroutine()
    {
        //for(float t = 0; t < )

        yield return null;
    }
}
