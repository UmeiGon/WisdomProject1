using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum EnemyType
{
    NORMAL,
    ARCHER,
    //STONE,
    UFO,
    //ROLL,
    TREASURE,
    SWORD,
    GHOST,
    //HAMMER,
    MAKEWALL,
    LIGHT
}

public class Enemy : BattleParticipant
{
    // エネミーのステータス、状態異常、行動選択を管理

    public EnemyType type = 0;
    AImove thisAImove = null;
    [SerializeField]
    List<GameObject> myBodys;

    // 倍速モンスターは 0.5f
    float needActGauge = 1;

    // 倍速時は 2
    float moveSpeed = 1;
    [SerializeField]
    List<GameObject> skills;
    // 行動選択してターンが来るまでスキルを準備しておくリスト
    List<GameObject> prepareSkills;
    List<ActionData> thisTurnAction = new List<ActionData>();
    AnimationChanger anim = null;
    SpecialActAI spAI;

    public int RewardExp = 0;

    public override void Start()
    {
        //base.Start();

    }

    public override void Init()
    {
        base.Init();

        // 敵の強さ初期設定
        //HP = 15;
        //MaxHP = 15;


        thisAImove = GetComponent<AImove>();
        thisAImove.Init();

        Body[] bodys = GetComponentsInChildren<Body>();
        foreach ( Body body in bodys )
        {
            myBodys.Add(body.gameObject);
        }

        spAI = GetComponent<SpecialActAI>();
        if ( type == EnemyType.SWORD )
        {
            needActGauge = 0.5f;
        }
        prepareSkills = new List<GameObject>();

        anim = GetComponent<AnimationChanger>();
        anim.Init();

    }

    public void PlusActionGauge()
    {
        float plusActionGauge = 1;
        // プレイヤー倍速時
        if ( player.abnoState.spdUpTurn > 0 ) plusActionGauge = 0.5f;

        actionGauge += plusActionGauge;

        // 行動選択に影響を及ぼす状態異常の処理
        if ( abnoState.freezeTurn > 0 )
        {
            actionGauge = 0;
            return;
        }
        if ( abnoState.paralizeTurn > 0 )
        {
            actionGauge -= 0.5f;
        }

    }

    public void SelectAction()
    {
        int sequence = 0;

        // 倍速モンスターなどの行動後半かどうか
        if ( needActGauge <= 0.5f && actionGauge <= needActGauge * 1 )
        {
            sequence = 1;
        }

        // actionGauge を必要分消費して行動可能とする
        if ( actionGauge < needActGauge )
        {
            // 行動不可
            //action = ActionType.NON_ACTION;
            //turnMn.AddAction(this, action, sequence, Vector3.zero);
            pos = transform.position;
            return;
        }
        else actionGauge -= needActGauge;



        // 死亡時行動不可
        //if (action == ActionType.DEAD) return;

        // 特殊行動の判定
        bool specialAct = false;
        if (spAI != null)
        {
            specialAct = spAI.ShouldSpecialAct();
            if (specialAct)
            {
                Debug.Log("特殊行動しました");
                if (!skills[1])
                {
                    // 特殊攻撃持たないのに特殊攻撃選択
                    return;
                }
                var sp = Instantiate(skills[1], transform);
                prepareSkills.Add(sp);
                NPCSkill _speSkill = sp.GetComponent<NPCSkill>();
                _speSkill.thisChara = this;
                _speSkill.target = GetComponent<SpecialActAI>().targetChara;
                _speSkill.Init();
                _speSkill.Prepare();
                ActionData addAction = new ActionData(this, ActionType.ENEMY_SPECIAL, 0, sequence, Vector3.zero);
                thisTurnAction.Add(addAction);
                turnMn.AddAction(addAction);
            }
        }

        if (!specialAct) // falseだったので移動または通常攻撃を選択
        {
            // 付属のAImove が移動すべきと判定すれば true
            if (thisAImove.GetMoveVec() )
            {
                sPos = pos + moveVec;
                moveDir = moveVec;
                if (moveDir != Vector3.zero) charaDir = moveDir;
                mapMn.SetCharaExistInfo(pos);
                mapMn.SetCharaExistInfo(sPos, idNum, true);
                // 倍速時の一手先行動選択のため pos に sPos を入れる
                pos = sPos;
                ActionData addAction = new ActionData(this, ActionType.MOVE, 0, sequence, sPos);
                thisTurnAction.Add(addAction);
                turnMn.AddAction(addAction);
            }
            else // falseだったので通常攻撃を選択
            {
                var atk = Instantiate(skills[0], transform);
                prepareSkills.Add(atk);
                var norSkill = atk.GetComponent<NPCSkill>();
                norSkill.thisChara = this;
                norSkill.Init();
                norSkill.Prepare();
                norSkill.target = target;
                ActionData addAction = new ActionData(this, ActionType.ATTACK, 0, sequence, Vector3.zero);
                thisTurnAction.Add(addAction);
                turnMn.AddAction(addAction);
            }
        }
        transform.LookAt(pos + charaDir);
        ParamChangeByTurn();
    }
    Coroutine cort;

