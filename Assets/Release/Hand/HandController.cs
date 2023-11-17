using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public Hand LeftHand;
    public Hand RightHand;

    // Start is called before the first frame update
    void Start()
    {


    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetLeftHandPose(HandPose handPose)
    {
        if (handPose != LeftHand.CurrentHandPose)
        {
            HandleLeftPoseEnd(LeftHand.CurrentHandPose);
            LeftHand.SetHandPose(handPose);
            HandleLeftPoseStart(LeftHand.CurrentHandPose);
        }
    }

    public void SetRightHandPose(HandPose handPose)
    {
        if (handPose != RightHand.CurrentHandPose)
        {
            HandleRightPoseEnd(RightHand.CurrentHandPose);
            RightHand.SetHandPose(handPose);
            HandleRightPoseStart(RightHand.CurrentHandPose);
        }
    }


    private void HandleLeftPoseStart(HandPose state)
    {
        switch (state)
        {
            case HandPose.HandUpward:
                break;
        }
    }

    private void HandleLeftPoseEnd(HandPose state)
    {
        switch (state)
        {
            case HandPose.HandUpward:
                break;
        }
    }

    private void HandleRightPoseStart(HandPose state)
    {
        switch (state)
        {
            case HandPose.FingerCarving:
                //CollisionCamera.cullingMask = jointCullingMask;
                break;
        }
    }

    private void HandleRightPoseEnd(HandPose state)
    {
        switch (state)
        {
            case HandPose.FingerCarving:
                //CollisionCamera.cullingMask = handCullingMask;
                break;
        }
    }
}
