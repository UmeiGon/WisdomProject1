using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class EventSceneManager : MonoBehaviour {
    
    enum ImageFile
    {
        Angel1,
        Angel2,
        Angel3,
        Reaper1,
        Reaper2
    }

    Dictionary<ImageFile, Sprite> data = new Dictionary<ImageFile, Sprite>();
    
    TextAsset dialog;
    StringReader reader;
    [SerializeField]
    float textDisplaySpeed = 0.3f;

    GameObject parent;
    UISwitch uiSwitch;
    [SerializeField]
    Image[] Pictures;
    [SerializeField]
    Text eventText;

    // Use this for initialization
    void Start () {
        parent = GameObject.Find("GameObjectParent");
        uiSwitch = parent.GetComponentInChildren<UISwitch>();

        data[ImageFile.Angel1] = Resources.Load<Sprite>("Image/EventUI/angel_1") as Sprite;
        data[ImageFile.Angel2] = Resources.Load<Sprite>("Image/EventUI/angel_2") as Sprite;
        data[ImageFile.Angel3] = Resources.Load<Sprite>("Image/EventUI/angel_3") as Sprite;
        data[ImageFile.Reaper1] = Resources.Load<Sprite>("Image/EventUI/reaper_1") as Sprite;
        data[ImageFile.Reaper2] = Resources.Load<Sprite>("Image/EventUI/reaper_2") as Sprite;

        PictureChange(0, ImageFile.Angel1);
        PictureChange(1, ImageFile.Reaper2);

        SetEventDialog("EventText2");
    }
	
	// Update is called once per frame
	void Update () {

    }

    void SetEventDialog(string _fileName)
    {
        dialog = Resources.Load<TextAsset>("Dialog/" + _fileName) as TextAsset;
        reader = new StringReader(dialog.text);
        // 最初の行を表示
        NextMessage();
    }

    void PictureChange(int pos, ImageFile img)
    {
        Pictures[pos].sprite = data[img];
    }


    public void OnTapMessageBox()
    {
        // 次の行がある場合
        if ( 0 == NextMessage() )
        {

        }
        // ダイアログ終了
        else uiSwitch.SwitchUI((int)DungUIType.BATTLE);
    }

    int NextMessage()
    {
        // ダイアログ終点に達した時
        if ( reader.Peek() <= -1 ) return -1;

        // 次の行を取得

        // 区切り指定文字
        char[] delimiterChars = { ',' };

        string str = reader.ReadLine();
        string[] words = str.Split(delimiterChars);

        if ( words.Length < 3 ) return -1;

        int picNum;
        int.TryParse(words[0], out picNum);
        PictureChange(0, (ImageFile)picNum);
        int.TryParse(words[1], out picNum);
        PictureChange(1, (ImageFile)picNum);
        eventText.text = words[2];

        return 0;
    }
}
