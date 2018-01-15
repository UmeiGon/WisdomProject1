using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetPlayerMove : AImove {

    BattleParticipant targetChara;
    [SerializeField] Vector3 targetPos = new Vector3(-1, 0, 0);
    //thisChara.type　でタイプを取得できる
    int preExistRoomNum = 0;
    
    void Update () {
		
	}

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
                if (d_x < searchTargetRange)
                {
                    if (d_z < searchTargetRange)
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

    public override bool GetMoveVec()
    {

        //光かプレイヤかの判断する関数を呼ぶ
        FindLightMonster();

        bool targetFind = false;
        if (targetChara.existRoomNum >= 0 && targetChara.existRoomNum < mapMn.max_room) // ターゲットが部屋に居る
        {
            targetFind = (targetChara.existRoomNum == thisChara.existRoomNum); // プレイヤーと同部屋にいれば発見
            if (!targetFind && thisChara.existRoomNum >= mapMn.max_room)
            {
                int d_x = Mathf.Abs((int)(targetChara.sPos.x - thisChara.pos.x));
                int d_z = Mathf.Abs((int)(targetChara.sPos.z - thisChara.pos.z));
                targetFind = (d_x <= searchTargetRange && d_z <= searchTargetRange);
            }
        }
        else                                                                            // ターゲットが部屋以外の場所に居る
        {
            int d_x = Mathf.Abs((int)(targetChara.sPos.x - thisChara.pos.x));
            int d_z = Mathf.Abs((int)(targetChara.sPos.z - thisChara.pos.z));
            targetFind = (d_x <= searchTargetRange && d_z <= searchTargetRange);
            //float tDis = (targetChara.sPos - thisChara.pos).magnitude;
            // 平方根とるより軽いやり方がある！
            //targetFind = (tDis <= 1.5f); // 隣接してれば襲ってくる
        }
        if (!targetFind) {
            // 目標地点がある かつ　着いた
            if (targetPos.x != -1 && thisChara.pos == targetPos)                                             
            {
                targetPos.x = -1;   // ←NULLとして扱う
            }
            // 目標地点NULL かつ 敵自身が部屋に居る場合
            if (targetPos.x == -1 && thisChara.existRoomNum >= 0 && thisChara.existRoomNum < mapMn.max_room) 
            {
                // 一番遠い出入り口を目標地点に登録
                Vector3[] gatewayPos = mapMn.room_info[thisChara.existRoomNum].gatewayPos;
                float maxDis = 0;
                int maxNum = 0;
                float minDis = 100;
                int minNum = 0;
                for (int i = 0; i < gatewayPos.Length; i++)
                {
                    if (gatewayPos[i].x == -1) continue;
                    float d = (gatewayPos[i] - thisChara.pos).magnitude;
                    if (d > maxDis)
                    {
                        maxNum = i;
                        maxDis = d;
                    }
                    if (d < minDis)
                    {
                        minNum = i;
                        minDis = d;
                    }
                }
                targetPos = gatewayPos[maxNum];
                // 元々この部屋に居てtargetPosがnullになったということは
                // 目的があってここに来た（プレイヤーなどがいた）のでそのまま近い出入り口を登録
                if(thisChara.existRoomNum == preExistRoomNum)
                {
                    targetPos = gatewayPos[minNum];
                }
            }
            // 目標地点NULL かつ 部屋以外に居る場合
            else if (targetPos.x == -1) 
            {
                Vector3 dir = thisChara.charaDir;               // まっすぐ進む
                if (mapMn.CanMoveCheck(thisChara.pos, thisChara.pos + dir))
                {
                    thisChara.moveVec = dir;
                    return true;
                }
                    dir = Quaternion.Euler(0, 90f * (Random.Range(0, 2) * 2 - 1), 0) * dir;    // 左か右に進む
                if (mapMn.CanMoveCheck(thisChara.pos, thisChara.pos + dir))
                {
                    thisChara.moveVec = dir;
                    return true;
                }
                    dir = Quaternion.Euler(0, -180f, 0) * dir;  // ↑の逆方向（右か左）に進む
                if (mapMn.CanMoveCheck(thisChara.pos, thisChara.pos + dir))
                {
                    thisChara.moveVec = dir;
                    return true;
                }
                dir = Quaternion.Euler(0, -90f, 0) * dir;       // 後ろに進む
                if (mapMn.CanMoveCheck(thisChara.pos, thisChara.pos + dir))
                {
                    thisChara.moveVec = dir;
                    return true;
                }
                thisChara.moveVec = Vector3.zero;
                return true;
            }
            if (targetPos.x != -1)                                                // 目標地点がある
            {
                GetVecToTarget(targetPos);
                if (thisChara.moveVec == Vector3.zero)
                {
                    // 目標地点があるのに移動できない状態の場合
                    targetPos.x = -1; 
                }
                return true;
            }
        }
        targetPos = targetChara.sPos;
        preExistRoomNum = thisChara.existRoomNum;
        Vector3 dis = targetPos - thisChara.sPos;
        if (dis.magnitude < 1.5f) // ルート２以下＝隣接している
        {
            
            if ( mapMn.DiagonalCheck(thisChara.pos, thisChara.pos + dis) )
            {
                // 通常攻撃が可能なので false
                thisChara.target = targetChara;
                thisChara.charaDir = dis;
                return false;
            }
        }
        // 攻撃できないならプレイヤーへのベクトルを設定する
        return GetVecToTarget(targetPos);
    }
    
   private bool GetVecToTarget(Vector3 targetPos)
    {
        //Vector3 moveVec = Vector3.zero;
        //Vector3 dis = targetPos - transform.position;

        //if (dis == Vector3.zero) return false;

        //if (dis.x > 0) moveVec.x = 1;
        //if (dis.x < 0) moveVec.x = -1;
        //if (dis.z > 0) moveVec.z = 1;
        //if (dis.z < 0) moveVec.z = -1;

        //Vector3 tmpVec = Vector3.zero;
        //float x_abs = dis.x * dis.x;
        //float z_abs = dis.z * dis.z;

        //bool can_move = false;
        //can_move = mapMn.CanMoveCheck(transform.position, transform.position + moveVec);
        //if (!can_move)
        //{
        //    if (moveVec.x == 0) // z方向のみならナナメにしてみる
        //    {
        //        tmpVec = moveVec;
        //        moveVec.x = Random.Range(0, 2) * 2 - 1;
        //    }
        //    if (moveVec.z == 0) // x方向のみならナナメにしてみる
        //    {
        //        tmpVec = moveVec;
        //        moveVec.z = Random.Range(0, 2) * 2 - 1;
        //    }
        //    can_move = mapMn.CanMoveCheck(transform.position, transform.position + moveVec);
        //}
        //if (!can_move)
        //{
        //    if (tmpVec.z != 0) // もとはz方向のみならx方向を反転
        //    {
        //        moveVec.x *= -1;
        //    }
        //    if (tmpVec.x != 0) // もとはx方向のみならz方向を反転
        //    {
        //        moveVec.z *= -1;
        //    }
        //    can_move = mapMn.CanMoveCheck(transform.position, transform.position + moveVec);
        //}
        ////if (!can_move)
        ////{
        ////    if (tmp != 0 && moveVec.z == 0)
        ////    {
        ////        moveVec.x *= -1;
        ////    }
        ////    if (tmp != 0 && moveVec.x == 0)
        ////    {
        ////        moveVec.z *= -1;
        ////    }
        ////    can_move = mapMn.CanMoveCheck(transform.position, transform.position + moveVec);
        ////}

        //// ナナメが無理なら
        //if (!can_move)
        //{
        //    if (x_abs >= z_abs)
        //    {
        //        tmpVec = moveVec;
        //        moveVec.z = 0;
        //    }
        //    else
        //    {
        //        tmpVec = moveVec;
        //        moveVec.x = 0;
        //    }
        //    can_move = mapMn.CanMoveCheck(transform.position, transform.position + moveVec);
        //}
        //if (!can_move)
        //{
        //    if (x_abs >= z_abs)
        //    {
        //        moveVec = tmpVec;
        //        moveVec.x = 0;
        //    }
        //    else
        //    {
        //        moveVec = tmpVec;
        //        moveVec.z = 0;
        //    }
        //    can_move = mapMn.CanMoveCheck(transform.position, transform.position + moveVec);
        //}

        //if (!can_move)
        //{
        //    moveVec.x = 0;
        //    moveVec.z = 0;
        //}
        //thisChara.moveVec = moveVec;

        Vector3 dis = targetPos - transform.position;
        if (dis == Vector3.zero)
            return false;
        return AStarN(targetPos);
    }

}
