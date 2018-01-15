using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerActionAnimation : MonoBehaviour {

    GameObject _parent;
    Player _player;
    PlayerMove _playerMove;
    AnimationChanger _anim;
    float _timeCount = 0;
    int _attackType = -1;

    [SerializeField]
    public GameObject[] acts;

    [SerializeField]
    public Dictionary<int, GameObject> skills;

    // Use this for initialization
    void Start () {
        _parent = GameObject.Find("GameObjectParent");
        _player = GetComponent<Player>();
        _playerMove = GetComponent<PlayerMove>();
        _anim = GetComponent<AnimationChanger>();
        Init();
	}
	
	// Update is called once per frame
	void Update () {
        //if (_attackType != -1)
        //{
        //    int _useMp = skills[_attackType].GetComponent<SkillBase>().useMp;
        //    if (!_player.MpUseSkill(_useMp)) _attackType = 0;      // MP足りなかったら物理攻撃を自動選択
        //    var _act = Instantiate(skills[_attackType], transform);
        //    _act.GetComponent<SkillBase>().shouldActionStart = true;
        //    _attackType = -1;
        //}
	}

    public void SetAttackNumber(int num)
    {
        _attackType = num;
        if ( _attackType != -1 )
        {
            int _useMp = skills[_attackType].GetComponent<SkillBase>().useMp;
            if ( !_player.MpUseSkill(_useMp) ) _attackType = 0;      // MP足りなかったら物理攻撃を自動選択
            var _act = Instantiate(skills[_attackType], transform);
            var _skillBase = _act.GetComponent<SkillBase>();
            _skillBase.shouldActionStart = true;
            _skillBase.SetStart();
            _skillBase.SetAbnormalState();
            _attackType = -1;
        }
    }

    public void Init()
    {
        skills = new Dictionary<int, GameObject>();
        Dictionary<int, SkillData> skillData = _player.GetComponent<PlayerSkillTree>().Skills;
        List<int> ids = new List<int>();
        foreach (int key in skillData.Keys)
        {
            ids.Add(key);
        }
        foreach (int k in ids)
        {
            skills[k] = Resources.Load<GameObject>("SkillPrefab/" + k) as GameObject;
            //Debug.Log(skills[k].name);
        }

        //acts[1] = Resources.Load < GameObject > ("SkillPrefab/101") as GameObject;
        
    }
}
