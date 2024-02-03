using System.Collections;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEngine;

public class LittleHand : MonoBehaviour
{
    public HandType handType;
    public List<Transform> handJoints = new List<Transform>(new Transform[(int)HandJoint.JointMax]);
    private HandJointLocations handJointLocations = new HandJointLocations();

    private void Start()
    {

    }

    private void Update()
    {
        UpdateHandJoints();
    }

    private void UpdateHandJoints()
    {
        if (PXR_HandTracking.GetJointLocations(handType, ref handJointLocations))
        {
            if (handJointLocations.isActive == 0) return;

            //transform.localScale = Vector3.one * handJointLocations.handScale;

            for (int i = 0; i < handJoints.Count; ++i)
            {
                if (handJoints[i] == null) continue;

                if (i == (int)HandJoint.JointWrist)
                {
                    handJoints[i].localRotation = handJointLocations.jointLocations[i].pose.Orientation.ToQuat();
                }
                else
                {
                    UnityEngine.Pose parentPose = UnityEngine.Pose.identity;

                    if (i == (int)HandJoint.JointPalm ||
                        i == (int)HandJoint.JointThumbMetacarpal ||
                        i == (int)HandJoint.JointIndexMetacarpal ||
                        i == (int)HandJoint.JointMiddleMetacarpal ||
                        i == (int)HandJoint.JointRingMetacarpal ||
                        i == (int)HandJoint.JointLittleMetacarpal)
                    {
                        parentPose = new UnityEngine.Pose(handJointLocations.jointLocations[1].pose.Position.ToVector3(), handJointLocations.jointLocations[1].pose.Orientation.ToQuat());
                    }
                    else
                    {
                        parentPose = new UnityEngine.Pose(handJointLocations.jointLocations[i - 1].pose.Position.ToVector3(), handJointLocations.jointLocations[i - 1].pose.Orientation.ToQuat());
                    }

                    var inverseParentRotation = Quaternion.Inverse(parentPose.rotation);
                    handJoints[i].localRotation = inverseParentRotation * handJointLocations.jointLocations[i].pose.Orientation.ToQuat();
                }
            }
        }
    }




}
