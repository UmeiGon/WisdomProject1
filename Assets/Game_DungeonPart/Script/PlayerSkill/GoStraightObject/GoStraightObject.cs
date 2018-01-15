using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GoStraightObject : MonoBehaviour
{

    float timeCount = 0;
    float needTime = 0;
    public Vector3 targetPos;
    Vector3 moveDir;
    GameObject parent;
    public GameObject actionParent;
    public BattleParticipant target;

    public float moveSpeed = 3.0f;
    [SerializeField]
    GameObject hitEffect;
    [SerializeField]
    float hitEffExistTime = 2.0f;
    [SerializeField] bool cameraShake = false;
    [SerializeField] float shakeMagnitude = 1.0f;
    CameraManager _camera;

    // Use this for initialization
    void Start()
    {
        parent = GameObject.Find("GameObjectParent");
        if (cameraShake)
            _camera = parent.GetComponentInChildren<CameraManager>();
    }

    // Update is called once per frame
    void Update()
    {
        if (needTime != 0)
        {
            if (timeCount >= needTime)
            {
                if (cameraShake) _camera.CameraShake(shakeMagnitude);
                actionParent.GetComponent<StraightShot>().HitAndParamChange();
                if (hitEffect)
                {
                    var hitEff = Instantiate(hitEffect);
                    hitEff.transform.position = transform.position;
                    if (target) hitEff.GetComponent<Seek>().target = target.gameObject;
                    Destroy(hitEff, hitEffExistTime);
                }
                Destroy(this.gameObject);
                return;
            }
            transform.position += moveDir * Time.deltaTime * moveSpeed;
            timeCount += Time.deltaTime;
        }
        else    // 起こり得ない
        {
            if (transform.position == targetPos)
            {
                needTime = 10;
                timeCount = 15;
            }
        }
    }

    public void SetTarget(Vector3 pos)
    {
        targetPos = pos;
        moveDir = pos - transform.position;
        needTime = moveDir.magnitude / moveSpeed;
        moveDir = moveDir.normalized;
    }
}