    //public override void CharaUpdate()
    //{
    //    if ( nowAct != null ) { /*Debug.Log(idNum + " Exist:nowAct");*/ }

    //    base.CharaUpdate();

    //    DeathCheck();
    //    if (thisAImove == null) thisAImove = GetComponent<AImove>();
    //    //if (anim == null) anim = GetComponent<AnimationChanger>();
    //    if (!actAllowed) return;
    //    if (nowAct == null) return;

    //    // 行動処理
    //    switch (nowAct.actType)
    //    {
    //        case ActionType.NON_ACTION:
    //            ActEnd();
    //            break;
    //        case ActionType.MOVE:
    //            if ( actStarted ) return;
    //            if ( needActGauge < 1 ) moveSpeed = 2;
    //            cort = StartCoroutine(MoveCoroutine());
    //            actStarted = true;
    //            break;
    //        case ActionType.ATTACK:
    //            if (actStarted) return;
    //            prepareSkills[0].GetComponent<NPCSkill>().actionStart = true;
    //            actStarted = true;
    //            break;
    //        case ActionType.ENEMY_SPECIAL:
    //            if (actStarted) return;
    //            prepareSkills[0].GetComponent<NPCSkill>().actionStart = true;
    //            actStarted = true;
    //            break;
    //        default:
    //            break;
    //    }
    //    //// 倍速モンスターなら次の行動の準備
    //    //if (action == ActionType.NON_ACTION && actionGauge >= needActGauge)
    //    //{
    //    //    actStarted = false;
    //    //    SelectAction();
    //    //}
    //}

    public override void CharaUpdate()
    {
        base.CharaUpdate();

        DeathCheck();
        if ( thisAImove == null ) thisAImove = GetComponent<AImove>();
        //if (anim == null) anim = GetComponent<AnimationChanger>();
        if ( thisTurnAction.Count <= 0 ) return;
        if ( !thisTurnAction[0].allowed ) return;

        // 行動処理
        switch ( thisTurnAction[0].actType )
        {
            case ActionType.NON_ACTION:
                ActEnd();
                break;
            case ActionType.MOVE:
                ActionData act = thisTurnAction[0];

                // 1ターンに2回移動するなら倍速になる
                if ( thisTurnAction.Count >= 2
                && thisTurnAction[1].actType == ActionType.MOVE
                && thisTurnAction[1].allowed )
                {
                    act.speedRate = 2;
                    thisTurnAction[1].speedRate = 2;
                }
                // プレイヤーからかなり遠い所でのActionなら描写を早く終わらせる
                if ( act.actionRate == 0 )
                {
                    Vector3 disToPlayer = act.pos - player.pos;
                    if ( disToPlayer.sqrMagnitude > 10 * 10 )
                    {
                        act.speedRate = 100;
                    }
                }
                // 経時変化
                act.actionRate += Time.deltaTime * turnMn.moveSpeed * act.speedRate;
                if ( act.actionRate >= 1 )
                {
                    // 移動終了
                    act.finish = true;
                    transform.position = act.pos;
                    pos = act.pos;
                    ActEnd();
                }
                else
                {
                    // 移動の途中
                    Vector3 distance = ( act.pos - pos ) * act.actionRate;
                    transform.position = pos + distance;
                }
                break;
            case ActionType.ATTACK:
                if ( actStarted ) return;
                prepareSkills[0].GetComponent<NPCSkill>().actionStart = true;
                actStarted = true;
                break;
            case ActionType.ENEMY_SPECIAL:
                if ( actStarted ) return;
                prepareSkills[0].GetComponent<NPCSkill>().actionStart = true;
                actStarted = true;
                break;
            default:
                break;
        }
        //// 倍速モンスターなら次の行動の準備
        //if (action == ActionType.NON_ACTION && actionGauge >= needActGauge)
        //{
        //    actStarted = false;
        //    SelectAction();
        //}
    }
    public override void UpdateAbnoEffect()
    {
        // 凍結
        if ( abnoState.freezeTurn > 0 && abnoEffect[0] == null )
        {
            abnoEffect[0] = Instantiate(parent.GetComponentInChildren<AbnormalEffect>().abnoEffect[0]);
            abnoEffect[0].transform.position = this.transform.position;
            abnoEffect[0].transform.parent = this.transform;
            Debug.Log("Freeze!");
        }
        if ( abnoState.freezeTurn <= 0 )
        {
            if ( abnoEffect[0] ) Destroy(abnoEffect[0]);
        }

        // 感電
        if ( abnoState.paralizeTurn > 0 && abnoEffect[1] == null )
        {
            abnoEffect[1] = Instantiate(parent.GetComponentInChildren<AbnormalEffect>().abnoEffect[1]);
            abnoEffect[1].transform.position = this.transform.position;
            abnoEffect[1].transform.parent = this.transform;
            Debug.Log("Paralize!");
        }
        if ( abnoState.paralizeTurn <= 0 )
        {
            if ( abnoEffect[1] ) Destroy(abnoEffect[1]);
        }
    }

