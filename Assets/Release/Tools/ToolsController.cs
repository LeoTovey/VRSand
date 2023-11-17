using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToolsController : MonoBehaviour
{
    public Hand RightHand;
    public GameObject Pen;
    public Vector3 Velocity;

    private Vector3 lastPosition;

    void Start()
    {
        Pen.SetActive(false);

        RightHand.BindHandPoseStartCallback(HandPose.ToolHolding, () => {
            Pen.SetActive(true);
            RightHand.HandRenderer.enabled = false;

        });

        RightHand.BindHandPoseStartCallback(HandPose.ToolRemoving, () => {
            Pen.SetActive(false);
            RightHand.HandRenderer.enabled = true;
        });
    }

    // Update is called once per frame
    void Update()
    {
        Velocity = (Pen.transform.position - lastPosition) / Time.deltaTime;
        lastPosition = Pen.transform.position;

        if (Pen.gameObject.activeSelf == true)
        {
            Pen.transform.position = RightHand.PalmTransform.position;
        }
    }
}
