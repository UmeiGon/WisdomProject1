using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcherSpecialAttack : NPCSkill {

    float _timeCount = 0;
    float _needTime = 0;
    [SerializeField]
    float _arrowSpeed = 10.0f;
    [SerializeField]
    GameObject effect;

	// Use this for initialization
	protected override void Start () {

        base.Start();
        SetNeedTime();
	}
	
	// Update is called once per frame
	void Update () {
		if (actionStart)
        {
            StartCoroutine(Coroutine());
            actionStart = false;
        }
	}

    IEnumerator Coroutine()
    {
        anim.TriggerAnimator("Special");
        yield return new WaitForSeconds(0.25f);
        var eff = Instantiate(effect, transform);

        int d_x = (int)(target.sPos.x - thisChara.pos.x);
        int d_z = (int)(target.sPos.z - thisChara.pos.z);
        if (d_x != 0) d_x = (d_x > 0) ? 1 : -1;
        if (d_z != 0) d_z = (d_z > 0) ? 1 : -1;
        thisChara.charaDir = new Vector3(d_x, 0, d_z);
        thisChara.SetObjectDir();

        for (float t = 0; t < _needTime; t += Time.deltaTime)
        {
            // 矢が飛んでく
            Vector3 dis = target.pos - thisChara.pos;
            eff.transform.position = thisChara.pos + dis * (t / _needTime);
            yield return null;
        }
        // ターゲットにヒット
        // ダメージ
        target.DamageParameter(3);
        // 行動完了フラグ
        thisChara.ActEnd();
        // 後始末
        Destroy(gameObject);
        yield return null;
    }

    public void SetNeedTime()
    {
        Vector3 dis = target.pos - thisChara.pos;
        _needTime = dis.magnitude / _arrowSpeed;
    }
}
