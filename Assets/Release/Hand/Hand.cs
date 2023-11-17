using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;
using System;
using System.Net.NetworkInformation;
using CurvedUI;

public enum HandPose
{
    None = 0,
    HandUpward = 1,
    HandDownward = 2,
    SkinnyPouring = 3,
    ScatterPouring = 4,
    FingertipTracing = 5,
    FingerCarving = 6,
    HandSweeping = 7,
    PalmRubbing = 8,
    UIActivation = 9,
    ToolHolding = 10,
    ToolRemoving = 11,
    Fist = 12,
}

public enum HandStatus
{
    Draw,
    Tools,
    UI,
    HeightAdjusting,
}

public class Hand : MonoBehaviour
{
    public HandType handType;
    public bool IsValid { get; private set; }
    //public bool EnableCollisionTest { get; private set; }

    public HandPose CurrentHandPose { get; private set; } = HandPose.None;
    public HandStatus CurrentHandStatus { get; private set; } = HandStatus.Draw;

    public Bounds HandBound { get; private set; }
    public Vector3 Velocity { get; private set; }
    public float Strength { private set; get; } 
    public Vector3 SkinnyPouringCenter { private set; get; }
    public Bounds ScatterPouringCenter { private set; get; }


    public GameObject HandMesh;
    public Renderer HandRenderer;

    public Transform IndexTipTransform;

    public Transform PalmTransform;


    private Vector3 LastPalmPosition;

    public List<Transform> handJoints = new List<Transform>(new Transform[(int)HandJoint.JointMax]);
    private HandJointLocations handJointLocations = new HandJointLocations();

    private Dictionary<HandPose, Action> OnHandPoseStart = new Dictionary<HandPose, Action>();
    private Dictionary<HandPose, Action> OnHandPoseUpdate = new Dictionary<HandPose, Action>();
    private Dictionary<HandPose, Action> OnHandPoseEnd = new Dictionary<HandPose, Action>();


    private void Start()
    {
        IsValid = false; ;
        HandMesh.SetActive(false);
        //EnableCollisionTest = true;
        CurrentHandStatus = HandStatus.Draw;
        CurrentHandPose = HandPose.None;
    }

    private void Update()
    {
        UpdateHandJoints();

        if (CurrentHandPose == HandPose.SkinnyPouring)
        {
            Vector3 center = handJoints[22].position;
            center += handJoints[23].position;
            center += handJoints[24].position;
            center += handJoints[25].position;
            SkinnyPouringCenter = (center / 4.0f);

        }

        if (CurrentHandPose == HandPose.ScatterPouring)
        {

            Vector3 minPoint = handJoints[0].position;
            Vector3 maxPoint = handJoints[0].position;

            foreach (Transform trans in handJoints)
            {
                Vector3 pos = trans.position;

                if (pos.x < minPoint.x) minPoint.x = pos.x;
                if (pos.y < minPoint.y) minPoint.y = pos.y;
                if (pos.z < minPoint.z) minPoint.z = pos.z;

                if (pos.x > maxPoint.x) maxPoint.x = pos.x;
                if (pos.y > maxPoint.y) maxPoint.y = pos.y;
                if (pos.z > maxPoint.z) maxPoint.z = pos.z;
            }
            ScatterPouringCenter = new Bounds((maxPoint + minPoint) * 0.5f, maxPoint - minPoint);
        }

        // update bounds
        HandBound = HandRenderer.bounds;

        // update velocity
        Velocity = (PalmTransform.position - LastPalmPosition) / Time.deltaTime;
        LastPalmPosition = PalmTransform.position;
    }

    private void UpdateHandJoints()
    {
        if (PXR_HandTracking.GetJointLocations(handType, ref handJointLocations))
        {
            if (handJointLocations.isActive == 0)
            {
                SetIsValid(false);
                return;
            };

            SetIsValid(true);
            UpdateAimState();
            transform.localScale = Vector3.one * handJointLocations.handScale;

            for (int i = 0; i < handJoints.Count; ++i)
            {
                if (handJoints[i] == null) continue;

                if (i == (int)HandJoint.JointWrist)
                {
                    handJoints[i].localPosition = handJointLocations.jointLocations[i].pose.Position.ToVector3();
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
        else
        {
            SetIsValid(false);
        }
    }

    public void SetIsValid(bool newState)
    {
        if (newState != IsValid)
        {
            HandMesh.SetActive(newState);
            //EnableCollisionTest = newState;
            IsValid = newState;
        }
    }

    /*
    public void SetEnableCollisionTest(bool enbale)
    {
        if (enbale != EnableCollisionTest)
        {
            EnableCollisionTest = enbale;
        }
    }
    */

    public void SetHandPose(HandPose handPose)
    {
        if (handPose != CurrentHandPose)
        {
            if (OnHandPoseEnd.ContainsKey(CurrentHandPose))
            {
                OnHandPoseEnd[CurrentHandPose]?.Invoke();
            }
            HandlePoseEnd(CurrentHandPose);
            CurrentHandPose = handPose;
            HandlePoseStart(CurrentHandPose);
            if (OnHandPoseStart.ContainsKey(CurrentHandPose))
            {
                OnHandPoseStart[CurrentHandPose]?.Invoke();
            }
        }
    }

    private void HandlePoseStart(HandPose state)
    {
        switch (state)
        {
            case HandPose.HandUpward:
                CurrentHandStatus = HandStatus.HeightAdjusting;
                break;
            case HandPose.ToolHolding:
                CurrentHandStatus = HandStatus.Tools;
                break;
            case HandPose.ToolRemoving:
                CurrentHandStatus = HandStatus.Draw;
                break;
        }
    }

    private void HandlePoseEnd(HandPose state)
    {
        switch (state)
        {
            case HandPose.HandUpward:
                CurrentHandStatus = HandStatus.Draw;
                break;
        }
    }

    // TODO: 允许碰撞这一块还有bug！
    /*
    public void SetEnableCollisionTest(bool enableCollision)
    {
        EnableCollisionTest = CurrentHandPose == HandPose.None && enableCollision;
    }
        */
    public void BindHandPoseStartCallback(HandPose handPose, Action callback)
    {
        if (!OnHandPoseStart.ContainsKey(handPose))
        {
            OnHandPoseStart[handPose] = callback;
        }
        else
        {
            OnHandPoseStart[handPose] += callback;
        }
    }

    public void BindHandPoseEndCallback(HandPose handPose, Action callback)
    {
        if (!OnHandPoseEnd.ContainsKey(handPose))
        {
            OnHandPoseEnd[handPose] = callback;
        }
        else
        {
            OnHandPoseEnd[handPose] += callback;
        }
    }

    public void BindHandPoseUpdateCallback(HandPose handPose, Action callback)
    {
        if (!OnHandPoseUpdate.ContainsKey(handPose))
        {
            OnHandPoseUpdate[handPose] = callback;
        }
        else
        {
            OnHandPoseUpdate[handPose] += callback;
        }
    }

    private void UpdateAimState()
    {
        Strength = Vector3.Distance(handJointLocations.jointLocations[5].pose.Position.ToVector3() ,handJointLocations.jointLocations[8].pose.Position.ToVector3());
    }


}