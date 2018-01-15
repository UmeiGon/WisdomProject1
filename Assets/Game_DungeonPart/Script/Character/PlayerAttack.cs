using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAttack : MonoBehaviour {

    GameObject parent;
    Player player;
    int attackType = -1;
    PlayerActionAnimation actAnim;
    TurnManager turnMn;
    [SerializeField] GameObject rangePrafab;
    [SerializeField] List<GameObject> rangeRects = new List<GameObject>();
    GameObject selectingAct;

	// Use this for initialization
	void Start () {
        parent = GameObject.Find("GameObjectParent");
        player = parent.GetComponentInChildren<Player>();
        //player = Game.Instance.Player;
        actAnim = GetComponent<PlayerActionAnimation>();
        turnMn = parent.GetComponentInChildren<TurnManager>();
	}
	
	// Update is called once per frame
	void Update () {
        //if (attackType != -1)
        //{
        //    actAnim.SetAttackNumber(attackType);
        //    attackType = -1;
        //}
	}
    
    public void SetSkillRangeActive(int num)
    {
        
        for (int i = rangeRects.Count - 1; i >= 0; --i)
        {
            Destroy(rangeRects[i]);
            rangeRects.RemoveAt(i);
        }

        if (selectingAct) Destroy(selectingAct);
        selectingAct = Instantiate(actAnim.skills[num]);
        Debug.Log(num);
        SkillBase skill = selectingAct.GetComponent<SkillBase>();
        skill.SetStart();
        List<Vector3> ranges = new List<Vector3>();
        ranges = skill.GetRange();
        foreach(Vector3 pos in ranges)
        {
            var rect = Instantiate(rangePrafab);
            rect.transform.position = pos;
            rangeRects.Add(rect);
        }
    }

    public void DestroySkillRange()
    {
        for (int i = rangeRects.Count - 1; i >= 0; --i)
        {
            Destroy(rangeRects[i]);
            rangeRects.RemoveAt(i);
        }
    }
    public void MagicAttack(int num)
    {
        if (turnMn.PlayerActionSelected) return;

        attackType = num;
        if ( attackType != -1 )
        {
            actAnim.SetAttackNumber(attackType);
            attackType = -1;
        }
        player.action = ActionType.ATTACK;
        ActionData addAction = new ActionData(player, player.action, attackType, 0, Vector3.zero);
        player.thisTurnAction.Add(addAction);
        player.PlayerActSelect();
    }
}
