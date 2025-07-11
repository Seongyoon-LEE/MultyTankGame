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
        PhotonNetwork.IsMessageQueueRunning = true; // �����Ʈ��ũ �����κ��� ���� �޽����� �޴´�.
        CreateTank(); // ��ũ ����
    }
    void CreateTank()
    {
        float pos = Random.Range(-100f, 100f);
        PhotonNetwork.Instantiate("Tank", new Vector3(pos, 3, pos), Quaternion.identity, 0, null); // ��ũ ����
    }
    //public void CursorLockUnLock()
    //{
    //    if(Input.GetKey(KeyCode.Escape))
    //    {
    //        Cursor.lockState = CursorLockMode.None; // Ŀ�� ��� ����
    //        Cursor.visible = true; // Ŀ�� ���̱�
    //    }
    //    if(Input.GetMouseButtonDown(1))
    //    {
    //        Cursor.lockState = CursorLockMode.Locked; // Ŀ�� ���
    //        Cursor.visible = false; // Ŀ�� �����
    //    }
    //}
}
