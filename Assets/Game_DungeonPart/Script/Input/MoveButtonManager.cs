using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveButtonManager : MonoBehaviour {

    public bool _isActive;
    public float pushIntervalMax = 0.4f;
    public float pushInterval = 0;
    public Vector3 moveDir = Vector3.zero;
    PlayerMove playerMove;

	// Use this for initialization
	void Start () {
        _isActive = true; 
        playerMove = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
	}
	
	// Update is called once per frame
	void Update () {
        pushInterval += Time.deltaTime;

        if (moveDir != Vector3.zero)
        {
            playerMove.MoveStart(moveDir);
            if (false == _isActive && pushInterval >= pushIntervalMax)
            {
                playerMove.resMoveDir = moveDir;
                pushInterval = 0;
            }
        }
    }

    public void MoveButtonPointerDown(int dir)
    {
        moveDir = Vector3.zero;
        if (dir <= 1 || dir >= 7) moveDir.z = 1;
        if (dir >= 3 && dir <= 5) moveDir.z = -1;
        if (dir >= 1 && dir <= 3) moveDir.x = 1;
        if (dir >= 5 && dir <= 7) moveDir.x = -1;

        //if (false == _isActive && pushInterval >= pushIntervalMax)
        //{
        //    playerMove.resMoveDir = moveDir;
        //    pushInterval = 0;
        //}
        if (false == _isActive) return;

        playerMove.MoveStart(moveDir);
        pushInterval = 0;
    }

    public void MoveButtonPointerUp()
    {
        moveDir = Vector3.zero;
    }
}
