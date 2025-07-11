using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtCamera : MonoBehaviour
{
    public Transform mainCamTr;
    public Transform thisTr;
    void Start()
    {
        thisTr = transform;
        mainCamTr = Camera.main.transform;
    }

    void Update()
    {
        thisTr.LookAt(mainCamTr.position); // ī�޶� �ٶ󺸰� �Ѵ�.
    }
}
