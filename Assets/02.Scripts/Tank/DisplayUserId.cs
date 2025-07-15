using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;

public class DisplayUserId : MonoBehaviourPun
{
    public Text userId;
    void Start()
    {
        if (photonView != null)
        {
            userId.text = photonView.Owner.NickName;
        }
    }
}
