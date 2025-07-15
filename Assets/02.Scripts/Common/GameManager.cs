using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Photon.Realtime;
using Unity.VisualScripting;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance = null;
    public bool isGameOver = false;
    public Text txtConnect;
    public Text txtLogMsg;

    [SerializeField] private GameObject apachPrefab;
    [SerializeField] private List<Transform> spawnPointList;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        // 포콘네트워크 서버로 부터 오는 메시지를 받는다.
        PhotonNetwork.IsMessageQueueRunning = true;
        CreateTank();
        var spawnPoint = GameObject.Find("SpawnPoints").transform;
        if(spawnPoint != null)
        spawnPoint.GetComponentsInChildren<Transform>(spawnPointList);
        spawnPointList.RemoveAt(0); // 첫번째는 자기 자신이므로 제거

    }

    private void Start()
    {
        string msg = "\n<color=#00FF00>["+PhotonNetwork.NickName+"] Conneted</color>";
        photonView.RPC("LogMag", RpcTarget.AllBuffered, msg);
        if(spawnPointList.Count > 0 && PhotonNetwork.IsMasterClient)
        InvokeRepeating("CreateApach", 0.02f, 3.0f); // 0.02초 후에 시작해서 3초마다 호출 
    }
    void CreateTank()
    {
        float pos = Random.Range(-100f, 100f);
        PhotonNetwork.Instantiate("Tank", new Vector3(pos, 3, pos), Quaternion.identity, 0, null);
        // Resource에 있는 Tank 가져온기, 직접 받아온 프리팹을 넣을려면 넣고 name을 넣으면 됨
    }
    void CreateApach()
    {
        if (isGameOver) return;
        int count = (int)GameObject.FindGameObjectsWithTag("APACHE").Length;
        if (count < 10)
        {
            int idx = Random.Range(0, spawnPointList.Count);
            Transform spawnPoint = spawnPointList[idx];
            PhotonNetwork.InstantiateRoomObject(apachPrefab.name, spawnPointList[idx].position, spawnPointList[idx].rotation, 0, null);
            //PhotonNetwork.Instantiate 하면 방장 호스트가 나가면 같이 나가버림, 그래서 룸 씬에 붙어 있어야한다.
        }
    }

    [PunRPC]
    public void ApplyPlayerCountUpdate()
    {
        print("ApplyPlayerCountUpdate");
        Room currentRoom = PhotonNetwork.CurrentRoom;
        txtConnect.text = $"{currentRoom.PlayerCount:00} / {currentRoom.MaxPlayers:00}";
    }

    [PunRPC]
    public void GetConnectPlayerCount()
    {
        if (PhotonNetwork.IsMasterClient) // 방장
        {
            print("GetConnectPlayerCount");
            photonView.RPC("ApplyPlayerCountUpdate", RpcTarget.All);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer) // 유저가 입장했을 때
    {
        GetConnectPlayerCount();
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        GetConnectPlayerCount();
    }

    public void OnClickExitRoom()
    {
        string msg = "\n<color=#FF0000>[" + PhotonNetwork.NickName + "] DisConneted</color>";
        photonView.RPC("LogMag", RpcTarget.AllBuffered, msg);
        PhotonNetwork.LeaveRoom(); // 룸을 빠져나감(생성한 모든 네트워크 삭제)
    }
    public override void OnLeftRoom() // 룸에서 접속이 종료되었을 때 호출되는 콜백 함수
    {
        SceneManager.LoadScene("LobbyScene");
    }

    [PunRPC]
    public void LogMag(string msg)
    {
        txtLogMsg.text += msg;
    }
}
