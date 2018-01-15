using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TurnManager : MonoBehaviour {


    [SerializeField]
    public bool PlayerActionSelected { get; private set; }
    float startMoveTime = 0;
    [SerializeField]
    public float MoveTime { get { return Time.time - startMoveTime; } }
    [SerializeField]
    public float MoveTimeMax = 0.5f;
    [SerializeField]
    public float moveSpeed = 2.0f;

    [SerializeField]
    private bool turnTableStarted = false;
    [SerializeField]
    ActionType nowTurnActType;
    [SerializeField]
    private bool allActFinish = false;

    private int turnActNum = 0;
    List<ActionData> turnTable;


    Player player;
    PlayerMove playerMove;
    DungeonPartManager dMn;
    MapManager mapMn;
    EnemyManager eneMn;
    UIMiniMap miniMap;

    [SerializeField]
    bool outDebugLog = false;

	// Use this for initialization
	void Start () {
        GameObject parent = GameObject.Find("GameObjectParent");
        player = parent.GetComponentInChildren<Player>();
        playerMove = parent.GetComponentInChildren<PlayerMove>();
        eneMn = parent.GetComponentInChildren<EnemyManager>();
        miniMap = parent.GetComponentInChildren<UIMiniMap>();
        dMn = parent.GetComponentInChildren<DungeonPartManager>();
        mapMn = parent.GetComponentInChildren<MapManager>();
        turnTable = new List<ActionData>();
    }

    public void PlayerActSelect()
    {
        // すでに行動選択が終わっている場合は無意味
        if ( PlayerActionSelected ) return;
        PlayerActionSelected = true;

        // 本来はプレイヤー側で行う
        AddAction(player, player.action, 2, 0, Vector3.zero);

        eneMn.EnemyActionSelect();

        // デバッグ用に行動内容を参照する
        foreach ( ActionData turn in turnTable )
        {
            //Debug.Log(turn.actType.ToString());
        }

        // ターンコルーチンの開始
        //StartCoroutine("TurnCorountine");
        turnActNum = 0;
        turnTableStarted = true;
    }

    // Update is called once per frame
    void Update () {

        //if ( nowTurnActType == ActionType.MOVE )
        //{
        //    //if ()
        //    if ( MoveTime >= MoveTimeMax )
        //    {
        //        MoveTime = 0;
        //        nowTurnActType = ActionType.NON_ACTION;
        //    }
        //    MoveTime += Time.deltaTime;
        //}

        if ( !turnTableStarted ) return;

        // 0番から所定の番号までの行動Updateを呼ぶ
        for (int n = 0; n <= turnActNum; ++n )
        {
            //turnTable[n].ActionUpdate();
            turnTable[n].allowed = true;
        }

        // 行動をさせる番号を進める処理
        for ( ; turnActNum < turnTable.Count - 1; )
        {
            // 移動以外のアクションは１つずつ順番に行うが
            // 移動アクションは一斉に行う

            // 今アクション「移動」
            // 次アクション「移動」
            // なら次も行動可
            if ( turnTable[turnActNum].actType == ActionType.MOVE
                && turnTable[turnActNum + 1].actType == ActionType.MOVE )
            {
                ++turnActNum;
                continue;
            }
            // 今アクション「移動」
            // 次アクション「移動じゃない」
            // なら今アクションが完了したら次が行動可
            else if ( turnTable[turnActNum].actType == ActionType.MOVE 
                && turnTable[turnActNum + 1].actType != ActionType.MOVE )
            {
                if ( turnTable[turnActNum].finish )
                {
                    ++turnActNum;
                    continue;
                }
            }
            // 今アクション「移動じゃない」
            // なら今アクションが完了したら次が行動可
            else if( turnTable[turnActNum].actType != ActionType.MOVE )
            {
                if ( turnTable[turnActNum].finish )
                {
                    ++turnActNum;
                    continue;
                }
            }
            break;
        }

        // turnTableデバッグ表示
        if ( turnTable.Count > 0 )
        {
            DebugMessage.Print("turnActNum : " + turnActNum);
            int count = 0;
            foreach ( ActionData act in turnTable )
            {
                DebugMessage.Print("Turn " + count + " : " + act.actChara + act.sequence + act.actType + act.pos + act.finish);
                ++count;
            }
            DebugMessage.UpdateText();
        }

        int lastActNum = turnTable.Count - 1;
        if (turnTable[lastActNum].finish )
        {
            // 全ての行動をfinishしたかチェック
            allActFinish = true;
            for (int n = 0; n <= lastActNum - 1; ++n )
            {
                if ( !turnTable[n].finish )
                {
                    allActFinish = false;
                    break;
                }
            }
        }
        if (allActFinish)
        {
            allActFinish = false;

            PlayerActionSelected = false;
            turnTableStarted = false;
            eneMn.EnemyTurnReset();
            turnTable.Clear();


            // 足元にアイテム、階段などあるかチェック
            playerMove.FootCheck();
            // 死亡した敵がいるか確認
            DeathCheck();
            // ターン経過での敵スポーン
            eneMn.SpawnCounterPlus();

            miniMap.MiniMapUpdate();

            // セーブ
            player.SavePlayerInfo();
            // ↓重いかもしれないので必要なくなったら外す
            player.SaveSkill();
            mapMn.Save(1);
            eneMn.SaveEnemys();
            SaveData.Save();
        }

        player.CharaUpdate();
        playerMove.MoveUpdate();
        foreach ( Enemy ene in eneMn.enemys )
        {
            ene.CharaUpdate();
        }

        //if ( MoveTime >= _moveTimeMax )
        //{
        //    MoveTime = 0;
        //}

    }
    IEnumerator TurnCorountine()
    {
        for ( ;;)
        {
            for ( ; turnActNum < turnTable.Count; )
            {

                if ( turnTable[turnTable.Count - 1].finish )
                {
                    break;
                }

                // 順番がきたキャラは行動開始
                if (!turnTable[turnActNum].finish && !turnTable[turnActNum].actChara.ActStart(turnActNum, turnTable[turnActNum]) )
                {
                    //if ( turnActNum >= turnTable.Count - 1 ) break;
                    yield return null;
                    continue;
                }
                else
                {
                    if ( outDebugLog )
                        Debug.Log("Start turn " + turnActNum);
                }
                ActionType tempAct = nowTurnActType;
                nowTurnActType = turnTable[turnActNum].actType;
                if ( tempAct != ActionType.MOVE && nowTurnActType == ActionType.MOVE )
                {
                    startMoveTime = Time.time;
                }

                if ( turnTable[turnActNum].finish )
                {
                    turnActNum++;
                    continue;
                }

                // 次の手番のキャラの行動が 何もしないor移動 だったら
                if ( turnActNum + 1 < turnTable.Count && turnTable[turnActNum + 1].actType <= ActionType.MOVE )
                {
                    if ( turnTable[turnActNum].actType <= ActionType.MOVE )
                    {
                        turnActNum++;
                        continue;
                    }
                }
                yield return null;
            }
            if ( outDebugLog )
                Debug.Log("ターンが終わろうとしている" + ( turnTable.Count - 1 ));
            if ( turnTable[turnTable.Count - 1].finish )
            {
                bool _isEnd = true;
                foreach ( ActionData act in turnTable )
                {
                    if ( !act.finish )
                    {
                        _isEnd = false;
                        break;
                    }
                }
                if ( _isEnd )
                {
                    if (outDebugLog)
                        Debug.Log("ターン終了");
                    nowTurnActType = ActionType.NON_ACTION;
                    allActFinish = true;
                    // 終了
                    break;
                }
            }
            yield return null;
        }
    }

    public void DeathCheck()
    {
        bool ok = false;
        // 死んだ敵がリストから全く居なくなると ok = true になる
        while ( !ok )
        {
            foreach ( Enemy ene in eneMn.enemys )
            {
                ok = false;
                if ( !ene.IsAlive )
                {
                    eneMn.EnemyRemove(ene.idNum);
                    Destroy(ene.gameObject);
                    //SetLastEnemy();
                    break;
                }
                ok = true;
            }
            if ( eneMn.enemys.Count == 0 )
            {
                ok = true;
            }
        }
    }

    public void AddAction(BattleParticipant _chara, ActionType _type,int _skillNum, int _seq, Vector3 _pos)
    {
        var addAction = new ActionData(_chara, _type, _skillNum, _seq, _pos);
        addAction.turnMn = this;
        turnTable.Add(addAction);
    }
    public void AddAction(ActionData addAction)
    {
        addAction.turnMn = this;
        turnTable.Add(addAction);
    }
    public void OneActEnd(int _turnActNum)
    {
        //nowTurnActType = ActionType.NON_ACTION;
        if ( outDebugLog )
            Debug.Log("Turn " + _turnActNum + ":End");
        if ( turnTable.Count > 0 ) turnTable[_turnActNum].finish = true;
    }
}
