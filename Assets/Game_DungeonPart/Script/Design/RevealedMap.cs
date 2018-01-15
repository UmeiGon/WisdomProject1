using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RevealedMap : MonoBehaviour {

    bool init = false;
    public bool[,] reveal2D { get; private set; }
    [SerializeField]
    int revealRange = 2; // 道に居る時、周囲 2 マスのマッピングをする

    GameObject parent;
    MapManager mapMn;
    Player player;

    [SerializeField]
    bool debugMode = false;

    // Use this for initialization
    void Start () {
        //parent = GameObject.Find("GameObjectParent");
        //mapMn = parent.GetComponentInChildren<MapManager>();
        //reveal2D = new bool[MapManager.DUNGEON_HEIGHT, MapManager.DUNGEON_WIDTH];
        //player = parent.GetComponentInChildren<Player>();
	}

    public void Init()
    {
        parent = GameObject.Find("GameObjectParent");
        mapMn = parent.GetComponentInChildren<MapManager>();
        reveal2D = new bool[MapManager.DUNGEON_HEIGHT, MapManager.DUNGEON_WIDTH];
        player = parent.GetComponentInChildren<Player>();
    }

    public void UpdateRevealMap()
    {
        if ( debugMode )
        {
            for ( int z = 0; z < MapManager.DUNGEON_HEIGHT; ++z )
            {
                for ( int x = 0; x < MapManager.DUNGEON_WIDTH; ++x )
                {
                    reveal2D[z, x] = true;
                }
            }
        }

        int _existRoomNum = player.existRoomNum;
        if (_existRoomNum < mapMn.max_room )
        {
            // プレイヤーが部屋に居る
            for (int z = 0; z < MapManager.DUNGEON_HEIGHT; ++z )
            {
                for (int x = 0; x < MapManager.DUNGEON_WIDTH; ++x )
                {
                    if ( _existRoomNum == mapMn.dung_room_info2D[z, x] )
                    {
                        reveal2D[z, x] = true;
                        continue;
                    }
                    if ((int)player.sPos.z - revealRange <= z && z <= (int)player.sPos.z + revealRange
                        && (int)player.sPos.x - revealRange <= x && x <= (int)player.sPos.x + revealRange )
                        reveal2D[z, x] = true;
                }
            }
        }
        else
        {
            // プレイヤーが部屋以外（道など）に居る
            for (int z = (int)player.sPos.z - revealRange; z <= (int)player.sPos.z + revealRange; ++z )
            {
                for ( int x = (int)player.sPos.x - revealRange; x <= (int)player.sPos.x + revealRange; ++x )
                {
                    if ( z < 0 || z >= MapManager.DUNGEON_HEIGHT
                        || x < 0 || x >= MapManager.DUNGEON_WIDTH ) continue;

                    reveal2D[z, x] = true;
                }
            }
        }
    }

    public bool IsSightRange(Vector3 pos)
    {
        if ( debugMode ) return true;

        int z = (int)pos.z;
        int x = (int)pos.x;
        if ( z < 0 || z >= MapManager.DUNGEON_HEIGHT
            || x < 0 || x >= MapManager.DUNGEON_WIDTH ) return false;

        int _existRoomNum = player.existRoomNum;
        if ( _existRoomNum < mapMn.max_room )
        {
            // プレイヤーが部屋に居る
            if ( mapMn.dung_room_info2D[z, x] == _existRoomNum )
            {
                return true;
            }
        }
        else
        {
            // プレイヤーが部屋以外（道など）に居る
            if ( z >= player.sPos.z - revealRange && z <= player.sPos.z + revealRange 
                && x >= player.sPos.x - revealRange && x <= player.sPos.x + revealRange )
            {
                return true;
            }
        }
        return false;
    }

    public void SwitchDebugMode()
    {
        debugMode = !debugMode;
    }
}
