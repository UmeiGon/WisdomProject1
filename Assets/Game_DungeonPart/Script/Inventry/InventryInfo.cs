using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class InventryInfo : MonoBehaviour {

    GameObject parent;
    PlayerItem playerItem;

    const int MAX_INVENTRY = 25;
    GameObject[] cells = new GameObject[MAX_INVENTRY];
    //float timeCount = 0;

    // メモリに確保せず UpdateInventry のたびにGetComponen がいいかもしれない
    //Image[] icons = new Image[MAX_INVENTRY];
    //Text

	// Use this for initialization
	void Start () {
        parent = GameObject.Find("GameObjectParent");
        playerItem = parent.GetComponentInChildren<PlayerItem>();

        OneCell[] _cells = GetComponentsInChildren<OneCell>();
        for(int i = 0; i < MAX_INVENTRY; ++i ) 
        {
            cells[i] = _cells[i].gameObject;
        }
	}
	
	// Update is called once per frame
	void Update () {
        //// 暫定的に
        //timeCount += Time.deltaTime;
        //if(timeCount > 1 )
        //{
        //    timeCount = 0;
        //    UpdateInventry();
        //}
	}

    public void UpdateInventry()
    {
        var items = playerItem.items;
        int count = 0;

        foreach ( KeyValuePair<int, ItemData> item in items )
        {
            // ソウルストーン系はインベントリに表示しない
            if ( item.Key < 100 ) continue;

            if ( item.Value.kosuu > 0 )
            {
                Image icon = cells[count].GetComponentInChildren<ImageIcon>().GetComponent<Image>();
                icon.color = Color.white;
                icon.sprite = item.Value.itemImage;

                Text text = cells[count].GetComponentInChildren<Text>();
                text.text = "x" + item.Value.kosuu.ToString();
                ++count;
            }
        }
        for (int i = count; i < MAX_INVENTRY; ++i )
        {
            Image icon = cells[i].GetComponentInChildren<ImageIcon>().GetComponent<Image>();
            // 透明
            icon.color = Color.clear;
        }
    }

    private void OnEnable()
    {
        if ( !parent ) Start();

        UpdateInventry();
    }
}
