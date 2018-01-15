using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemManager : MonoBehaviour
{
    //プレイヤーの中に入れる予定
    public Dictionary<int, ItemData> items = new Dictionary<int, ItemData>();
    public List<ItemData> canCreateItems = new List<ItemData>();
    [SerializeField]
    GameObject MainPanel;
    private List<GameObject> createItemImages = new List<GameObject>();
    void Awake()
    {
        items[0] = new ItemData("赤のソウルストーン", "赤く光る石。");
        items[1] = new ItemData("黄のソウルストーン", "黄色いく光る石。");
        items[2] = new ItemData("青のソウルストーン", "青く光る石。");
        items[100] = new ItemData("攻撃力アップのオーブ", "");
        items[101] = new ItemData("防御力アップのオーブ", "");
        items[102] = new ItemData("倍速効果のオーブ", "");
        items[103] = new ItemData("即時回復オーブ", "");
        items[104] = new ItemData("透明化のオーブ", "");
        items[105] = new ItemData("気配察知のオーブ", "モンスターの位置が分かる。");
        items[106] = new ItemData("単体攻撃のオーブ", "");
        items[107] = new ItemData("範囲攻撃のオーブ", "投げて当たった5×5マスに範囲攻撃。");
        items[108] = new ItemData("暗視のオーブ", "ダンジョンの構造、モンスターの位置が分かる。");
        items[0].kosuu = 100;
        items[1].kosuu = 100;
        items[2].kosuu = 100;
    }
    private void Start()
    {
        for (int i = 1; i <= 12; i++)
        {
            createItemImages.Add(MainPanel.transform.Find("CreateImage ("+i+")").gameObject);
        }
        setCreateItems();
    }
    public void onClickItemCreate()
    {
        setCreateItems();
    }
    void setCreateItems()
    {
        canCreateItems.Clear();
        foreach (var i in items)
        {
            if (canCreate(i.Key))
            {
                canCreateItems.Add(i.Value);
            }
        }
        foreach (var txt in createItemImages)
        {
            txt.transform.GetComponentInChildren<Text>().text = "";
        }
        for (int i = 0; i < canCreateItems.Count; i++)
        {
            createItemImages[i].GetComponentInChildren<Text>().text = canCreateItems[i].name;

        }
    }
    bool canCreate(int itemId)
    {
        switch (itemId)
        {
            case 100:
                return createHantei(5, 0, 0);
            case 101:
                return createHantei(0, 0, 5);
            case 102:
                return createHantei(0, 5, 0);
            case 103:
                return createHantei(5, 5, 5);
            case 104:
                return createHantei(0, 5, 5);
            case 105:
                return createHantei(0, 5, 5);
            case 106:
                return createHantei(10, 0, 0);
            case 107:
                return createHantei(10, 5, 5);
            case 108:
                return createHantei(0, 10, 10);
        }
        return false;
    }
    bool createHantei(int red, int yellow, int blue)
    {
        if (red <= items[0].kosuu && yellow <= items[1].kosuu && blue <= items[2].kosuu)
        {
            return true;
        }
        return false;
    }
}
