using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonInitializer : MonoBehaviour {

    GameObject parent;
    DungeonPartManager dMn;
    [SerializeField] GameObject player;
    MapManager mapMn;
    EnemyManager enemyMn;
    OnGroundObjectManager groundObjMn;
    ObstacleManager obsMn;
    int height;
    int width;
    int[,] chara_exist2D;
    int[,] onground_exist2D;
    [SerializeField] int enemyCount = 5;
    int itemCount = 3;

    int eneCount = 1;

    // 難易度調整用
    // playerCloseEnemyMax + 1匹目以降はプレイヤーから半径 closeRangeMin 以下の場所にはスポーンさせない
    int playerCloseEnemyMax = 2;
    float closeRangeMin = 7;

    public void Init()
    {
        parent = GameObject.Find("GameObjectParent");
        dMn = parent.GetComponentInChildren<DungeonPartManager>();
        //Dungeon大きさ情報の取得
        mapMn = DungeonPartManager.Instance.mapManager;
        height = mapMn.GetDungeonHeight();
        width = mapMn.GetDungeonWidth();
        chara_exist2D = new int[height, width];
        onground_exist2D = new int[height, width];
        for (int z = 0; z < height; z++)
        {
            for (int x = 0; x < width; x++)
            {
                chara_exist2D[z, x] = -1;
                onground_exist2D[z, x] = -1;
            }
        }
        enemyMn = DungeonPartManager.Instance.enemyManager;
        enemyMn.d_init = this;

        //キャラなどの配置
        PlayerStartPosDecide();

        if ( 1 == SaveData.GetInt("IsInterrupt", 0) )
        {

            enemyMn.LoadEnemys();
        }
        else
        {
            //if(dMn.floor == 8) // ボス部屋
            //{
            //    enemyCount = 1;
            //}
            EnemyStartPosDecide();
            // MapManagerにその情報を渡す
            mapMn.SetCharaAndObjectInfo(chara_exist2D, onground_exist2D);

            // デバッグ用
            //for (int z = 0; z < height; z++)
            //{
            //    string info = "";
            //    for (int x = 0; x < width; x++)
            //    {
            //        info += chara_exist2D[z, x];
            //    }
            //    Debug.Log(info);
            //}
        }

        groundObjMn = parent.GetComponentInChildren<OnGroundObjectManager>();
        groundObjMn.d_init = this;
        groundObjMn.Init();

        obsMn = parent.GetComponentInChildren<ObstacleManager>();
        obsMn.d_init = this;
        obsMn.Init();

        // 最後にはメモリから消えてもらう
        //Destroy(gameObject);
    }


    void PlayerStartPosDecide()
    {
        player.GetComponent<PlayerMove>().charaID = 1;

        Vector3 pos;
        Vector3 charaDir = new Vector3(0, 0, 1);
        //プレイヤー位置の決定
        if ( 1 == SaveData.GetInt("IsInterrupt", 0) )
        {
            Player.PosData _data = SaveData.GetClass<Player.PosData>("PlayerPosData", new Player.PosData());
            pos = new Vector3(_data.PosX, 0, _data.PosZ);
            charaDir = new Vector3(_data.DirX, 0, _data.DirZ);
        }
        else
        {
            pos = GetRandomPos();
            charaDir = Calc.RandomDir();
            // キャラの位置を配列に入れて予約（他とかぶらないようにする）
            chara_exist2D[(int)pos.z, (int)pos.x] = 1;
        }
        player.transform.position = pos;
        Player pl = player.GetComponent<Player>();
        pl.pos = pos;
        pl.charaDir = charaDir;
        pl.init = false;
        
    }

    void EnemyStartPosDecide()
    {
        while (eneCount <= enemyCount)
        {
            float sqrPlayerCloseRange = 0;
            Vector3 pos = Vector3.zero;

            // プレイヤーに近い敵が一定数以上にならないよう難易度調整
            do
            {
                pos = GetRandomPos();
                sqrPlayerCloseRange = ( pos - player.transform.position ).sqrMagnitude;
            } while ( eneCount <= playerCloseEnemyMax && sqrPlayerCloseRange < closeRangeMin * closeRangeMin );

            enemyMn.EnemyAdd(pos);

            // キャラの位置を配列に入れて予約（他とかぶらないようにする）
            chara_exist2D[(int)pos.z, (int)pos.x] = eneCount + 500;

            eneCount++;
        }
        DebugMessage.UpdateText();
    }

    public Vector3 StairsPosDecide()
    {
        Vector3 pos;
        // 中断した場合はロード
        if ( 1 == SaveData.GetInt("IsInterrupt", 0))
            pos = new Vector3(SaveData.GetInt("StairX", 0), 0, SaveData.GetInt("StairZ", 0));
        else pos = GetRandomPos();

        if (dMn.floor == 8) pos = new Vector3(23, 0, 16);
        
        // 中断用にセーブしておく
        SaveData.SetInt("StairX", (int)pos.x);
        SaveData.SetInt("StairZ", (int)pos.z);
        return pos;
    }

    //public Vector3

    public Vector3 GetRandomPos()
    {
        int px = Random.Range(0, width);
        int pz = Random.Range(0, height);
        int attempt = 0;

        while (mapMn.GetDungeonInfo(px, pz) == -1 //壁である 
            || mapMn.GetDungeonInfo(px, pz) >= mapMn.max_room   // 通路である
            || chara_exist2D[pz,px] != -1) //既にキャラが存在する
        {
            px = Random.Range(0, width);
            pz = Random.Range(0, height);
            attempt++;
            if ( attempt >= 30 )
            {
                Debug.Log("敵の初期位置のランダム設定に失敗、場外に配置しました。");
                return new Vector3(-1, 0, -1);
            }
        }

        return new Vector3(px, 0, pz);
    }

    public int[,] GetCharaExist2D()
    {
        return chara_exist2D;
    }

    public int[,] GetOnGroundExist2D()
    {
        return onground_exist2D;
    }
}
