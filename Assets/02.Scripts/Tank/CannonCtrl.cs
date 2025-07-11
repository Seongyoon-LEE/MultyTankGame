using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
public class CannonCtrl : MonoBehaviourPun, IPunObservable
{
    private TankInput input;
    private Transform tr;
    public float rotSpeed = 1500f;
    public float upperAngle = -30f;
    public float downAngle = 0f;
    public float currentRotage = 0f; // 현재 회전 각도

    Quaternion curRot = Quaternion.identity; // 현재 회전값을 저장
    void Start()
    {
        tr = transform;
        input = GetComponentInParent<TankInput>();
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
        if (photonView.IsMine)
        {
            float wheel = -input.m_scrolWheel;
            float angle = Time.deltaTime * rotSpeed * wheel;
            currentRotage += angle;
            if (wheel <= -0.01f)
            {

                if (currentRotage > upperAngle)
                {
                    tr.Rotate(angle, 0f, 0f);
                }
                else
                {
                    currentRotage = upperAngle;
                }
            }
            else // 포신을 내릴때
            {
                if (currentRotage < downAngle)
                {
                    tr.Rotate(angle, 0f, 0f);
                }
                else
                {
                    currentRotage = downAngle;
                }
            }
        }
        else // 로컬과 리모트의 회전은 부드럽게 구현 해야 한다
        {
            tr.localRotation = Quaternion.Slerp(tr.localRotation, curRot, Time.deltaTime * rotSpeed);
        }
    }

}
