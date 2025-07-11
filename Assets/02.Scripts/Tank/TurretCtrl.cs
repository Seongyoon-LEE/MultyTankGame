using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class TurretCtrl : MonoBehaviourPun, IPunObservable
{
    Ray ray;
    RaycastHit hit;

    Transform tr;
    private float rotSpeed = 5f;
    [SerializeField] float maxDistance = 1000f;
    Quaternion curRot = Quaternion.identity;


    void Start()
    {
        tr = transform;
        curRot = tr.localRotation; // 현재 회전값을 저장
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // 자신의 탱크의 움직임을 송신 
        {
            // 로컬의 회전값을 네트워크 상의 타인에게 송신
            stream.SendNext(tr.localRotation);
        }
        else if (stream.IsReading) // 리모트의 탱크의 움직임을 수신
        {
            // 리모트의 회전값을 수신 받아서 네트워크 상 움직임이 서로 보여야 하기 때문에
            curRot = (Quaternion)stream.ReceiveNext();
        }
    }
    void Update()
    {
        if (photonView.IsMine) // 포톤네트워크상에 탱크가 나의 것이면 키보드로 조작   
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.green);

            if (Physics.Raycast(ray, out hit, 60f, 1 << 6))
            {
                Vector3 relative = tr.InverseTransformPoint(hit.point); // 광선이 맞은 지점인 월드 좌표를 로컬 좌표로 변환
                                                                        // 결과값 = 역탄젠트 (로컬지점.x, 로컬지점.z) * PI * 2 / 360
                float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
                tr.Rotate(0f, angle * Time.deltaTime * rotSpeed, 0f);

            }
        }
        else // 로컬과 리모트의 회전은 부드럽게 구현 해야 한다
        {
            tr.localRotation = Quaternion.Slerp(tr.localRotation, curRot, Time.deltaTime * rotSpeed);
        }
    }

}
