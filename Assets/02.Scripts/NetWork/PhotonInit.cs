using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhotonInit : MonoBehaviourPunCallbacks // Pun 네트워크 관련 라이브러리
{
    public string Version = "V1.1.0";
    public InputField userId;
    void Awake()
    {
        PhotonNetwork.GameVersion = Version;
        PhotonNetwork.ConnectUsingSettings();
        // 포톤 네트워크에서 접속

    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log($"마스터 클라이언트 접속");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("로비에 접속");
        //PhotonNetwork.JoinRandomRoom(); // 무작위의 방에 접속
        userId.text = GetUserID();
    }
    string GetUserID()
    {
        string userId = PlayerPrefs.GetString("USER_ID");
        if (string.IsNullOrEmpty(userId))
        {
            userId = $"USER_ {Random.Range(0, 999):000}";
        }

        return userId;
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("룸 접속 실패");
        PhotonNetwork.CreateRoom("TankBattleFiledRoom", new RoomOptions { IsOpen = true, IsVisible = true, MaxPlayers = 20 });
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("정상적으로 룸 접속!");
        //CreateTank();
        StartCoroutine(LoadBattleFiled()); // 다른 씬으로 이동하기 위해 스타트 코루틴 선언
    }

    IEnumerator LoadBattleFiled()
    {
        // 씬 이동하는 동안 포콘 클라우드 서버로 부터 네트워크 메시지 수신 중단
        PhotonNetwork.IsMessageQueueRunning = false;
        // 비동기적으로 씬로딩
        AsyncOperation ao = SceneManager.LoadSceneAsync("BattleFieldScene");

        yield return ao; // 비동기적으로 리턴
    }

    public void OnClickJoinRandomRoom()
    {
        PhotonNetwork.NickName = userId.text;
        PlayerPrefs.SetString("USER_ID", userId.text);
        PhotonNetwork.JoinRandomRoom();
    }
    public void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());

    }


}
