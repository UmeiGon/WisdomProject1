using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DebugMessage : MonoBehaviour {

    static DebugMessage instance;
    List<string> messages = new List<string>();
    Text text;

    Image backImg;


    [SerializeField]
    bool isActive = true;

    // Use this for initialization
    void Start () {
        text = GetComponentInChildren<Text>();
        backImg = GetComponent<Image>();
        instance = this;
        isActive = ( SaveData.GetInt("DebugMessageActive", 0) != 0 ) ? true : false ;
        isActive = !isActive;
        SwitchActive();
	}
	
	// Update is called once per frame
	void Update () {

	}

    void updateText()
    {
        string mes = "";
        foreach ( string str in messages )
        {
            mes += str;
        }
        text.text = mes;
        messages.Clear();
    }

    public static void Print(string _message)
    {
        string mes = _message + "\n";
        instance.messages.Add(mes);
    }

    public static void UpdateText()
    {
        instance.updateText();
    }

    public void SwitchActive()
    {
        isActive = !isActive;
        SaveData.SetInt("DebugMessageActive", ( isActive ) ? 1 : 0);
        SaveData.Save();

        text.color = ( isActive ) ? new Color(1, 1, 1, 1) : new Color(1, 1, 1, 0);
        backImg.color = ( isActive ) ? new Color(0, 0, 0, 0.2f) : new Color(1, 1, 1, 0);
    }
}
