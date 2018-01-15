using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NPCSkill : MonoBehaviour {

    [SerializeField]
    SkillBase.TYPE element = SkillBase.TYPE.NO_ELEMENT;
    [SerializeField]
    int rowPower = 3;
    float calcPower = 0;
    protected bool init = false;
    public BattleParticipant thisChara;
    public BattleParticipant target = null;
    public float timeCount = 0;
    public bool actionStart = false;

    public GameObject parent;
    public MapManager mapMn;
    protected EnemyManager eneMn;
    public AnimationChanger anim = null;

    // Use this for initialization
    protected virtual void Start () {

    }

    public virtual void Init()
    {
        parent = GameObject.Find("GameObjectParent");
        mapMn = parent.GetComponentInChildren<MapManager>();
        eneMn = parent.GetComponentInChildren<EnemyManager>();
        anim = thisChara.GetComponent<AnimationChanger>();
        init = true;

        AtkAndDef atkAndDef = thisChara.GetComponent<AtkAndDef>();
        // 自身の攻撃力などで補正
        if ( atkAndDef ) calcPower = thisChara.GetComponent<AtkAndDef>().CalcPower(element, rowPower);
        else calcPower = (int)Mathf.Round(rowPower);
    }

    public virtual void Prepare()
    {

    }

    public virtual void TargetDamage()
    {

        // 計算した power を対象者へ
        target.DamageParameter(calcPower, ParamType.HP, element);
    }
}
