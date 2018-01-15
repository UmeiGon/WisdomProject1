using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyNormalAttack : NPCSkill {

    [SerializeField]
    GameObject hitEff;
	
	// Update is called once per frame
	void Update () {
        if (anim == null) return;
        if (!actionStart) return;
        //if (target == null)
        //{
        //    Vector3 tarPos = thisChara.pos + thisChara.charaDir;
        //    int tarCharaNum = mapMn.chara_exist2D[(int)tarPos.z, (int)tarPos.x];
        //    int playerNum = parent.GetComponentInChildren<PlayerMove>().charaID;
        //    if (tarCharaNum == playerNum)
        //    {
        //        target = parent.GetComponentInChildren<Player>();
        //        //target = Game.Instance.Player;
        //        Debug.Log("PlayerTarget");
        //    }
        //}
        if (timeCount < 0.1f)
        {
            anim.TriggerAnimator("Attack");
            thisChara.charaDir = target.sPos - thisChara.sPos;
            thisChara.SetObjectDir();
            //GetComponent<AudioSource>().Play();
            timeCount = 0.1f;
        }
        if (timeCount > 0.5f && timeCount < 0.8f)
        {
            if (target != null)
            {
                TargetDamage();
                if ( hitEff )
                {
                    var hit = Instantiate(hitEff, target.transform);
                    Destroy(hit, 2.0f);
                }
                //Game.Instance.Player.hp -= 4;
                //Debug.Log("PlayerDamage");
            }
            timeCount = 0.8f;
            thisChara.ActEnd();
            Destroy(gameObject);
        }
        timeCount += Time.deltaTime;
	}
}
