using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventTriggerMagicButton : EventTrigger {

    GameObject parent;
    PlayerAttack _playerAttack;
    [SerializeField]
    public int _skillNum;

    private void Start()
    {
        parent = GameObject.Find("GameObjectParent");
        _playerAttack = parent.GetComponentInChildren<PlayerAttack>();
        //_skillNum = GetComponent<SkillNum>().SkillNumber;
    }

    public override void OnPointerEnter(PointerEventData eventData)
    {
        _playerAttack.SetSkillRangeActive(_skillNum);
    }

    public override void OnPointerExit(PointerEventData eventData)
    {
        base.OnPointerExit(eventData);
        _playerAttack.DestroySkillRange();
    }

    public override void OnDrop(PointerEventData eventData)
    {
        base.OnDrop(eventData);
        _playerAttack.MagicAttack(_skillNum);
        _playerAttack.DestroySkillRange();
    }

    
}
