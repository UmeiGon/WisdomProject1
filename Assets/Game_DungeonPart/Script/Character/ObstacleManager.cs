using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour {

    [SerializeField]
    GameObject[] obstaclePrefabs;

    List<GameObject> obstacleList = new List<GameObject>();
    [SerializeField] int obstacleCount = 5;

    GameObject parent;
    MapManager mapMn;
    public DungeonInitializer d_init;

    [SerializeField]
    GameObject charactersParent;

    public void Init()
    {
        parent = GameObject.Find("GameObjectParent");
        mapMn = parent.GetComponentInChildren<MapManager>();

        int obsCount = 0;
        while ( obsCount < obstacleCount )
        {
            int obsType = Random.Range(0, 2);
            Vector3 pos = d_init.GetRandomPos();
            var obj = Instantiate(obstaclePrefabs[obsType], charactersParent.transform);
            obj.transform.position = pos;

            // obstacle には 200～の IDをつける
            int _ID = obsType + 200;
            mapMn.chara_exist2D[(int)pos.z, (int)pos.x] = _ID;
            //obj.GetComponent<OnGroundObject>().ID = _ID;
            obstacleList.Add(obj);
            obsCount++;
        }
    }
}
