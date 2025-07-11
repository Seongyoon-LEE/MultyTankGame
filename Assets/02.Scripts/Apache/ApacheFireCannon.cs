using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApacheFireCannon : MonoBehaviour
{
    [SerializeField] private Transform firePosL;
    [SerializeField] private Transform firePosR;
    [SerializeField] private LeaserBeam leaserBeamL;
    [SerializeField] private LeaserBeam leaserBeamR;

    [SerializeField] private AudioSource source;
    [SerializeField] private GameObject expEffect;
    [SerializeField] private AudioClip fireClip;
    [SerializeField] private AudioClip expClip;
    [SerializeField] private int terrainLayer;

    Ray ray;
    //public bool isHit = false;

    void Start()
    {
        firePosL = transform.GetChild(3).GetChild(0).GetComponent<Transform>();
        firePosR = transform.GetChild(3).GetChild(1).GetComponent<Transform>();
        leaserBeamL = firePosL.GetComponentInChildren<LeaserBeam>();
        leaserBeamR = firePosR.GetComponentInChildren<LeaserBeam>();

        source = GetComponent<AudioSource>();
        expEffect = Resources.Load<GameObject>("Effects/BigExplosionEffect");
        fireClip = Resources.Load<AudioClip>("Sounds/ShootMissile");
        expClip = Resources.Load<AudioClip>("Sounds/DestroyedExplosion");
        terrainLayer = LayerMask.NameToLayer("TERRAIN");
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            Fire(firePosL, leaserBeamL);
            Fire(firePosR, leaserBeamR);
        }
    }

    void Fire(Transform firePos, LeaserBeam leaserBeam)
    {
        source.PlayOneShot(fireClip, 1.0f);
        bool isHit;

        RaycastHit hit;
        ray = new Ray(firePos.position, firePos.forward);
        if (Physics.Raycast(ray, out hit, 200f, 1 << terrainLayer))
            isHit = true;
        else
            isHit = false;

        leaserBeam.FireRay(); // 라인 랜더러
        ShowEffect(hit, firePos, isHit);
    }

    void ShowEffect(RaycastHit hit, Transform firePos, bool isHit)
    {
        Vector3 hitPoint;

        if (isHit)
            hitPoint = hit.point;
        else
            hitPoint = ray.GetPoint(200f);

        Vector3 _normal = (firePos.position - hitPoint).normalized;
        Quaternion rot = Quaternion.FromToRotation(-Vector3.forward, _normal);

        var eff = Instantiate(expEffect, hitPoint, rot);
        Destroy(eff, 1.5f);
        source.PlayOneShot(expClip, 1.0f);

    }
}
