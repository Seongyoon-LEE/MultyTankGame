using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
// RPC: Remote Procedure Call
public class FireCannon : MonoBehaviourPun
{
    TankInput input;
    public Transform firePos;
    [SerializeField] LeaserBeam leaserBeam;

    //public GameObject expEffect;
    public AudioSource source;
    [SerializeField] private AudioClip fireClip;
    [SerializeField] private AudioClip expClip;
    Ray ray;
    public int terrainLayer;
    public int tankLayer;
    public bool isHit = false;

    Vector3 hitPoint;
    Vector3 _normal;
    Quaternion rot;

    readonly string tankTag = "TANK";
    readonly string apacheTag = "APACHE";

    void Start()
    {
        input = GetComponent<TankInput>();
        firePos = transform.GetChild(4).GetChild(1).GetChild(1).GetComponent<Transform>();
        leaserBeam = firePos.GetChild(0).GetComponent<LeaserBeam>();
        source = GetComponent<AudioSource>();
        fireClip = Resources.Load<AudioClip>("Sounds/ShootMissile");
        expClip = Resources.Load<AudioClip>("Sounds/DestroyedExplosion");
        terrainLayer = LayerMask.NameToLayer("TERRAIN");
        tankLayer = LayerMask.NameToLayer("TANK");
    }

    void Update()
    {
        if (input.isFire)
        {
            if(photonView.IsMine)
            {
                Fire();
                photonView.RPC("Fire", RpcTarget.Others); // �ٸ� �÷��̾�� Fire �Լ��� ȣ��
            }
           
        }
    }
    [PunRPC] // ��Ʈ����Ʈ �������� �ִ� ��Ʈ��ũ ������  Fire �Լ��� ȣ���� �� �ֵ���
    void Fire()
    {
        SoundManager.s_Instance.PlaySfx(firePos.position, fireClip, false);
        RaycastHit hit;
        ray = new Ray(firePos.position, firePos.forward);
        if (Physics.Raycast(ray, out hit, 200f, 1 << terrainLayer | 1 << tankLayer))
        {
            isHit = true;
            ShowEffect(hit);
            if (hit.collider.CompareTag(tankTag))
            {
                string tag = hit.collider.tag; // ���� �ݶ��̴��� �±׸� ���� 
                hit.collider.transform.SendMessage("OnDamage", tag, SendMessageOptions.DontRequireReceiver); // ���� ��ũ���� ������ ����

            }
        }
        else
        {
            isHit = false;

            leaserBeam.FireRay(); // ���� ������
            ShowEffect(hit);
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

        var eff = PoolingManager.p_Instance.GetExp();
        eff.transform.position = hitPoint;
        eff.transform.rotation = rot;
        StartCoroutine(ExpEffect(eff));

        //source.PlayOneShot(expClip, 1.0f);
        SoundManager.s_Instance.PlaySfx(hitPoint, expClip, false);
    }

    IEnumerator ExpEffect(GameObject eff)
    {
        eff.gameObject.SetActive(true);
        yield return new WaitForSeconds(1);
        eff.gameObject.SetActive(false);
    }

}
