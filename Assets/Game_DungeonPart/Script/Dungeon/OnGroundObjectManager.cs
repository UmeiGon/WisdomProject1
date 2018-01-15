using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OnGroundObjectManager : MonoBehaviour {

    [SerializeField]
    GameObject[] objectPrefabs;

    List<GameObject> objectList = new List<GameObject>();
    [SerializeField] int groundObjCount = 5;

    GameObject parent;
    MapManager mapMn;
    public DungeonInitializer d_init;

    [SerializeField]
    GameObject mapChipsParent;

    public void Init()
    {
        parent = GameObject.Find("GameObjectParent");
        mapMn = parent.GetComponentInChildren<MapManager>();

        int gObjCount = 0;
        while ( gObjCount < groundObjCount )
        {
            int gObjType = Random.Range(0, 2);
            Vector3 pos = d_init.GetRandomPos();
            var obj = Instantiate(objectPrefabs[gObjType], mapChipsParent.transform);
            obj.transform.position = pos;

            // mapobject には 300～の IDをつける
            int _ID = gObjType + 300;
            mapMn.onground_exist2D[(int)pos.z, (int)pos.x] = _ID;
            obj.GetComponent<OnGroundObject>().ID = _ID;
            objectList.Add(obj);
            gObjCount++;
        }
    }
}
