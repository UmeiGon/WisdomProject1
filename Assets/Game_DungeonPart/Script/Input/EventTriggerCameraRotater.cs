using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class EventTriggerCameraRotater : EventTrigger{

    GameObject _parent;
    GameObject _cameraParent;
    CameraManager _cameraMn;
    GameObject _moveButtonsParent;
    GameObject _minimapCenter;
    Vector2 _dragStartFingerPos;
    float _selectRotateY = 0;
    float[] _cameraPosRotateY = { 0,90,180,270,360 };
    bool _isDrag;

    // Use this for initialization
    void Start () {
        _parent = GameObject.Find("GameObjectParent");
        _cameraParent = _parent.GetComponentInChildren<MainCameraParent>().gameObject;
        _cameraMn = _parent.GetComponentInChildren<CameraManager>();
        _moveButtonsParent = _parent.GetComponentInChildren<MoveButtonsParent>().gameObject;
        _minimapCenter = _parent.GetComponentInChildren<MinimapCenter>().gameObject;
        _minimapCenter.SetActive(false);
	}
	
	// Update is called once per frame
	void Update () {
        if (!_isDrag)
        {
            if (_selectRotateY == -360)
            {
                // 固定角度を決定
                float _minDeltaR = 3600;
                foreach (int rotY in _cameraPosRotateY)
                {
                    float _deltaR = _cameraParent.transform.eulerAngles.y - rotY;
                    _deltaR = Mathf.Abs(_deltaR);
                    if (_deltaR < _minDeltaR)
                    {
                        //Debug.Log(_deltaR);
                        _selectRotateY = rotY;
                        _minDeltaR = _deltaR;
                    }
                    float _moveButtonRotate = _selectRotateY - _moveButtonsParent.transform.eulerAngles.z;
                    _moveButtonsParent.transform.Rotate(0, 0, _moveButtonRotate);
                    _minimapCenter.transform.Rotate(0, 0, _moveButtonRotate);
                }
            }
            else
            {
                float _leftRotateY = _selectRotateY - _cameraParent.transform.eulerAngles.y;
                _leftRotateY *= 10.0f * Time.deltaTime;
                _cameraParent.transform.Rotate(0, _leftRotateY, 0);
                if (_cameraParent.transform.eulerAngles.y < 10 && _selectRotateY == 360)
                {
                    _selectRotateY = 0;
                }
            }
        }
	}

    public override void OnPointerDown(PointerEventData eventData)
    {
        base.OnPointerDown(eventData);
        _dragStartFingerPos = eventData.pressPosition;
    }

    public override void OnEndDrag(PointerEventData eventData)
    {
        base.OnEndDrag(eventData);
        _isDrag = false;
    }

    public override void OnDrag(PointerEventData eventData)
    {
        base.OnDrag(eventData);
        Vector2 delta = eventData.delta;

        _cameraParent.transform.Rotate(0, delta.x * 0.15f, 0);
        _cameraMn.SetCameraHeight(delta.y * -0.01f);
        _selectRotateY = -360;
        _isDrag = true;
    }
    
}
