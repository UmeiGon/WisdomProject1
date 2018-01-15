using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMove : MonoBehaviour {

    GameObject parent;
    Player player;
    public Vector3 resMoveDir;
    MapManager mapMn;
    public TurnManager turnMn;
    MoveButtonManager moveBtMn;

    GameObject _checkNextFloor; 

    public int charaID = 0;
    public Vector3 moveVec;
    AnimationChanger anim;
    [SerializeField]
    private float moveSpeed = 2.5f;
    bool init = false;

	// Use this for initialization
	void Start () {
        parent = GameObject.Find("GameObjectParent");
        player = parent.GetComponentInChildren<Player>();
        mapMn = parent.GetComponentInChildren<MapManager>();
        turnMn = parent.GetComponentInChildren<TurnManager>();
        moveBtMn = parent.GetComponentInChildren<MoveButtonManager>();
        anim = GetComponent<AnimationChanger>();
        
        _checkNextFloor = GameObject.Find("MessageWindow_NextFloor");
        _checkNextFloor.SetActive(false);
	}
	
	// Update is called once per frame
	public void MoveUpdate () {
        if (!init)
        {
            //MoveStart(Vector3.zero);
            player.pos = transform.position;
            player.sPos = player.pos;
            player.charaDir = Vector3.forward;
            init = true;
        }
        

        if(moveVec != Vector3.zero )
        {
            Move();
        }

        if (moveVec == Vector3.zero && resMoveDir != Vector3.zero) // 先行入力があったら
        {
            if (!turnMn.PlayerActionSelected 
                && !MoveStart(resMoveDir))
                resMoveDir = Vector3.zero;
            if (moveVec != Vector3.zero)
            {
                //Move();
            }
        }
        if (player.action >= ActionType.ATTACK)
        {
            resMoveDir = Vector3.zero;
        }
    }

    float actionRate = 0;

    public void Move()
    {
        //Vector3 _moveDis = moveVec * turnMn._moveRate;
        //transform.position = player.pos + _moveDis;
        //if (turnMn._moveRate >= 1)
        //{
        //    transform.position = player.sPos;
        //    player.pos = player.sPos;
        //    moveVec = Vector3.zero;
        //    DungeonPartManager.Instance.moveButtonManager._isActive = true;
        //    anim.TriggerAnimator("Move", false);
        //    player.ActEnd();
        //}
        actionRate += Time.deltaTime * turnMn.moveSpeed;
        if(actionRate >= 1 )
        {
            // 移動終了
            transform.position = player.pos + moveVec;
            player.pos += moveVec;
            moveVec = Vector3.zero;
            anim.TriggerAnimator("Move", false);
            DungeonPartManager.Instance.moveButtonManager._isActive = true;
            player.ActEnd();

            actionRate = 0;
        }
        else
        {
            Vector3 distance = moveVec * actionRate;
            transform.position = player.pos + distance;
        }
    }

    public bool MoveStart(Vector3 dir)   // true ならば 先行入力が有効
    {
        if ( turnMn.PlayerActionSelected ) return true;

        player.charaDir = dir; // 選択方向を向く
        player.SetObjectDir();

        // マップ外に出ようとしている
        if ( !mapMn.InsideMap(player.pos + dir) ) return false;

        if (!mapMn.CanMoveCheck(player.pos, player.pos + dir)) return false;
        mapMn.SetCharaExistInfo(player.pos);
        moveBtMn._isActive = false;
        player.sPos = player.pos + dir;
        mapMn.SetCharaExistInfo(player.sPos, charaID, true);
        moveVec = dir;
        anim.TriggerAnimator("Move", true);
        player.action = ActionType.MOVE;
        player.PlayerActSelect();
        ActionData addAction = new ActionData(player, player.action, 0, 0, player.sPos);
        player.thisTurnAction.Add(addAction);
        //StartCoroutine(MoveCoroutine());
        return false;
    }

    public IEnumerator MoveCoroutine()
    {
        float timeMax = turnMn.MoveTimeMax;
        anim.TriggerAnimator("Move");

        for ( float moveTime = turnMn.MoveTime; moveTime < timeMax; moveTime = turnMn.MoveTime )
        {
            float moveRate = moveTime / timeMax;
            Vector3 _moveDis = moveVec * moveRate;
            transform.position = player.pos + _moveDis;
            yield return null;
        }
        // 移動終了
        transform.position = player.sPos;
        player.pos = player.sPos;
        moveVec = Vector3.zero;
        anim.TriggerAnimator("Move", false);


        DungeonPartManager.Instance.moveButtonManager._isActive = true;
        player.ActEnd();

        //float timeMax = turnMn.MoveTimeMax;

        //timeMax /= moveSpeed;
        //sPos = nowAct.pos;
        //anim.TriggerAnimator("Move");
        //for ( float moveTime = turnMn.MoveTime; moveTime < timeMax; moveTime = turnMn.MoveTime )
        //{
        //    float moveRate = moveTime / timeMax;
        //    Vector3 _moveDis = moveVec * moveRate;
        //    transform.position = pos + _moveDis;
        //    yield return null;
        //}
        //// 移動終了
        //transform.position = sPos;
        //pos = sPos;
        //moveVec = Vector3.zero;
        //anim.TriggerAnimator("Move", false);
        //ActEnd();
    }

    public void SetCharaDir(Vector3 dir)
    {
        if (moveVec != Vector3.zero) return; // 移動中は向きを変えない
        player.charaDir = dir;
        player.SetObjectDir();
    }

    public void FootCheck()
    {
        if (mapMn.onground_exist2D[(int)player.pos.z, (int)player.pos.x] == 100)
        {
            _checkNextFloor.SetActive(true);
        }
        else _checkNextFloor.SetActive(false);
    }
}
