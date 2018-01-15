using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonPartManager : MonoBehaviour {

    [SerializeField] GameObject parent;
    Player player;
    public static DungeonPartManager Instance;
    public MapManager mapManager;
    UIMiniMap miniMap;
    public DungeonInitializer dungeonInitializer;
    public EnemyManager enemyManager;
    public MoveButtonManager moveButtonManager;
    public int dungeonType = 0;
    public int floor = 1;
    UISwitch ui;

    private void Awake()
    {
        Instance = this;
        Application.targetFrameRate = 30;
    }
    // Use this for initialization
    void Start () {
        if (!parent) parent = GameObject.Find("GameObjectParent");
        player = parent.GetComponentInChildren<Player>();

        floor = SaveData.GetInt("Floor", 1);
        if ( floor % 8 >= 5 && floor % 8 <= 7 && floor != 30)
        {
            dungeonType = 1;
        }
        mapManager = GameObject.Find("MapManager").GetComponent<MapManager>();
        miniMap = parent.GetComponentInChildren<UIMiniMap>();
        dungeonInitializer = GameObject.Find("DungeonInitializer").GetComponent<DungeonInitializer>();
        enemyManager = GameObject.Find("EnemyManager").GetComponent<EnemyManager>();
        moveButtonManager = GameObject.Find("MoveButtonManager").GetComponent<MoveButtonManager>();

        mapManager.d_initializer = dungeonInitializer;
        mapManager.DungeonGenerate();
        //dungeonInitializer.Init();
        int _bgmNum = dungeonType;
        if (floor % 8 == 0 || floor == 30) _bgmNum = 2;
        parent.GetComponentInChildren<BGMSet>().SetBGM(_bgmNum);
        player.LoadPlayerInfo();
        player.Init();
        miniMap.MiniMapInit();

        ui = parent.GetComponentInChildren<UISwitch>();
        ui.SwitchUI((int)DungUIType.BATTLE);
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void NextFloor()
    {
        //if (floor == 4)
        //{
        //    dungeonType = 2;
        //}
        if (floor == 30)
        {
            UnityEngine.SceneManagement.SceneManager.LoadScene("GameClear");
            return;
        }
        floor++;
        SaveData.SetInt("Floor", floor);
        // 中断フラグOFF
        SaveData.SetInt("IsInterrupt", 0);
        SaveData.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene("Dungeon1");
    }

    public void SaveDataReset()
    {
        SaveData.Clear();
        SaveData.Save();
    }
}
