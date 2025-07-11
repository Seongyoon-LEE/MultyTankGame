using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Photon.Pun;
public class TankMoveAndRotate : MonoBehaviourPun, IPunObservable
{
    public float moveSpeed = 12f;
    public float rotSpeed = 90f;
    Transform tr;
    Rigidbody rb;
    TankInput input;
    Vector3 curPos = Vector3.zero; // 다른 네트워크의 위치값을 저장할 변수
    Quaternion curRot = Quaternion.identity; // 다른 네트워크의 회전값을 저장할 변수

    IEnumerator Start()
    {
        tr = transform;
        input = GetComponent<TankInput>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0f, -0.5f, 0f);
        curPos = tr.position; // 현재 위치값을 저장
        curRot = tr.rotation; // 현재 회전값을 저장

        yield return null;
        if (photonView != null)
        {
            if (photonView.IsMine) // 포턴네트워크상 포톤뷰가 내 것이라면
            {
                // 카메라를 내 탱크에 맞춘다.
                CinemachineVirtualCamera vCam = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
                vCam.Follow = tr;
                vCam.LookAt = tr;
            }
        }
    }
    // 로컬의 이동 회전을 네트워크 상의 타인인 리모트에 송신하고
    // 반대로 리모트의 이동 회전을 수신 받아서 네트워크 상 움직임이 서로 보여야 하기 때문에
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // 자신의 탱크의 움직임을 송신 
        {
            // 로컬의 이동 회전을 네트워크 상의 타인에게 송신
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }
        else if (stream.IsReading) // 리모트의 탱크의 움직임을 수신
        {
            // 리모트의 이동 회전을 수신 받아서 네트워크 상 움직임이 서로 보여야 하기 때문에
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();

        }
    }
        void FixedUpdate()
        {
            if (photonView.IsMine) // 포톤네트워크상에 탱크가 나의 것이면 키보드로 조작 
            {
                tr.Translate(Vector3.forward * input.v * moveSpeed * Time.deltaTime);
                tr.Rotate(Vector3.up * input.h * rotSpeed * Time.deltaTime);
            }
            else // 포톤네트워크상에 탱크가 내 것이 아니면 다른 네트워크의 위치값과 회전값을 적용
            {
                tr.position = Vector3.Lerp(tr.position, curPos, Time.deltaTime * 3f);
                tr.rotation = Quaternion.Slerp(tr.rotation, curRot, Time.deltaTime * 3f);
            }
        }
    }

