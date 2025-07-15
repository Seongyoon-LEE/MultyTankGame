using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;

public class CannonCtrl : MonoBehaviourPun, IPunObservable
{
    private TankInput input;
    private Transform tr;
    public float rotSpeed = 1000f;
    public float upperAngle = -30f;
    public float downAngle = 10f;
    public float currentRotage = 0f; // 현재 회전 각도
    Quaternion curRot = Quaternion.identity;
    void Start()
    {
        tr = transform;
        input = GetComponentInParent<TankInput>();
        curRot = tr.localRotation;
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(tr.localRotation);
        }
        else
        {
            curRot = (Quaternion)stream.ReceiveNext();
        }
    }
    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            float wheel = input.m_scrolWheel;
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
            else
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
        else
        {
            tr.localRotation = Quaternion.Lerp(tr.localRotation, curRot, Time.fixedDeltaTime * rotSpeed);
        }
    }

    
}
