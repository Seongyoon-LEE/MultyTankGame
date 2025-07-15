using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ApacheMove : MonoBehaviour
{
    public enum AppacheState { PATROL, ATTACK, DESTROY }
    public AppacheState state = AppacheState.PATROL;

    private Transform tr;
    public List<Transform> nodePoints;
    public int currentIdx = 0;
    float rotSpeed = 15f, moveSpeed = 10f;
    void Start()
    {

        tr = transform;
        var path = GameObject.Find("Points").transform;
        if (path != null)
            path.GetComponentsInChildren<Transform>(nodePoints);

        nodePoints.RemoveAt(0);
    }

    void FixedUpdate()
    {
        WayPointMove();
        CheckDistance();
    }

    void WayPointMove()
    {
        Vector3 movePos = nodePoints[currentIdx].position - tr.position;

        tr.rotation = Quaternion.Slerp(tr.rotation, Quaternion.LookRotation(movePos), Time.fixedDeltaTime * moveSpeed);
        tr.Translate(Vector3.forward * rotSpeed * Time.fixedDeltaTime);
    }
    void CheckDistance()
    {
        // 경로 체크를 해서  인덱스틑 다시 0으로

        if (Vector3.Distance(transform.position, nodePoints[currentIdx].position) <= 5f)
        {
            if (currentIdx == nodePoints.Count - 1)
                currentIdx = 0;
            else
                currentIdx++;
        }
    }
}
