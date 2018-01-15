using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MagicSelectButtonManager : MonoBehaviour {

    bool _isActive = false;
    GameObject parent;
    GameObject magicSelectWindow;
    MagicButtonManager magicBtnMn;
    float timeCount = 0;

	// Use this for initialization
	void Start () {
        parent = GameObject.Find("GameObjectParent");
        magicBtnMn = parent.GetComponentInChildren<MagicButtonManager>();
        magicSelectWindow = magicBtnMn.gameObject;
        magicSelectWindow.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (_isActive) timeCount = 0;
        else timeCount += Time.deltaTime;

        if (timeCount > 0.2f && timeCount < 2.0f)
        {
            magicSelectWindow.SetActive(false);
        }
	}

    public void DragStart()
    {
        _isActive = true;
        magicSelectWindow.SetActive(true);
        magicBtnMn.UpdateMagic();
    }
    public void DragEnd()
    {
        _isActive = false;
        //magicSelectWindow.SetActive(false);
    }
}
