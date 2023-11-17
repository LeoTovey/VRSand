using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PicoHandTracking : MonoBehaviour
{

    public HandController Hands;

    public void LeftHandStart()
    {
        Hands.SetLeftHandPose(HandPose.UIActivation);
    }

    public void LeftHandEnd()
    {
        Hands.SetLeftHandPose(HandPose.None);
    }

    public void RightHandStart()
    {
        Hands.SetRightHandPose(HandPose.HandUpward);
    }

    public void RightHandEnd()
    {
        Hands.SetRightHandPose(HandPose.None);
    }
}
