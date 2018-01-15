using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AtkAndDef : MonoBehaviour {

    GameObject parent;
    BattleParticipant thisChara;
    MapManager mapMn;

    [SerializeField]
    float base_magic_power = 1.0f;
    public float BaseMagicPower { get { return base_magic_power; } set { base_magic_power = value; } }

    [SerializeField]
    float normal_power = 1.0f;
    public float NormalPower { get { return normal_power; } set { normal_power = value; } }
    [SerializeField]
    float flame_power = 1.0f;
    public float FlameMagicPower { get { return flame_power; } set { flame_power = value; } }
    [SerializeField]
    float light_power = 1.0f;
    public float LightMagicPower { get { return light_power; } set { light_power = value; } }
    [SerializeField]
    float ice_power = 1.0f;
    public float IceMagicPower { get { return ice_power; } set { ice_power = value; } }

    [SerializeField]
    float normal_resist = 1.0f;
    [SerializeField]
    float flame_resist = 1.0f;
    [SerializeField]
    float light_resist = 1.0f;
    [SerializeField]
    float ice_resist = 1.0f;

    // Use this for initialization
    void Start () {
        parent = GameObject.Find("GameObjectParent");
        thisChara = GetComponent<BattleParticipant>();
        mapMn = parent.GetComponentInChildren<MapManager>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    // ダメージ与える側の計算
    public float CalcPower(SkillBase.TYPE type, float row_power)
    {
        float _powerF = row_power;
        if (thisChara.abnoState.atkUpTurn > 0 )
        {
            _powerF *= 2.0f;
        }
        // 属性耐性
        switch ( type )
        {
            case SkillBase.TYPE.NO_ELEMENT:
                _powerF *= normal_power;
                break;
            case SkillBase.TYPE.FLAME:
                _powerF *= base_magic_power;
                _powerF *= flame_power;
                break;
            case SkillBase.TYPE.LIGHTNING:
                _powerF *= base_magic_power;
                _powerF *= light_power;
                break;
            case SkillBase.TYPE.ICE:
                _powerF *= base_magic_power;
                _powerF *= ice_power;
                break;
        }
        // 地形効果

        // float のまま返す
        return _powerF;
    }

    // ダメージ受ける側の計算
    public int CalcDamage(SkillBase.TYPE type, float row_damage)
    {
        float _damageF = row_damage;
        if(thisChara.abnoState.invincibleTurn > 0 )
        {
            _damageF = 0;
        }
        // 属性耐性
        switch (type)
        {
            case SkillBase.TYPE.NO_ELEMENT:
                _damageF *= normal_resist;
                break;
            case SkillBase.TYPE.FLAME:
                _damageF *= flame_resist;
                break;
            case SkillBase.TYPE.LIGHTNING:
                _damageF *= light_resist;
                break;
            case SkillBase.TYPE.ICE:
                _damageF *= ice_resist;
                break;
        }
        // 地形効果

        // 四捨五入して返す
        return (int)Mathf.Round(_damageF);
    }
}
