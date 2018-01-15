using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UISwitch : MonoBehaviour {

    [SerializeField]
    GameObject[] uis = new GameObject[(int)DungUIType.DungUITypeMax];
    [SerializeField]
    GameObject subCamera;

    [SerializeField]
    GameObject uiMenuButtons;

	// Use this for initialization
	void Start () {
        for ( int t = 0; t < (int)DungUIType.DungUITypeMax; t++ )
        {
            if ( uis[t] )
            {
                // 可視化
                uis[t].GetComponent<Canvas>().enabled = true;
            }
        }
        uiMenuButtons.GetComponent<Canvas>().enabled = true;
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void SwitchUI(int type)
    {
        // type に合致するものだけアクティブ化
        for (int t = 0; t < (int)DungUIType.DungUITypeMax; t++)
        {
            if ( uis[t] )
            {
                uis[t].SetActive(( t == type ) ? true : false);
            }
        }

        // サブカメラON/OFF
        subCamera.SetActive((type == (int)DungUIType.INVENTRY) ? true : false);

        // 「修練＆精製」「ステータス」などのウィンドウ選択ボタン
        uiMenuButtons.SetActive(( (int)DungUIType.INVENTRY <= type && type <= (int)DungUIType.SKILLTREE ) ? 
            true : false);
    }
}
