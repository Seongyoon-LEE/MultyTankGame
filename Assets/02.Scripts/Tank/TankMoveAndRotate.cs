using System.Collections;
using System.Collections.Generic;
using Cinemachine;
using Photon.Pun;

using UnityEngine;

public class TankMoveAndRotate : MonoBehaviourPun, IPunObservable
{
    public float moveSpeed = 12f;
    public float rotSpeed = 90f;
    Transform tr;
    Rigidbody rb;
    TankInput input;

    Vector3 curPos = Vector3.zero;
    Quaternion curRot = Quaternion.identity;
    IEnumerator Start()
    {
        tr = transform;
        input = GetComponent<TankInput>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0f, -0.5f, 0f);
        curPos = tr.position;
        curRot = tr.rotation;
        yield return null;
        if (photonView != null)
        {
            if (photonView.IsMine) // 포톤네트워크상 포톤뷰가 나의것이라면
            {
                CinemachineVirtualCamera vcam = FindObjectOfType<CinemachineVirtualCamera>();
                vcam.Follow = transform;
                vcam.LookAt = transform;
            }
        }
        
    }

    // 로컬의 이동 회전을 네트워크 상의 타인인 리모트에 송신하고
    // 반대로 리모트의 이동 회전을 수신 받아서 네트워크 상 움직임 서로 보여야 하기 때문에
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // 자신의 탱크 움직임을 송신
        {
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }
        else // 리모트의 움직임을 수신
        {
            curPos = (Vector3) stream.ReceiveNext();
            curRot = (Quaternion) stream.ReceiveNext();
        }
    }

    void FixedUpdate()
    {
        if (photonView.IsMine)
        {
            tr.Translate(Vector3.forward * input.v * moveSpeed * Time.fixedDeltaTime);
            tr.Rotate(Vector3.up * input.h * rotSpeed * Time.fixedDeltaTime);
        }
        else
        {
            tr.localPosition = Vector3.Lerp(tr.localPosition, curPos, Time.fixedDeltaTime * moveSpeed);
            tr.localRotation = Quaternion.Lerp(tr.localRotation, curRot, Time.fixedDeltaTime * rotSpeed);
        }
        
    }
}
