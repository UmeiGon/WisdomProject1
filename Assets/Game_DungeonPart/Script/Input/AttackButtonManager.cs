using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AttackButtonManager : MonoBehaviour {

    PlayerMove playerMove;

	// Use this for initialization
	void Start () {
        playerMove = GameObject.FindWithTag("Player").GetComponent<PlayerMove>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void AttackButtonDown()
    {
        //playerMove
    }
}
