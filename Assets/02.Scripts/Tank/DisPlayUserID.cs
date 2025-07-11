using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class DisPlayUserID : MonoBehaviourPun
{
    public Text userIDText; // ���� ID�� ǥ���� UI Text ������Ʈ

    void Start()
    {
        if (photonView != null)
        {
            // PhotonView�� �ִ� ���, ���� �÷��̾��� UserID�� �����ͼ� ǥ��
            userIDText.text = photonView.Owner.NickName;
        }
    }
}
