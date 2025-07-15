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
    public GameObject roomItem;
    public Transform scrollContents;
    public InputField roomName;
    private Dictionary<string, GameObject> roomItemCache = new Dictionary<string, GameObject>();

    void Awake()
    {
        if (!PhotonNetwork.IsConnected)
        {
            PhotonNetwork.GameVersion = Version;
            PhotonNetwork.ConnectUsingSettings();
            // 포톤 네트워크에서 접속
            roomName.text = $"Room_{Random.Range(0, 999):000}";
        }
        
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
        PhotonNetwork.CreateRoom("TankBattleFiledRoom", new RoomOptions { IsOpen = true, IsVisible = true, MaxPlayers = 20});
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

    // 방이 생성 되거나 삭제시 자동으로 호출되는 콜백 함수
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        // 1. 새로 받은 룸 리스트에서 현재 유효한 룸 이름들만 저장 (빠른 조회를 위해 HashSet 사용)
        HashSet<string> activeRoomNames = new HashSet<string>();
        foreach(RoomInfo roomInfo in roomList)
        {
            if (roomInfo.RemovedFromList == false) // 룸이 삭제되지 않은 경우
            {
                activeRoomNames.Add(roomInfo.Name);
            }
        }
        // 2. 현재 UI로 표시중인 룸(캐시) 중에서 사라져야할 룸을 찾아 삭제 (이부분이 나간 룸 삭제 로직)
        List<string> roomsToDelete = new List<string>();
        foreach(string cachedRoomName in roomItemCache.Keys)
        {
            if (!activeRoomNames.Contains(cachedRoomName)) // 현재 룸 리스트에 없는 룸 이름
            {
                roomsToDelete.Add(cachedRoomName);
            }
        }
        foreach(string roomNameToDelete in roomsToDelete)
        {
            // 해당 GameObject 파괴
            Destroy(roomItemCache[roomNameToDelete]);
            // 캐시에서 제거
            roomItemCache.Remove(roomNameToDelete);
        }
        // 3. 새로 받은 룸 리스트를 기준으로 UI 업데이트 및 생성
        foreach (RoomInfo roomInfo in roomList)
        {
            // 이미 삭제 처리된 방이면 건너 뛰기
            if (roomInfo.RemovedFromList)
                continue;

            GameObject roomItemObject;
            // 3-1 이미 존재하는 룸이면 정보만 업데이트
            if (roomItemCache.TryGetValue(roomInfo.Name, out roomItemObject))
            {
                RoomData roomData = roomItemObject.GetComponent<RoomData>();
                roomData.connectPlayer = roomInfo.PlayerCount;
                roomData.maxPlayer = roomInfo.MaxPlayers;
                roomData.DisPlayerRoomData(); // UI 텍스트 업데이트 
            }
            // 3-2 새로 생긴 룸이면 생성
            else
            {
                GameObject newRoom = Instantiate(roomItem, scrollContents.transform);
                RoomData roomData = newRoom.GetComponent<RoomData>();
                roomData.roomName = roomInfo.Name;
                roomData.connectPlayer = roomInfo.PlayerCount;
                roomData.maxPlayer = roomInfo.MaxPlayers;
                roomData.DisPlayerRoomData(); // UI 텍스트 업데이트

                roomData.GetComponent<Button>().onClick.AddListener(() =>
                    {
                        OnClickRoomItem(roomData.roomName);
                    });
                // 새로 생성한 룸을 캐시에 추가
                roomItemCache.Add(roomInfo.Name, newRoom);
            }
        }

        //foreach (GameObject obj in GameObject.FindGameObjectsWithTag("ROOMITEM"))
        //{
        //    // 삭제 할 때마다 룸을 다시 구성 한다 처음 부터
        //    Destroy(obj);
        //}

        // 삭제된 RoomItem 프리팹을 저장 할 임시 변수
        //GameObject tempRoom = null;
        //foreach (RoomInfo roomInfo in roomList)
        //{
        //    if (roomInfo.RemovedFromList == true)
        //    {
        //        // 딕셔너리에서 룸 이름으로 검색해 저장된 RoomItem 프리팹을 추출
        //        rooms.TryGetValue(roomInfo.Name, out tempRoom);

        //        // RoomItem 삭제
        //        Destroy(tempRoom);
        //        rooms.Remove(roomInfo.Name);
        //    }
        //    else // 룸 정보가 변경 된 경우
        //    {
        //        RoomData roomData = null;
        //        if (!rooms.ContainsKey(roomInfo.Name))
        //        {
        //            GameObject roomPrefab = Instantiate(roomItem, scrollContents);
        //            roomData = roomPrefab.GetComponent<RoomData>();

        //            // RoomItem Button 컴포넌트에 클릭 이벤트를 동적으로 연결
        //            // 이것을 동적 이벤트 리스너라고 한다.
        //            roomData.GetComponent<Button>().onClick.AddListener(delegate
        //            { OnClickRoomItem(roomData.roomName); });

        //            rooms.Add(roomInfo.Name, roomPrefab);
        //        }
        //        else // 룸 이름이 딕셔너리에 없는 경우에 룸 정보를 업데이트
        //        {
        //            rooms.TryGetValue(roomInfo.Name, out tempRoom);
        //            roomData = tempRoom.GetComponent<RoomData>();
        //        }
        //        roomData.roomName = roomInfo.Name;
        //        roomData.connectPlayer = roomInfo.PlayerCount;
        //        roomData.maxPlayer = roomInfo.MaxPlayers;

        //        roomData.DisPlayerRoomData();
        //        // 룸 이름이 딕셔너리에 없는 경우 새로 추가
        //        print("룸 정보가 변경");
        //    }
        //}


    }
    void OnClickRoomItem(string roomName)
    {
        PhotonNetwork.NickName = userId.text;
        PlayerPrefs.SetString("USER_ID", userId.text);
        PhotonNetwork.JoinRoom(roomName);

    }
    public void OnClickCreateRoom()
    {
        string _roomName = roomName.text;
        if (string.IsNullOrEmpty(_roomName))
        {
            _roomName = $"ROOM_{Random.Range(0, 999):000}";
        }

        PhotonNetwork.NickName = userId.text;
        PlayerPrefs.SetString("USER_ID", userId.text);
        RoomOptions roomOptions = new RoomOptions();
        roomOptions.IsOpen = true;
        roomOptions.IsVisible = true;
        roomOptions.MaxPlayers = 20;

        // 지정한 조건에 맞는 룸 생성
        PhotonNetwork.CreateRoom(_roomName, roomOptions, TypedLobby.Default);
    }
    public void OnGUI()
    {
        GUILayout.Label(PhotonNetwork.NetworkClientState.ToString());
    }

    
}
