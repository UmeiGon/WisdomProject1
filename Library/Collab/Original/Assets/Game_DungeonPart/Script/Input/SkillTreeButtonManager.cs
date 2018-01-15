using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillTreeButtonManager : MonoBehaviour
{
    GameObject parent;
    private const int TREE_WID = 3, TREE_HEI = 7, NONE = -1;
    public const int RED = 0, YELLOW = 1, BLUE = 2, TREE_SUU = 3, SET_SUU = 6;
    private int SelectSkill = NONE;
    private int SelectSkillSet = NONE;
    //UIのgameobject達
    [SerializeField]
    private GameObject SkillButtonPre;
    [SerializeField]
    private GameObject BranchPre;
    [SerializeField]
    private GameObject ScrollPanel;
    [SerializeField]
    private GameObject SyutokuButton;
    [SerializeField]
    private GameObject TourokuButton;
    [SerializeField]
    private GameObject KaijoButton;
    [SerializeField]
    private GameObject SkillSetPanel;
    [SerializeField]
    private GameObject SetSelectImage;
    [SerializeField]
    private GameObject SkillSelectImage;
    [SerializeField]
    private GameObject SkillTreeCanvas;
  
    public GameObject SetumeiPanel;
    [SerializeField]
    private GameObject SyadanPanel;


    private Dictionary<int, GameObject> skillButtons = new Dictionary<int, GameObject>();
    public GameObject[] SoulStoneImage = new GameObject[TREE_SUU];
    private GameObject[] SkillSetButton = new GameObject[SET_SUU];

    private PlayerSkillTree pst;

    // Use this for initialization
    void Start()
    {
        parent = GameObject.Find("GameObjectParent");
        //プレイヤーからスキルツリーデータを持ってくる
        pst = GameObject.Find("Player_Unitychan").GetComponent<PlayerSkillTree>();
        //ツリー背景インスタンス化
        GameObject[] TreeBacks = new GameObject[TREE_SUU];
        for (int i = 0; i < TREE_SUU; i++)
        {
            TreeBacks[i] = ScrollPanel.transform.Find(""+i).gameObject;
        }
        //ツリーの配置データを作成
        int[][,] TreeHaiti = new int[TREE_SUU][,]
        {
            //redのツリー配置
        new int[TREE_HEI, TREE_WID]
        {
            {  0,  0,108},
            {  0,105,  0},
            {103,  0,107},
            {  0,109,  0},
            {102,  0,106},
            {  0,104,  0},
            {101,  0,  0}
        },
         //yellowのツリー配置
         new int[TREE_HEI, TREE_WID]
        {
            {  0,  0,208},
            {203,  0,  0},
            {  0,205,207},
            {202,  0,  0},
            {  0,204,  0},
            {  0,  0,206},
            {201,  0,  0}
        },
          //blueのツリー配置
          new int[TREE_HEI, TREE_WID]
        {
            {  0,308,  0},
            {303,  0,306},
            {  0,  0,  0},
            {302,  0,307},
            {  0,305,  0},
            {301,  0,  0},
            {304,  0,  0}
        }
        };
        //スクロールパネルにツリーを配置
        for (int i = 0; i < TREE_SUU; i++)
        {
            SkillTreeUICreate(TreeBacks[i], TreeHaiti[i]);
            TreeBacks[i].transform.localPosition = new Vector3(i * (TreeBacks[i].GetComponent<RectTransform>().sizeDelta.x + 60) + 30, 0);
        }
        TreeBacks[RED].GetComponent<Image>().color = Color.red;
        TreeBacks[YELLOW].GetComponent<Image>().color = Color.yellow;
        TreeBacks[BLUE].GetComponent<Image>().color = Color.blue;
        Kousin();
        for (int i = 0; i < SET_SUU; i++)
        {
            SkillSetButton[i] = SkillSetPanel.transform.Find("" + i).gameObject;
        }
        //セレクトimageを前に出す
        SkillSelectImage.transform.SetAsLastSibling();
        SetumeiPanel.transform.SetAsLastSibling();
        SetSelectImage.transform.SetAsLastSibling();
        transform.SetAsLastSibling();
        
    }

    void SkillTreeUICreate(GameObject treeBack, int[,] haitidata)
    {
        float width = treeBack.GetComponent<RectTransform>().sizeDelta.x;
        float height = treeBack.GetComponent<RectTransform>().sizeDelta.y;
        float offset = 60;
        float masukanW = (width - offset * 2) / (TREE_WID - 1);
        float masukanH = (height - offset * 2) / (TREE_HEI - 1);
        Dictionary<int, GameObject> Buttons = new Dictionary<int, GameObject>();
        for (int h = 0; h < TREE_HEI; h++)
        {
            for (int w = 0; w < TREE_WID; w++)
            {
                int num = haitidata[h, w];
                if (num > 0)
                {
                    skillButtons[num] = Instantiate(SkillButtonPre, treeBack.transform);
                    Buttons[num] = skillButtons[num];
                    Buttons[num].transform.localPosition = new Vector3(offset + (w) * masukanW, -offset + (h) * -masukanH);
                    Buttons[num].GetComponent<SkillButton>().init(num);
                    Buttons[num].GetComponentInChildren<Text>().text += num;
                    Buttons[num].transform.Find("skillImage").GetComponent<Image>().sprite = pst.Skills[num].skillImage;
                    Buttons[num].GetComponent<UnityEngine.UI.Button>().onClick.AddListener(() => SkillSelectClick(num));
                }
            }
        }
        //前提スキルボタンに対して枝を伸ばす。
        foreach (GameObject i in Buttons.Values)
        {
            if (null == pst.Skills[i.GetComponent<SkillButton>().skillnum].ZenteiSkillId) break;
            foreach (int k in pst.Skills[i.GetComponent<SkillButton>().skillnum].ZenteiSkillId)
            {
                var dis = Vector3.Distance(Buttons[k].transform.localPosition, i.transform.localPosition);
                var diff = (Buttons[k].transform.position - i.transform.position).normalized;
                var br = Instantiate(BranchPre, i.transform.position, Quaternion.FromToRotation(Vector3.up, diff), treeBack.transform);
                br.GetComponent<RectTransform>().sizeDelta = new Vector2(20, dis);
                br.transform.SetAsFirstSibling();
                Buttons[k].GetComponent<SkillButton>().Branch.Add(br);
            }
        }

    }
    // Update is called once per frame
    void Update()
    {
        for (int i = 0; i < TREE_SUU; i++)
        {
            SoulStoneImage[i].transform.Find("Text").GetComponent<Text>().text = "x" + pst.GetComponent<ItemManager>().items[i].kosuu;
        }
    }

    //全てのスキルボタンの状態を更新,新たにスキルを習得した時に呼び出す。
    void Kousin()
    {
        //ボタンの状態を更新
        foreach (var i in skillButtons.Values)
        {
            i.GetComponent<SkillButton>().jotaiKousin();
        }

        //周りのボタンを更新
        if (SelectSkill != NONE)
        {

            SyutokuButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
            foreach (var i in SoulStoneImage)
            {
                i.transform.Find("usesoultext").GetComponent<Text>().text = "";
            }

            switch (skillButtons[SelectSkill].GetComponent<SkillButton>().Jotai)
            {
                case SkillButton.jotai.GOT:

                    break;
                case SkillButton.jotai.UN_GOT:
                    SyutokuButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
                    SoulStoneImage[RED].transform.Find("usesoultext").GetComponent<Text>().text = "-" + pst.Skills[SelectSkill].UseSoul[SkillTreeButtonManager.RED];
                    SoulStoneImage[YELLOW].transform.Find("usesoultext").GetComponent<Text>().text = "-" + pst.Skills[SelectSkill].UseSoul[SkillTreeButtonManager.YELLOW];
                    SoulStoneImage[BLUE].transform.Find("usesoultext").GetComponent<Text>().text = "-" + pst.Skills[SelectSkill].UseSoul[SkillTreeButtonManager.BLUE];
                    break;
            }
        }

    }
    public void TourokuKaijoButKousin()
    {
        TourokuButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
        KaijoButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
        if (SelectSkill != NONE && SelectSkillSet != NONE)
        {
            if (skillButtons[SelectSkill].GetComponent<SkillButton>().Jotai == SkillButton.jotai.GOT)
            {
                TourokuButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
            }
            if (pst.SetSkills[SelectSkillSet] != NONE)
            {
                KaijoButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
            }
        }
    }
    public void SkillSetClick(int num)
    {
        SelectSkillSet = num;
        SetSelectImage.SetActive(true);
        SetSelectImage.transform.position = SkillSetButton[num].transform.position;
        TourokuKaijoButKousin();
    }
    public void SkillSelectClick(int num)
    {
        SelectSkill = num;
        SkillSelectImage.SetActive(true);
        float scX = transform.parent.GetComponent<RectTransform>().sizeDelta.x;
        SetumeiPanel.transform.position= skillButtons[num].transform.position+((skillButtons[num].transform.position.x>scX/2)?new Vector3(-(100+skillButtons[num].GetComponent<RectTransform>().sizeDelta.x+SkillSelectImage.GetComponent<RectTransform>().sizeDelta.x),100):new Vector3(100,100));
        SetumeiPanel.transform.Find("SkillNameText").GetComponent<Text>().text = pst.Skills[num].SkillName;
        SetumeiPanel.SetActive(true);
        SyadanPanel.SetActive(true);
        SkillSelectImage.transform.position = skillButtons[num].transform.position;
        Kousin();
        TourokuKaijoButKousin();
    }
    public void SyutokuClick()
    {
        bool canSyutoku = true;
        //習得できるか判定
        for (int i = 0; i < TREE_SUU; i++)
        {
            if (pst.GetComponent<ItemManager>().items[i].kosuu < pst.Skills[SelectSkill].UseSoul[i])
            {
                canSyutoku = false;
            }
        }
        if (canSyutoku)
        {
            for (int i = 0; i < TREE_SUU; i++)
            {
                pst.GetComponent<ItemManager>().items[i].kosuu -= pst.Skills[SelectSkill].UseSoul[i];
            }
            pst.Skills[SelectSkill].Syutoku = true;
            Kousin();
            TourokuKaijoButKousin();
        }

    }
    public void TourokuClick()
    {
        if (SelectSkillSet != NONE)
        {
            for (int i=0;i<SET_SUU;i++)
            {
                if (pst.SetSkills[i]==SelectSkill)
                {
                    SkillSetButton[i].transform.Find("Text").GetComponent<Text>().text = "";
                    pst.SetSkills[i] = NONE;
                }
            }
            pst.SetSkills[SelectSkillSet] = SelectSkill;
            //SkillSetButton[SelectSkillSet].transform.Find("skillImage").GetComponent<Image>().enabled = true;
            //SkillSetButton[SelectSkillSet].transform.Find("skillImage").GetComponent<Image>().sprite =  pst.Skills[SelectSkill].skillImage;
            SkillSetButton[SelectSkillSet].transform.Find("Text").GetComponent<Text>().text = "" + SelectSkill;
            // 登録スキルの画像セット
            PlayerSkillTree skillTree = parent.GetComponentInChildren<PlayerSkillTree>();
            Image iconImg = SkillSetButton[SelectSkillSet].GetComponentInChildren<SkillImage>().GetComponent<Image>();
            iconImg.enabled = true;
            iconImg.color = new Color(1, 1, 1, 1);
            iconImg.sprite = skillTree.Skills[SelectSkill].skillImage;

            KaijoButton.GetComponent<UnityEngine.UI.Button>().interactable = true;
            TourokuKaijoButKousin();
        }
    }
    public void KaijoClick()
    {
        pst.SetSkills[SelectSkillSet] = -1;
        SkillSetButton[SelectSkillSet].transform.Find("skillImage").GetComponent<Image>().enabled=false;
        SkillSetButton[SelectSkillSet].transform.Find("Text").GetComponent<Text>().text = "";
        KaijoButton.GetComponent<UnityEngine.UI.Button>().interactable = false;
    }
    //後で削除
    public void XXXX()
    {
        SkillTreeCanvas.SetActive(!SkillTreeCanvas.activeSelf);
    }
}
