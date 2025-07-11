using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
public class DisPlayUserID : MonoBehaviourPun
{
    public Text userIDText; // 유저 ID를 표시할 UI Text 컴포넌트

    void Start()
    {
        if (photonView != null)
        {
            // PhotonView가 있는 경우, 로컬 플레이어의 UserID를 가져와서 표시
            userIDText.text = photonView.Owner.NickName;
        }
    }
}
