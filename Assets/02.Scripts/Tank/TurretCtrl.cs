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
        curRot = tr.localRotation; // ���� ȸ������ ����
    }
    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting) // �ڽ��� ��ũ�� �������� �۽� 
        {
            // ������ ȸ������ ��Ʈ��ũ ���� Ÿ�ο��� �۽�
            stream.SendNext(tr.localRotation);
        }
        else if (stream.IsReading) // ����Ʈ�� ��ũ�� �������� ����
        {
            // ����Ʈ�� ȸ������ ���� �޾Ƽ� ��Ʈ��ũ �� �������� ���� ������ �ϱ� ������
            curRot = (Quaternion)stream.ReceiveNext();
        }
    }
    void Update()
    {
        if (photonView.IsMine) // �����Ʈ��ũ�� ��ũ�� ���� ���̸� Ű����� ����   
        {
            ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            Debug.DrawRay(ray.origin, ray.direction * maxDistance, Color.green);

            if (Physics.Raycast(ray, out hit, 60f, 1 << 6))
            {
                Vector3 relative = tr.InverseTransformPoint(hit.point); // ������ ���� ������ ���� ��ǥ�� ���� ��ǥ�� ��ȯ
                                                                        // ����� = ��ź��Ʈ (��������.x, ��������.z) * PI * 2 / 360
                float angle = Mathf.Atan2(relative.x, relative.z) * Mathf.Rad2Deg;
                tr.Rotate(0f, angle * Time.deltaTime * rotSpeed, 0f);

            }
        }
        else // ���ð� ����Ʈ�� ȸ���� �ε巴�� ���� �ؾ� �Ѵ�
        {
            tr.localRotation = Quaternion.Slerp(tr.localRotation, curRot, Time.deltaTime * rotSpeed);
        }
    }

}
