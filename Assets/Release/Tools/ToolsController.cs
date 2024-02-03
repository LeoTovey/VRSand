using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/*
public class ToolsController : MonoBehaviour
{
    public Hand RightHand;
    public Pen Pen;
    public float BufferTime = 1.0f;
    private float _toolHoldingTime = 0.0f;
    private float _toolRemovingTime = 0.0f;

    void Start()
    {
        Pen.gameObject.SetActive(false);

        
        RightHand.BindHandPoseStartCallback(HandPose.ToolHolding, () => {
            _toolHoldingTime += Time.deltaTime;
            if (_toolHoldingTime > BufferTime)
            {
                Pen.gameObject.SetActive(true);
                RightHand.HandRenderer.enabled = false;
            }

        });

        
        RightHand.BindHandPoseStartCallback(HandPose.ToolRemoving, () => {
            _toolRemovingTime += Time.deltaTime;
            if (_toolRemovingTime > BufferTime)
            {
                Pen.gameObject.SetActive(false);
                RightHand.HandRenderer.enabled = true;
            }
        });
        
        RightHand.BindHandPoseStartCallback(HandPose.ToolHolding, () => {
            _toolHoldingTime = 0.0f;

        });


        RightHand.BindHandPoseEndCallback(HandPose.ToolRemoving, () => {
            _toolRemovingTime = 0.0f;

        });
    }

    void Update()
    {
        if (RightHand.CurrentHandPose == HandPose.ToolHolding)
        {
            _toolHoldingTime += Time.deltaTime;
        }
        else if (RightHand.CurrentHandPose == HandPose.ToolRemoving)
        {
            _toolRemovingTime += Time.deltaTime;
        }

        if (_toolHoldingTime > BufferTime)
        {

        }

        Pen.SetVelocity(RightHand.PalmVelocity);

        if (Pen.gameObject.activeSelf == true)
        {
            Pen.transform.position = RightHand.PalmTransform.position;
        }
    }
}
*/