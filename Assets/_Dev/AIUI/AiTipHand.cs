using System.Collections;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;

public class AiTipHand : MonoBehaviour
{
    public List<Transform> HandJoints = new List<Transform>(new Transform[(int)HandJoint.JointMax]);

}
