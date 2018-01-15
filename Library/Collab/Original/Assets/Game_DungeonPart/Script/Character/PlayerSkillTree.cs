using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class PlayerSkillTree : MonoBehaviour
{
    //添字がスキルのIdになっている。中身はSkillData
    public Dictionary<int, SkillData> Skills = new Dictionary<int, SkillData>();
    public int[] SetSkills = new int[6] { 1,-1,-1,-1,-1,-1};
    // Use this for initialization
    void Awake()
    {
        Skills[0] = new SkillData("杖殴り", null, new int[] { 0, 0, 0 });
        Skills[1] = new SkillData("マジックショット", null, new int[] { 0, 0, 0 });
        //101~109
        Skills[101] = new SkillData("フレイムショット", null,new int[] { 5,0,0});
        Skills[102] = new SkillData("バーニングショット", new List<int> { 101 });
        Skills[103] = new SkillData("イグニスショット", new List<int> { 102 });
        Skills[104] = new SkillData("ブラスター", new List<int> { 101 });
        Skills[105] = new SkillData("エクスプロージョン", new List<int> { 109 });
        Skills[106] = new SkillData("ファイアエラプション", new List<int> { 104});
        Skills[107] = new SkillData("バーニングエラプション", new List<int> { 106,109});
        Skills[108] = new SkillData("イグニスエラプション", new List<int> { 107});
        Skills[109] = new SkillData("ブレイブエンゲージ", new List<int> {102,104 });
        //201~208
        Skills[201] = new SkillData("パラライズショット", null,new int[] {0,5,0});
        Skills[202] = new SkillData("サンダーボール", new List<int> { 201});
        Skills[203] = new SkillData("ライトニングボール(Ⅲ)", new List<int> { 202,205 });
        Skills[204] = new SkillData("雷遠広", new List<int> {201 });
        Skills[205] = new SkillData("倍速化", new List<int> { 202,204});
        Skills[206] = new SkillData("パラライズブラスト", new List<int> {201});
        Skills[207] = new SkillData("カラミティサンダー(Ⅱ)", new List<int> {204,206});
        Skills[208] = new SkillData("カラミティサンダー(Ⅲ)", new List<int> { 205,207});
        //301~308
        Skills[301] = new SkillData("氷遠小", new List<int> { 304},new int[] {0,0,5 });
        Skills[302] = new SkillData("氷遠中", new List<int> { 301,305});
        Skills[303] = new SkillData("氷遠大", new List<int> { 302 });
        Skills[304] = new SkillData("氷ブロ1弱", null, new int[] { 0, 0, 5 });
        Skills[305] = new SkillData("氷ブロ1中", new List<int> { 301});
        Skills[306] = new SkillData("氷ブロ9弱", new List<int> { 302,307});
        Skills[307] = new SkillData("部屋雨", new List<int> { 305 });
        Skills[308] = new SkillData("完全防御", new List<int> { 303,306 });
        //画像入れる。
        foreach (var i in Skills)
        {
            i.Value.setImage("Image/StatusUI/"+i.Key);
            Debug.Log(i.Key);
        }

    }

    // Update is called once per frame
    void Update()
    {

    }
}
