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
    public float currentRotage = 0f; // ���� ȸ�� ����

    Quaternion curRot = Quaternion.identity; // ���� ȸ������ ����
    void Start()
    {
        tr = transform;
        input = GetComponentInParent<TankInput>();
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
            else // ������ ������
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
        else // ���ð� ����Ʈ�� ȸ���� �ε巴�� ���� �ؾ� �Ѵ�
        {
            tr.localRotation = Quaternion.Slerp(tr.localRotation, curRot, Time.deltaTime * rotSpeed);
        }
    }

}
