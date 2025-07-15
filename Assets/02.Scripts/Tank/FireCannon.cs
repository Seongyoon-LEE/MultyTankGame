using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

// RPC : Remote Procedual Call
public class FireCannon : MonoBehaviourPun
{
    TankInput input;
    public Transform firePos;
    [SerializeField] LeaserBeam leaserBeam;

    public GameObject expEffect;
    public AudioSource source;
    [SerializeField] private AudioClip fireClip;
    [SerializeField] private AudioClip expClip;
    Ray ray;
    public int terrainLayer;
    public int TankLayer;
    public bool isHit = false;

    Vector3 hitPoint;
    Vector3 _normal;
    Quaternion rot;
    //GameObject eff;
    private readonly string tankTag = "TANK";
    private readonly string apacheTag = "APACHE";

    void Start()
    {
        input = GetComponent<TankInput>();
        firePos = transform.GetChild(4).GetChild(1).GetChild(1).GetComponent<Transform>();
        leaserBeam = firePos.GetChild(0).GetComponent<LeaserBeam>();
        //leaserBeam = GetComponentInChildren<LeaserBeam>();
        //firePos = leaserBeam.transform.parent;
        source = GetComponent<AudioSource>();
        expEffect = Resources.Load<GameObject>("Effects/BigExplosionEffect");
        fireClip = Resources.Load<AudioClip>("Sounds/ShootMissile");
        expClip = Resources.Load<AudioClip>("Sounds/DestroyedExplosion");
        terrainLayer = LayerMask.NameToLayer("TERRAIN");
        TankLayer = LayerMask.NameToLayer("TANK");

    }

    void Update()
    {
        if (HoverEvent.event_Instance.isEnter) return;
        if (input.isFire)
        {
            if (photonView.IsMine)
            {
                Fire();
                photonView.RPC("Fire", RpcTarget.Others);
            }
        }
    }

    [PunRPC] // 어트리뷰트 원격지에 있는 네트워크 유저가 Fire 함수를 호출 할 수 있게
    void Fire()
    {
        if (this.gameObject.tag != tankTag) return;
        source.PlayOneShot(fireClip, 1.0f);
        RaycastHit hit;
        ray = new Ray(firePos.position, firePos.forward);
        if (Physics.Raycast(ray, out hit, 200f, 1 << terrainLayer | 1 << TankLayer))
            isHit = true;
        else
            isHit = false;

        
        leaserBeam.FireRay(); // 라인 랜더러

        ShowEffect(hit);

        if (photonView.IsMine && (hit.collider != null && hit.collider.CompareTag(tankTag)))
        {
            string tag = hit.collider.tag;
            hit.collider.transform.SendMessage("OnDamage", tag, SendMessageOptions.DontRequireReceiver);
        }
    }


    void ShowEffect(RaycastHit hit)
    {
        if (isHit)
            hitPoint = hit.point;
        else
            hitPoint = ray.GetPoint(200f);

        _normal = (firePos.position - hitPoint).normalized;
        rot = Quaternion.FromToRotation(-Vector3.forward, _normal);

        //var eff = Instantiate(expEffect, hitPoint, rot);
        //Destroy(eff, 1f);
        var eff = PoolingManager.p_Instance.GetExp();
        eff.transform.position = hitPoint;
        eff.transform.rotation = rot;
        StartCoroutine(ExpEffect(eff));

        

        source.PlayOneShot(expClip, 1.0f);
    }

    IEnumerator ExpEffect(GameObject eff)
    {
        eff.SetActive(true);
        yield return new WaitForSeconds(1);
        eff.SetActive(false);
    }
}
