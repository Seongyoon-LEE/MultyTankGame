using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathPoint : MonoBehaviour
{
    public Color lineColor = Color.red;
    public List<Transform> nodeList;
    float radius = 3f;
    private void OnDrawGizmos()
    {
        Gizmos.color = lineColor;
        Transform[] pathTr = GetComponentsInChildren<Transform>();
        if (pathTr != null )
            nodeList = new List<Transform>(pathTr);

        nodeList.RemoveAt(0);

        for (int i = 0; i < nodeList.Count; i++)
        {
            Vector3 currentNode = nodeList[i].position;
            Vector3 previousNode = Vector3.zero;
            if (i > 0)
                previousNode = nodeList[i - 1].position;
            else if (i == 0 && nodeList.Count > 1)
                previousNode = nodeList[nodeList.Count - 1].position;

            Gizmos.DrawLine(previousNode, currentNode);
            Gizmos.DrawSphere(currentNode, radius);
        }
    }
}
