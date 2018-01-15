﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireRoom : RoomMagic {

    public override IEnumerator Coroutine()
    {
        _camera.SetIsBigMagicCamera(true);
        anim.TriggerAnimator("Magic_forward2");
        GetComponent<AudioSource>().Play();
        yield return new WaitForSeconds(0.5f);
        _camera.SetIsBigMagicCamera(false);
        if (player.existRoomNum < mapMn.max_room)
        {
            for (int p = 0; p < _particleMax; p++)
            {
                int num = Random.Range(0, range.Count);
                var eff = Instantiate(effects[0]);
                eff.transform.position = new Vector3(range[num].x, 0, range[num].y);
                yield return new WaitForSeconds(0.2f);
            }
        }
        else
        {
            foreach (Vector2 ran in range)
            {
                var eff = Instantiate(effects[0]);
                eff.transform.position = new Vector3(ran.x, 0, ran.y);
                yield return new WaitForSeconds(0.2f);
            }
        }
        //yield return new WaitForSeconds(0.25f);
        //var shotEff = Instantiate(effects[0]);
        ////var shotEff = EffectManager.Get(enum::Num)
        //shotEff.transform.position = transform.position + playerMove.charaDir * 0.5f;
        //shotEff.transform.rotation = transform.rotation;
        //var gso = shotEff.GetComponent<GoStraightObject>();
        //gso.SetTarget(hitPos);
        //if (battleTarget) gso.target = battleTarget;
        //gso.actionParent = this.gameObject;
        HitAndParamChange();
        yield return null;
    }
}
