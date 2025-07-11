using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// WS 앞뒤 AD 회전 마우스 휠로 포신 위 아래
// 마우스 x로 움직일 때 마다 삼각함수를 이용해서 터렛을 Y축으로 회전
// 역탄젠트 함수 Atan 아크탄젠트
public class TankInput : MonoBehaviour
{
    private readonly string hori = "Horizontal";
    private readonly string vert = "Vertical";
    private readonly string mouseScrollWheel = "Mouse ScrollWheel";
    private readonly string mouseX = "Mouse X";
    private readonly string fire = "Fire1";

    public float h = 0f;
    public float v = 0f;
    public float m_scrolWheel = 0f;
    public float m_x = 0f;
    public bool isFire = false;
    public float axisRaw = 0f;
    void Start()
    {
        
    }

    void Update()
    {
        if (GameManager.Instance != null && GameManager.Instance.isGameOver)
        {
            h = v = m_scrolWheel = m_x = axisRaw = 0f;
            isFire = false;
            return;
        }
        h = Input.GetAxis(hori);
        v = Input.GetAxis(vert);
        m_scrolWheel = Input.GetAxis(mouseScrollWheel);
        m_x = Input.GetAxis(mouseX);
        isFire = Input.GetButtonDown(fire);
        axisRaw = Input.GetAxisRaw(vert);
    }
}
