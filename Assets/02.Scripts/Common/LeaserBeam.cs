using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LeaserBeam : MonoBehaviour
{
    [SerializeField] private LineRenderer lineRenderer;
    Transform tr;

    void Start()
    {
        lineRenderer = GetComponent<LineRenderer>();
        lineRenderer.useWorldSpace = false;
        lineRenderer.enabled = false;

        tr = transform;
    }

    public void FireRay()
    {
        Ray ray = new Ray(tr.position + Vector3.up * 0.02f, tr.forward);
        RaycastHit hit;
        lineRenderer.SetPosition(0, tr.InverseTransformPoint(ray.origin));

        if (Physics.Raycast(ray, out hit, 200f, 1 << 6))
        {
            // 끝점은 맞은 위치로 잡고
            lineRenderer.SetPosition(1, tr.InverseTransformPoint(hit.point));
        }
        else
        {
            lineRenderer.SetPosition(1, tr.InverseTransformPoint(ray.GetPoint(200f)));
        }
        StartCoroutine(ShowLeaserBeam());
    }

    IEnumerator ShowLeaserBeam()
    {
        lineRenderer.enabled = true;
        yield return new WaitForSeconds(Random.Range(0.1f, 0.3f));
        lineRenderer.enabled = false;
    }
}
