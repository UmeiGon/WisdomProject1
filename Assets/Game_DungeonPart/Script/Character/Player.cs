using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Player : BattleParticipant {

    // プレイヤーのステータス、状態異常を管理

    public int MP;
    public int MaxMP;
    public int Stamina;
    List<KeyValuePair<int, int>> _haveItem = new List<KeyValuePair<int, int>>();

    public int Exp = 0;
    ExpTable expTable = new ExpTable();
    int[] hpGainByLevel = {
         4, 4, 4, 5, 5, 5, 6, 6, 6, 7, 7, 7, 8, 8, 8,
         9, 9, 9,10,10,10,11,11,11,12,12,12,13,13,13,
        14,14,14,15,15,15,16,16,16,17,17,17,18,18,18,
        19,19,19,20,20,20,21,21,21,22,22,22,23,23,23 };
    int[] mpGainByLevel = {
         4, 4, 4, 5, 5, 5, 6, 6, 6, 7, 7, 7, 8, 8, 8,
         9, 9, 9,10,10,10,11,11,11,12,12,12,13,13,13,
        14,14,14,15,15,15,16,16,16,17,17,17,18,18,18,
        19,19,19,20,20,20,21,21,21,22,22,22,23,23,23 };

    float hpRegene = 0;
    float mpRegene = 0;

    AnimationChanger anim = null;

    float deadTime = 0;
    DungeonPartManager dMn;
    PlayerItem playerItem;
    [SerializeField]
    Light _plSpotLight;
    [SerializeField]
    Light _plPointLight;

    public List<ActionData> thisTurnAction = new List<ActionData>();

    private void Start()
    {
        // セーブデータからHPをロード
        //hp = GameData.HP;
        //base.Start();
    }


    public override void Init()
    {
        idNum = 1;
        base.Init();

        ExpGet(0);
        UpdateLightRange();
        anim = GetComponent<AnimationChanger>();
        dMn = parent.GetComponentInChildren<DungeonPartManager>();
        playerItem = GetComponent<PlayerItem>();
        // ↓今は意味なし
        //mapMn.DarkenSet();
    }
    public override void CharaUpdate()
    {
        base.CharaUpdate();
        if (HP <= 0)
        {
            if (deadTime == 0)
            {
                anim.TriggerAnimator("Dead");
                dMn.SaveDataReset();
            }
            deadTime += Time.deltaTime;
            if(deadTime >= 3)
            {
                UnityEngine.SceneManagement.SceneManager.LoadScene("GameOver");
            }
        }
    }
    public override void DamageParameter(float value
    , ParamType para = ParamType.HP, SkillBase.TYPE element = SkillBase.TYPE.NO_ELEMENT)
    {
        int damage = atkAndDef.CalcDamage(element, value);
        switch (para)
        {
            case ParamType.HP:
                HP -= damage;
                break;
            case ParamType.MP:
                MP -= damage;
                break;
            case ParamType.STAMINA:
                Stamina -= damage;
                break;
            default:
                break;
        }
        anim.TriggerAnimator("Damaged");
        dmgEffMn.CreateDamagerText(sPos, damage * -1);
    }

    int preLevel = 0;

    public void ExpGet(int point)
    {
        preLevel = Level;
        Exp += point;
        Level = expTable.GetLevel(Exp);
        if (preLevel != Level)
        {
            LevelUp();
        }
    }

    public void LevelUp()
    {
        //int d_level = Level - preLevel;
        //while(d_level > 0)
        //{
        //    MaxHP += hpGainByLevel[preLevel - 1];
        //    MaxMP += mpGainByLevel[preLevel - 1];
        //    d_level--;
        //}

        MaxHP = expTable.GetHP(Level);
        MaxMP = expTable.GetMP(Level);
    }

    public bool MpUseSkill(int useMp)
    {
        if (MP < useMp) return false;
        MP -= useMp;
        return true;
    }
    public override void ParamChangeByTurn()
    {
        hpRegene += MaxHP * 0.02f;
        mpRegene += MaxMP * 0.06f;

        float hpRege = Mathf.Floor(hpRegene);
        float mpRege = Mathf.Floor(mpRegene);

        HP = Mathf.Min(MaxHP, HP + (int)hpRege);
        MP = Mathf.Min(MaxMP, MP + (int)mpRege);

        hpRegene -= hpRege;
        mpRegene -= mpRege;
        base.ParamChangeByTurn();
    }
    public void LoadPlayerInfo()
    {
        HP = SaveData.GetInt("HP", 15);
        MaxHP = SaveData.GetInt("MaxHP", 15);
        MP = SaveData.GetInt("MP", 15);
        MaxMP = SaveData.GetInt("MaxMP", 15);
        Exp = SaveData.GetInt("EXP", 0);
        Level = SaveData.GetInt("Level", 1);
        Stamina = SaveData.GetInt("Stamina", 100);
    }

    public void SavePlayerInfo()
    {
        SaveData.SetInt("HP", HP);
        SaveData.SetInt("MaxHP", MaxHP);
        SaveData.SetInt("MP", MP);
        SaveData.SetInt("MaxMP", MaxMP);
        SaveData.SetInt("EXP", Exp);
        SaveData.SetInt("Level", Level);
        SaveData.SetInt("Stamina", Stamina);
        SavePlayerPos();
        playerItem.SaveItemData();
    }
    
    public class PosData
    {
        public int PosX;
        public int PosZ;
        public int DirX;
        public int DirZ;
    }

    public void SavePlayerPos()
    {
        PosData _data = new PosData();
        _data.PosX = (int)pos.x;
        _data.PosZ = (int)pos.z;
        _data.DirX = (int)charaDir.x;
        _data.DirZ = (int)charaDir.z;
        SaveData.SetClass<PosData>("PlayerPosData", _data);
        SaveData.Remove("PlayerX");
        SaveData.Remove("PlayerZ");
    }

    public void UpdateLightRange()
    {
        if(existRoomNum < mapMn.max_room)
        {
            _plSpotLight.range = 23;
            _plSpotLight.spotAngle = 50;
            _plPointLight.range = 7;
        }
        else
        {
            _plSpotLight.range = 23;
            _plSpotLight.spotAngle = 35;
            _plPointLight.range = 6;
        }
    }

    public void DebugExpGet()
    {
        ExpGet(100);
    }

    public void DebugHeal()
    {
        HP = MaxHP;
        MP = MaxMP;
    }

    public void DebugSoulStone()
    {
        PlayerItem itMn = GetComponent<PlayerItem>();
        itMn.items[0].kosuu += Random.Range(1, 6);
        itMn.items[1].kosuu += Random.Range(1, 6);
        itMn.items[2].kosuu += Random.Range(1, 6);
    }

    public void PlayerActSelect()
    {
        turnMn.PlayerActSelect();
        ParamChangeByTurn();
        UpdateLightRange();
    }

    int turnActNum = 0;
    public override bool ActStart(int _turnActNum, ActionData _data)
    {
        nowAct = _data;
        action = nowAct.actType;
        turnActNum = _turnActNum;
        return true;
    }
    public override void ActEnd()
    {
        nowAct = null;
        action = ActionType.NON_ACTION;
        thisTurnAction[0].finish = true;
        thisTurnAction.RemoveAt(0);
        turnMn.OneActEnd(turnActNum);

        UpdateAbnoParam();
        UpdateAbnoEffect();
    }

    PlayerSkillTree pst = null;

    public void SaveSkill()
    {
        if ( !pst ) pst = GetComponent<PlayerSkillTree>();
        pst.SaveSkillData();
    }

    public void ElementLevelUp(int type)
    {

        switch( (SkillBase.TYPE) type )
        {
            case SkillBase.TYPE.FLAME:
                atkAndDef.FlameMagicPower *= 1.1f;
                break;
            case SkillBase.TYPE.LIGHTNING:
                atkAndDef.LightMagicPower *= 1.1f;
                break;
            case SkillBase.TYPE.ICE:
                atkAndDef.IceMagicPower *= 1.1f;
                break;
        }
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

        // 攻撃力アップ
        if ( abnoState.atkUpTurn > 0 && abnoEffect[2] == null )
        {
            abnoEffect[2] = Instantiate(parent.GetComponentInChildren<AbnormalEffect>().abnoEffect[2]);
            abnoEffect[2].transform.position = this.transform.position;
            abnoEffect[2].transform.parent = this.transform;
            Debug.Log("AttackUp!");
        }
        if ( abnoState.atkUpTurn <= 0 )
        {
            if ( abnoEffect[2] ) Destroy(abnoEffect[2]);
        }

        // 無敵化
        if ( abnoState.invincibleTurn > 0 && abnoEffect[3] == null )
        {
            abnoEffect[3] = Instantiate(parent.GetComponentInChildren<AbnormalEffect>().abnoEffect[3]);
            abnoEffect[3].transform.position = this.transform.position;
            abnoEffect[3].transform.parent = this.transform;
            Debug.Log("InvincibleTime!");
        }
        if ( abnoState.invincibleTurn <= 0 )
        {
            if ( abnoEffect[3] ) Destroy(abnoEffect[3]);
        }

        // 倍速
        if ( abnoState.spdUpTurn > 0 && abnoEffect[4] == null )
        {
            abnoEffect[4] = Instantiate(parent.GetComponentInChildren<AbnormalEffect>().abnoEffect[4]);
            abnoEffect[4].transform.position = this.transform.position;
            abnoEffect[4].transform.parent = this.transform;
            Debug.Log("SpeedUp!");
        }
        if ( abnoState.spdUpTurn <= 0 )
        {
            if ( abnoEffect[4] ) Destroy(abnoEffect[4]);
        }
    }

    public override void UpdateAbnoParam()
    {
        if ( abnoState.freezeTurn > 0 ) abnoState.freezeTurn--;
        if ( abnoState.paralizeTurn > 0 ) abnoState.paralizeTurn--;
        if ( abnoState.atkUpTurn > 0 ) abnoState.atkUpTurn--;
        if ( abnoState.invincibleTurn > 0 ) abnoState.invincibleTurn--;
        if ( abnoState.spdUpTurn > 0 ) abnoState.spdUpTurn--;
    }
}

