using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;

public class RoomData : MonoBehaviourPun
{
    public string roomName = string.Empty;
    public int connectPlayer = 0;
    public int maxPlayer = 20;
    public Text txtRoomName;
    public Text txtConnectInfo;
    
    public void DisPlayerRoomData()
    {
        txtRoomName.text = roomName;
        txtConnectInfo.text = $"({connectPlayer:00}/{maxPlayer:00})";
    }
}
