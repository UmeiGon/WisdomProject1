using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

[SerializeField]
public class EnemyManager : MonoBehaviour
{

    GameObject parent;
    [SerializeField]
    GameObject characterParent;
    public List<Enemy> enemys = new List<Enemy>();
    [SerializeField]
    GameObject[] enemyPrefab;
    [SerializeField]
    int[] testModeEnemyTypes;
    [SerializeField]
    Dictionary<int, EnemySpawnData> spawnTable = null;

    int enemyCount = 0;
    int enemyID = 500;
    int dungeonType = -1;
    int _floor = 1;

    [SerializeField]
    bool testMode = false;

    int spawnCounter = 0;
    [SerializeField]
    int spawnFrequency = 40;
    public DungeonInitializer d_init;

    Player player = null;
    [SerializeField] float closeFromPlayerRange = 7;

    // Use this for initialization
    void Start()
    {
        parent = GameObject.Find("GameObjectParent");
        player = parent.GetComponentInChildren<Player>();
    }
    

    public void SpawnCounterPlus()
    {
        if ( !player )
        {
            Start();
        }

        spawnCounter++;
        if ( spawnCounter >= spawnFrequency )
        {
            spawnCounter = 0;
            Vector3 pos;
            Vector3 dis;
            do
            {
                pos = d_init.GetRandomPos();
                dis = player.pos - pos;
            } while (dis.sqrMagnitude < closeFromPlayerRange * closeFromPlayerRange);
            
            EnemyAdd(pos);
        }
    }

    public void SetSpawnTable()
    {
        spawnTable = new Dictionary<int, EnemySpawnData>();
        // 0 = 基本
        // 1 = 弓
        // 2 = UFO
        // 3 = 豪華
        // 4 = 剣
        // 5 = 幽霊
        // 6 = 採掘士
        // 7 = 光源

        //spawnTable[1] = new EnemySpawnData(0, 100);
        //spawnTable[2] = new EnemySpawnData(0, 70, 1, 30);
        //spawnTable[3] = new EnemySpawnData(0, 50, 1, 50);
        //spawnTable[4] = new EnemySpawnData(0, 50, 1, 50);

        //spawnTable[5] = new EnemySpawnData(1, 100);

        //spawnTable[6] = new EnemySpawnData(0, 40, 1, 50, 2, 10);
        //spawnTable[7] = new EnemySpawnData(0, 20, 1, 60, 2, 20);
        //spawnTable[8] = new EnemySpawnData(0, 20, 1, 40, 2, 40);
        //spawnTable[9] = new EnemySpawnData(0, 10, 1, 10, 2, 80);

        //spawnTable[7] = new EnemySpawnData(1, 50, 2, 50);
        //spawnTable[8] = new EnemySpawnData(1, 50, 2, 50);

        //spawnTable[101] = new EnemySpawnData(1, 50, 2, 50);

        TextAsset tableData = Resources.Load<TextAsset>("EnemySpawnTable/Enemy_Dungeon1") as TextAsset;
        StringReader reader = new StringReader(tableData.text);

        while ( reader.Peek() > -1 )
        {
            // 1 行を読み取る
            string line = reader.ReadLine();
            char[] delimiterChars = { ',' };
            string[] words = line.Split(delimiterChars);
            int dataLength = 0;
            for (int k = 0; k < words.Length; ++k )
            {
                if ( words[k] == "" ) break;
                ++dataLength;
            }
            // 最初の「階層」分は削る
            --dataLength;
            int floor = 0;
            int.TryParse(words[0], out floor);

            // "基本","70", "弓", "30" とかを読み取る
            int[] datas = new int[dataLength];
            for ( int i = 0; i < dataLength; i+= 2 )
            {
                switch ( words[i + 1] )
                {
                    case "基本":
                        datas[i] = 0;
                        break;
                    case "弓":
                        datas[i] = 1;
                        break;
                    case "ユーフォー":
                        datas[i] = 2;
                        break;
                    case "豪華":
                        datas[i] = 3;
                        break;
                    case "剣":
                        datas[i] = 4;
                        break;
                    case "幽霊":
                        datas[i] = 5;
                        break;
                    case "採掘士":
                        datas[i] = 6;
                        break;
                    case "光源":
                        datas[i] = 7;
                        break;
                    default:
                        Debug.Log("敵出現テーブル:ロードエラー");
                        break;
                }
                int.TryParse(words[i + 2], out datas[i + 1]);
            }
            spawnTable[floor] = new EnemySpawnData(datas);
        }
    }

    public void EnemyAdd(Vector3 posi)
    {
        if ( posi.x < 0 ) return;
        if ( spawnTable == null )
        {
            SetSpawnTable();
        }
        if ( dungeonType == -1 )
        {
            parent = GameObject.Find("GameObjectParent");
            DungeonPartManager dMn = parent.GetComponentInChildren<DungeonPartManager>();
            dungeonType = dMn.dungeonType;
            _floor = dMn.floor;
        }
        enemyCount++; // enemyの総数を意図
        enemyID++; // enemyにIDナンバーを振る（総数に関係なく+1していく）

        GameObject newEnemyObj;
        // 敵の種類選択
        if ( testMode )
        {
            int type = testModeEnemyTypes[ Random.Range(0, testModeEnemyTypes.Length) ];
            newEnemyObj = Instantiate(enemyPrefab[type], characterParent.transform);
        }
        else
        {
            int type = -1;
            EnemySpawnData spawnData = spawnTable[_floor];
            int r = Random.Range(0, 100);
            int rateTotal = 0;
            // r < rateTotal により rate に見合う Enemy を選択
            for ( var i = 0; i < spawnData.data.Count; i++ )
            {
                rateTotal += spawnData.data[i].rate;
                if ( r < rateTotal )
                {
                    type = spawnData.data[i].ID;
                    break;
                }
            }

            if ( type >= 0 )
            {
                newEnemyObj = Instantiate(enemyPrefab[type], characterParent.transform);
            }
            else
            {
                Debug.Log("敵種選択エラー");
                return;
            }
        }
        Enemy newEne = newEnemyObj.GetComponent<Enemy>();
        DebugMessage.Print(newEne.ToString());
        newEne.SetStartParam(enemyID, posi);
        enemys.Add(newEne);
        newEne.Init();

        SetStrength(newEne);
    }