    public override void UpdateAbnoParam()
    {
        if ( abnoState.freezeTurn > 0 ) abnoState.freezeTurn--;

        if ( abnoState.paralizeTurn > 0 ) abnoState.paralizeTurn--;
    }


    IEnumerator MoveCoroutine()
    {
        float timeMax = turnMn.MoveTimeMax;
        
        timeMax /= moveSpeed;
        sPos = nowAct.pos;
        Vector3 _moveVec = sPos - pos;
        anim.TriggerAnimator("Move");
        for ( float moveTime = turnMn.MoveTime - nowAct.sequence * timeMax ; moveTime < timeMax; moveTime = turnMn.MoveTime - nowAct.sequence * timeMax )
        {
            float moveRate = moveTime / timeMax;
            Vector3 _moveDis = _moveVec * moveRate;
            transform.position = pos + _moveDis;
            yield return null;
        }
        // 移動終了
        transform.position = sPos;
        pos = sPos;
        anim.TriggerAnimator("Move", false);
        ActEnd();
    }

    protected override void DeathCheck()
    {
        if ( HP <= 0 && isAlive )
        {
            KillReward();
            var em = parent.GetComponentInChildren<EnemyManager>();
            mapMn.SetCharaExistInfo(sPos);
            foreach ( GameObject deadObj in deadObjPrefab )
            {
                var obj = Instantiate(deadObj);
                obj.transform.position = transform.position;
                obj.transform.rotation = transform.rotation;
                Destroy(obj, 5.0f);
            }
            foreach ( GameObject aliveBody in myBodys )
            {
                Destroy(aliveBody);
            }
            isAlive = false;
        }
    }

    void KillReward()
    {
        Player player = parent.GetComponentInChildren<Player>();
        player.ExpGet(RewardExp);
        PlayerItem itMn = player.GetComponent<PlayerItem>();

        itMn.items[0].kosuu += 1;
        itMn.items[1].kosuu += 1;
        itMn.items[2].kosuu += 1;
    }

    int turnActNum = 0;

    public override bool ActStart(int _turnActNum, ActionData _data)
    {
        // もうすでに行動中ならば
        if ( nowAct != null ) return false;

        //action = _data.actType;
        nowAct = _data;
        // 移動は nowAct.pos に従う（倍速移動もあるため）

        turnActNum = _turnActNum;
        actAllowed = true;
        if (HP <= 0 )
        {
            ActEnd();
        }
        return true;
    }

    public override void ActEnd()
    {
        if ( thisTurnAction[0].actType >= ActionType.ATTACK )
        {
            if ( prepareSkills[0] )
                prepareSkills.RemoveAt(0);
        }
        //nowAct = null;
        //cort = null;
        thisTurnAction[0].finish = true;
        thisTurnAction.RemoveAt(0);

        actStarted = false;
        actAllowed = false;
        //acted = true;
        // ターンで変化する状態異常の更新
        UpdateAbnoParam();
        UpdateAbnoEffect();

        //turnMn.OneActEnd(turnActNum);
    }
    
}