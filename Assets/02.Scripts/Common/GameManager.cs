using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance = null;
    public bool isGameOver = false;
    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else if (Instance != this)
            Destroy(gameObject);
        PhotonNetwork.IsMessageQueueRunning = true; // 포톤네트워크 서버로부터 오는 메시지를 받는다.
        CreateTank(); // 탱크 생성
    }
    void CreateTank()
    {
        float pos = Random.Range(-100f, 100f);
        PhotonNetwork.Instantiate("Tank", new Vector3(pos, 3, pos), Quaternion.identity, 0, null); // 탱크 생성
    }
    //public void CursorLockUnLock()
    //{
    //    if(Input.GetKey(KeyCode.Escape))
    //    {
    //        Cursor.lockState = CursorLockMode.None; // 커서 잠금 해제
    //        Cursor.visible = true; // 커서 보이기
    //    }
    //    if(Input.GetMouseButtonDown(1))
    //    {
    //        Cursor.lockState = CursorLockMode.Locked; // 커서 잠금
    //        Cursor.visible = false; // 커서 숨기기
    //    }
    //}
}
