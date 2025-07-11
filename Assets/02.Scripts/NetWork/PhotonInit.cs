using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PhotonInit : MonoBehaviourPunCallbacks // Pun ��Ʈ��ũ ���� ���̺귯��
{
    public string Version = "V1.1.0";
    public InputField userId;
    void Awake()
    {
        PhotonNetwork.GameVersion = Version;
        PhotonNetwork.ConnectUsingSettings();
        // ���� ��Ʈ��ũ���� ����

    }

    public override void OnConnectedToMaster()
    {
        base.OnConnectedToMaster();
        Debug.Log($"������ Ŭ���̾�Ʈ ����");
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        base.OnJoinedLobby();
        Debug.Log("�κ� ����");
        //PhotonNetwork.JoinRandomRoom(); // �������� �濡 ����
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
        Debug.Log("�� ���� ����");
        PhotonNetwork.CreateRoom("TankBattleFiledRoom", new RoomOptions { IsOpen = true, IsVisible = true, MaxPlayers = 20 });
    }

    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Debug.Log("���������� �� ����!");
        //CreateTank();
        StartCoroutine(LoadBattleFiled()); // �ٸ� ������ �̵��ϱ� ���� ��ŸƮ �ڷ�ƾ ����
    }

    IEnumerator LoadBattleFiled()
    {
        // �� �̵��ϴ� ���� ���� Ŭ���� ������ ���� ��Ʈ��ũ �޽��� ���� �ߴ�
        PhotonNetwork.IsMessageQueueRunning = false;
        // �񵿱������� ���ε�
        AsyncOperation ao = SceneManager.LoadSceneAsync("BattleFieldScene");

        yield return ao; // �񵿱������� ����
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