    Dictionary<int, int[]> strengthTable = null;

    void SetStrength(Enemy enemy)
    {
        if (strengthTable == null )
        {
            strengthTable = new Dictionary<int, int[]>();
            TextAsset strengthData = Resources.Load<TextAsset>("EnemySpawnTable/EnemyLevel") as TextAsset;
            StringReader reader = new StringReader(strengthData.text);
            // 最初の行は日本語の情報のみなので飛ばす
            reader.ReadLine();

            int _level = 1;
            while(reader.Peek() > -1 )
            {
                string line = reader.ReadLine();
                char[] delimiterChars = { ',' };
                string[] words = line.Split(delimiterChars);
                int[] datas = new int[words.Length];
                for (int i = 0; i < words.Length; ++i )
                {
                    int.TryParse(words[i], out datas[i]);
                }
                strengthTable[_level] = datas;
                ++_level;
            }
        }

        int level = 0;
        // レベル決定
        for (int i = 1; i <= 6; ++i )
        {
            if ( _floor < strengthTable[i][0] ) break;
            ++level;
        }

        // 強さを設定
        enemy.MaxHP = strengthTable[level][1];
        enemy.HP = enemy.MaxHP;
        AtkAndDef atkAndDef = enemy.GetComponent<AtkAndDef>();
        atkAndDef.NormalPower = strengthTable[level][2];
        enemy.RewardExp = strengthTable[level][3];
    }

    public void DamageParameter()
    {

    }

    public void EnemyActionSelect()
    {
        // 2倍回数行動まで想定されるので3回 SelectAction を呼ぶ
        foreach ( Enemy ene in enemys )
        {
            ene.PlusActionGauge();
            ene.SelectAction();
        }
        foreach ( Enemy ene in enemys )
        {
            ene.SelectAction();
        }
        foreach ( Enemy ene in enemys )
        {
            ene.SelectAction();
        }
    }

    public void EnemyTurnReset()
    {
        foreach ( Enemy ene in enemys )
        {
            ene.actAllowed = false;
            //ene.acted = false;
        }
    }
    public void EnemyRemove(int num)
    {
        foreach ( Enemy ene in enemys )
        {
            if ( ene.idNum == num )
            {
                //Debug.Log("Remove");
                enemys.Remove(ene);
                return;
            }
        }
    }
    public Enemy GetEnemy(int num)
    {
        foreach ( Enemy ene in enemys )
        {
            if ( ene.idNum == num )
            {
                return ene;
            }
        }
        return null;
    }

    // ↓独自クラスをjson文字列しセーブできるようにする
    [System.Serializable]
    public class Data
    {
        public EnemyType type;
        public int HP;
        public int MaxHP;
        public float NormalPower;
        public Vector3 pos;
        public Vector3 charaDir;

        public Data()
        {
            type = EnemyType.NORMAL;
            HP = 15;
            MaxHP = 15;
            NormalPower = 2;
            pos = new Vector3(0, 0, 0);
            charaDir = new Vector3(0, 0, -1);
        }
        public Data(Enemy enemy)
        {
            type = enemy.type;
            HP = enemy.HP;
            MaxHP = enemy.MaxHP;
            NormalPower = enemy.atkAndDef.NormalPower;
            pos = enemy.pos;
            charaDir = enemy.charaDir;
        }
        /// <summary>
        /// HPなどをセーブデータから設定
        /// </summary>
        /// <param name="enemy"></param>
        public void Load(Enemy enemy)
        {
            enemy.type = type;
            enemy.HP = HP;
            enemy.MaxHP = MaxHP;
            enemy.atkAndDef.NormalPower = NormalPower;
            enemy.pos = pos;
            enemy.charaDir = charaDir;
        }
    }
    public void SaveEnemys()
    {
        List<Data> enemysData = new List<Data>();
        foreach ( Enemy ene in enemys )
        {
            enemysData.Add(new Data(ene));
        }
        SaveData.SetList<Data>("EnemysData", enemysData);
    }
    public void LoadEnemys()
    {
        List<Data> enemysData = new List<Data>();
        enemysData = SaveData.GetList<Data>("EnemysData", enemysData);

        foreach ( Data data in enemysData )
        {
            enemyCount++; // enemyの総数を意図
            enemyID++; // enemyにIDナンバーを振る（総数に関係なく+1していく）

            GameObject newEnemyObj;
            // 敵の種類選択
            EnemyType type = data.type;

            if ( type >= 0 )
            {
                newEnemyObj = Instantiate(enemyPrefab[(int)type], characterParent.transform);
            }
            else
            {
                Debug.Log("敵種選択エラー");
                return;
            }

            Enemy newEne = newEnemyObj.GetComponent<Enemy>();
            newEne.SetStartParam(enemyID, data.pos);
            newEne.Init();
            // HPなど代入
            data.Load(newEne);

            enemys.Add(newEne);
        }
    }
}
