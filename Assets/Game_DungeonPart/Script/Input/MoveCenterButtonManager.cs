using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveCenterButtonManager : MonoBehaviour {

    [SerializeField]
    private Vector3 pos;
    private Vector3 sPos;
    private PlayerMove playerMove;
    public GameObject charaDirection;
    private GameObject canvas;
    GameObject _parent;
    GameObject _moveButtonsParent;
    Vector3[] fixedDirection = new Vector3[8];

	// Use this for initialization
	void Start () {
        _parent = GameObject.Find("GameObjectParent");
        _moveButtonsParent = _parent.GetComponentInChildren<MoveButtonsParent>().gameObject;
        canvas = GameObject.Find("Canvas");
        playerMove = _parent.GetComponentInChildren<PlayerMove>();
        charaDirection = _parent
            .GetComponentInChildren<CharaDirection_Player>().gameObject;
        charaDirection.SetActive(false);
        fixedDirection[0] = new Vector3(0, 1, 0);
        fixedDirection[1] = new Vector3(1, 1, 0);
        fixedDirection[2] = new Vector3(1, 0, 0);
        fixedDirection[3] = new Vector3(1, -1, 0);
        fixedDirection[4] = new Vector3(0, -1, 0);
        fixedDirection[5] = new Vector3(-1, -1, 0);
        fixedDirection[6] = new Vector3(-1, 0, 0);
        fixedDirection[7] = new Vector3(-1, 1, 0);
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    public void DragStart()
    {
        pos = Input.mousePosition;
        charaDirection.SetActive(true);
    }
    public void DragStay()
    {
        
        sPos = Input.mousePosition;
        for (int i = 0; i < 8; i++)
        {
            float angle = 0;
            angle = Vector3.Angle(sPos - pos, fixedDirection[i]);
            if (angle <= 22.5f)
            {
                Vector3 selectDir;
                //Vector3 selectDir = new Vector3(fixedDirection[i].x, 0, z);
                int _moveButtonsRotationZ = (int)_moveButtonsParent.transform.eulerAngles.z;
                selectDir = Calc.RotateZ(fixedDirection[i], -_moveButtonsRotationZ);
                selectDir = new Vector3(selectDir.x, 0, selectDir.y);
                playerMove.SetCharaDir(selectDir);
                break;
            }
        }
    }
    public void DragEnd()
    {
        charaDirection.SetActive(false);

    }
}
