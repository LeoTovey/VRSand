using System.Collections;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;

public class HandJointRender : MonoBehaviour
{
    public Transform[] waypoints; // ��Ķ��Transform��
    private LineRenderer lineRenderer;
    public Hand RightHand;

    void Start()
    {
        // ����LineRenderer��������ò���
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    void Update()
    {
        // ÿһ֡�����ߵ�λ��
        //UpdateLineRenderer();
    }

 
}
