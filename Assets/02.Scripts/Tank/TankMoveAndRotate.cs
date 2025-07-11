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
    Vector3 curPos = Vector3.zero; // �ٸ� ��Ʈ��ũ�� ��ġ���� ������ ����
    Quaternion curRot = Quaternion.identity; // �ٸ� ��Ʈ��ũ�� ȸ������ ������ ����

    IEnumerator Start()
    {
        tr = transform;
        input = GetComponent<TankInput>();
        rb = GetComponent<Rigidbody>();
        rb.centerOfMass = new Vector3(0f, -0.5f, 0f);
        curPos = tr.position; // ���� ��ġ���� ����
        curRot = tr.rotation; // ���� ȸ������ ����

        yield return null;
        if (photonView != null)
        {
            if (photonView.IsMine) // ���ϳ�Ʈ��ũ�� ����䰡 �� ���̶��
            {
                // ī�޶� �� ��ũ�� �����.
                CinemachineVirtualCamera vCam = GameObject.FindObjectOfType<CinemachineVirtualCamera>();
                vCam.Follow = tr;
                vCam.LookAt = tr;
            }
        }
    }
    // ������ �̵� ȸ���� ��Ʈ��ũ ���� Ÿ���� ����Ʈ�� �۽��ϰ�
    // �ݴ�� ����Ʈ�� �̵� ȸ���� ���� �޾Ƽ� ��Ʈ��ũ �� �������� ���� ������ �ϱ� ������
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // �ڽ��� ��ũ�� �������� �۽� 
        {
            // ������ �̵� ȸ���� ��Ʈ��ũ ���� Ÿ�ο��� �۽�
            stream.SendNext(tr.position);
            stream.SendNext(tr.rotation);
        }
        else if (stream.IsReading) // ����Ʈ�� ��ũ�� �������� ����
        {
            // ����Ʈ�� �̵� ȸ���� ���� �޾Ƽ� ��Ʈ��ũ �� �������� ���� ������ �ϱ� ������
            curPos = (Vector3)stream.ReceiveNext();
            curRot = (Quaternion)stream.ReceiveNext();

        }
    }
        void FixedUpdate()
        {
            if (photonView.IsMine) // �����Ʈ��ũ�� ��ũ�� ���� ���̸� Ű����� ���� 
            {
                tr.Translate(Vector3.forward * input.v * moveSpeed * Time.deltaTime);
                tr.Rotate(Vector3.up * input.h * rotSpeed * Time.deltaTime);
            }
            else // �����Ʈ��ũ�� ��ũ�� �� ���� �ƴϸ� �ٸ� ��Ʈ��ũ�� ��ġ���� ȸ������ ����
            {
                tr.position = Vector3.Lerp(tr.position, curPos, Time.deltaTime * 3f);
                tr.rotation = Quaternion.Slerp(tr.rotation, curRot, Time.deltaTime * 3f);
            }
        }
    }

