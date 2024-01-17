using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsController : MonoBehaviour
{
    public Hand RightHand;
    public Pen Pen;

    void Start()
    {
        Pen.gameObject.SetActive(false);

        RightHand.BindHandPoseStartCallback(HandPose.ToolHolding, () => {
            Pen.gameObject.SetActive(true);
            RightHand.HandRenderer.enabled = false;

        });

        RightHand.BindHandPoseStartCallback(HandPose.ToolRemoving, () => {
            Pen.gameObject.SetActive(false);
            RightHand.HandRenderer.enabled = true;
        });
    }

    void Update()
    {
        Pen.SetVelocity(RightHand.PalmVelocity);

        if (Pen.gameObject.activeSelf == true)
        {
            Pen.transform.position = RightHand.PalmTransform.position;
        }
    }
}
