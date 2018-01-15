using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillBase : MonoBehaviour{
    
    public List<Vector2> range;
    public int useMp = 0;
    [SerializeField]
    protected float rowPower = 0;
    protected float calcPower = 0;
    public bool shouldActionStart = false;

    public enum TYPE
    {
        NO_ELEMENT,
        FLAME,
        LIGHTNING,
        ICE
    }
    [SerializeField]
    TYPE element;

    protected GameObject parent;
    protected GameObject playerObj;
    protected Player player;
    protected PlayerMove playerMove;
    protected MapManager mapMn;
    protected AnimationChanger anim;
    protected CameraManager _camera;
    protected RawImage icon;
    [SerializeField] protected List<GameObject> effects;
    protected Vector3 hitPos; // 当たる予定の場所
    protected Vector3 targetExistPos;  // 当たる対象の居る場所
    protected float timeCount = 0;
    protected TurnManager turnMn;

    protected EnemyManager eneMn;
    

    public void SetStart()
    {
        parent = GameObject.Find("GameObjectParent");
        playerObj = GameObject.FindWithTag("Player");
        player = parent.GetComponentInChildren<Player>();
        //player = Game.Instance.Player;
        playerMove = parent.GetComponentInChildren<PlayerMove>();
        mapMn = parent.GetComponentInChildren<MapManager>();
        anim = playerObj.GetComponent<AnimationChanger>();
        _camera = parent.GetComponentInChildren<CameraManager>();
        turnMn = parent.GetComponentInChildren<TurnManager>();
        eneMn = parent.GetComponentInChildren<EnemyManager>();

        calcPower = player.GetComponent<AtkAndDef>().CalcPower(element, rowPower);
        SetTarget(Vector3.zero);
    }

    protected void Start()
    {
        // 上の SetStart で初期化してないなら実行
        //if ( parent ) return;
        //parent = GameObject.Find("GameObjectParent");
        //playerObj = GameObject.FindWithTag("Player");
        //player = parent.GetComponentInChildren<Player>();
        ////player = Game.Instance.Player;
        //playerMove = parent.GetComponentInChildren<PlayerMove>();
        //mapMn = parent.GetComponentInChildren<MapManager>();
        //anim = playerObj.GetComponent<AnimationChanger>();
        //_camera = parent.GetComponentInChildren<CameraManager>();
        //turnMn = parent.GetComponentInChildren<TurnManager>();
        //eneMn = parent.GetComponentInChildren<EnemyManager>();
        //SetTarget(Vector3.zero);
    }

    private void Update()
    {

    }

    public virtual void SetTarget(Vector3 pos)
    {

    }

    public virtual void SetAbnormalState()
    {

    }
    
    public virtual List<Vector3> GetRange()
    {
        Debug.Log("return null");
        return null;
    }
}
