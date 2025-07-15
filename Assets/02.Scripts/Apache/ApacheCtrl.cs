using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApacheCtrl : MonoBehaviour
{
    public float moveSpeed = 0f;
    public float rotSpeed = 0f;
    Transform tr;
    public float VerticalSpeed = 0f;
    Rigidbody rb;
    void Start()
    {
        tr = transform;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        #region 좌우로 회전 하는 로직 A, D
        if (Input.GetKey(KeyCode.A))
            rotSpeed += -0.02f;
        else if (Input.GetKey(KeyCode.D))
            rotSpeed += 0.02f;
        else
        {
            if (rotSpeed > 0f) rotSpeed += -0.02f;
            else if (rotSpeed < 0f) rotSpeed += 0.02f;
        }
        tr.Rotate(Vector3.up * rotSpeed * Time.fixedDeltaTime);
        #endregion

        #region 앞뒤로 이동 하는 로직
        if (Input.GetKey(KeyCode.W))
            moveSpeed += 0.02f;
        else if (Input.GetKey(KeyCode.S))
            moveSpeed += -0.02f;
        else
        {
            if (moveSpeed > 0f) moveSpeed += -0.02f;
            else if (moveSpeed < 0f) moveSpeed += 0.02f;
        }
        tr.Translate(Vector3.forward * moveSpeed * Time.fixedDeltaTime, Space.Self);
        #endregion

        #region 상하로 이동 하는 로직
        if (Input.GetKey(KeyCode.Q))
            VerticalSpeed += 0.02f;
        else if (Input.GetKey(KeyCode.E))
            VerticalSpeed += -0.02f;
        else
        {
            if (VerticalSpeed > 0f) VerticalSpeed += -0.02f;
            else if (VerticalSpeed < 0f) VerticalSpeed += 0.02f;
        }
        //rb.MovePosition(Vector3.up * VerticalSpeed * Time.fixedDeltaTime);
        tr.Translate(Vector3.up * VerticalSpeed * Time.fixedDeltaTime, Space.Self);
        #endregion
    }
}
