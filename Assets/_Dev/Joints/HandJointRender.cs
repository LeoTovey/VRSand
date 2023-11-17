using System.Collections;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;

public class HandJointRender : MonoBehaviour
{
    public Transform[] waypoints; // 你的多个Transform点
    private LineRenderer lineRenderer;
    public Hand RightHand;

    void Start()
    {
        // 创建LineRenderer组件并设置参数
        lineRenderer = gameObject.AddComponent<LineRenderer>();
        lineRenderer.startWidth = 0.1f;
        lineRenderer.endWidth = 0.1f;
    }

    void Update()
    {
        // 每一帧更新线的位置
        //UpdateLineRenderer();
    }

 
}
