using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.XR.PXR;
using System;
using System.Net.NetworkInformation;
using CurvedUI;
using KevinCastejon.HierarchicalFiniteStateMachine;
using Unity.VisualScripting;
using TMPro;

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

public class Hand : MonoBehaviour, ICollision
{
    public HandType handType;
    public bool IsValid { get; private set; }
    //public bool EnableCollisionTest { get; private set; }

    public HandPose CurrentHandPose { get; private set; } = HandPose.None;
    //public HandStatus CurrentHandStatus { get; private set; } = HandStatus.Draw;

    private HandFSM _fsm;

    public TextMeshProUGUI _text;


    public Bounds HandBound { get; private set; }
    public Vector3 PalmVelocity { get; private set; }
    public Vector3 IndexFingerTipVelocity { get; private set; }
    public Vector3 CenterVelocity => PalmVelocity;
    public Bounds CollisionBound => HandBound;
    public CollisionType CollisionType => CollisionType.Hand;

    public bool IsDraw => _fsm.GetCurrentStateEnumValue<HandState>() == HandState.DRAW;
    public bool IsTool => _fsm.GetCurrentStateEnumValue<HandState>() == HandState.TOOLS;
    public bool IsHeight => _fsm.GetCurrentStateEnumValue<HandState>() == HandState.HEIGHT;
    public bool IsUI => _fsm.GetCurrentStateEnumValue<HandState>() == HandState.UI;


    public bool DetectCollision()
    {
        return IsDraw;
    }

    public float Strength { private set; get; } 
    public Vector3 SkinnyPouringCenter { private set; get; }
    public Bounds ScatterPouringCenter { private set; get; }

    public SandScatterPouring SandScatterPouring;
    public SandSkinnyPouring SandSkinnyPouring;

    public GameObject HandMesh;
    public Renderer HandRenderer;
    public Transform IndexTipTransform;
    public Transform PalmTransform;
    public List<Transform> handJoints = new List<Transform>(new Transform[(int)HandJoint.JointMax]);

    private Vector3 _lastPalmPosition;
    private Vector3 _lastIndexFingerTipPosition;

    private HandJointLocations _handJointLocations = new HandJointLocations();

    private Dictionary<HandPose, Action> _onHandPoseStart = new Dictionary<HandPose, Action>();
    private Dictionary<HandPose, Action> _onHandPoseUpdate = new Dictionary<HandPose, Action>();
    private Dictionary<HandPose, Action> _onHandPoseEnd = new Dictionary<HandPose, Action>();


    private void Awake()
    {
        _fsm = AbstractHierarchicalFiniteStateMachine.CreateRootStateMachine<HandFSM>("HandStateMachine");
    }

    private void Start()
    {
        IsValid = false; ;
        HandMesh.SetActive(false);
        _fsm.SetHand(this);
        _fsm.OnEnter();
        CurrentHandPose = HandPose.None;
    }

   

    private void Update()
    {
        _text.text = _fsm.GetCurrentHierarchicalStatesNamesString();
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
        PalmVelocity = (PalmTransform.position - _lastPalmPosition) / Time.deltaTime;
        IndexFingerTipVelocity = (IndexTipTransform.position - _lastIndexFingerTipPosition) / Time.deltaTime;
        _lastPalmPosition = PalmTransform.position;
        _lastIndexFingerTipPosition = IndexTipTransform.position;

        _fsm.OnUpdate();
    }

    private void UpdateHandJoints()
    {
        if (PXR_HandTracking.GetJointLocations(handType, ref _handJointLocations))
        {
            if (_handJointLocations.isActive == 0)
            {
                SetIsValid(false);
                return;
            };

            SetIsValid(true);
            UpdateAimState();
            transform.localScale = Vector3.one * _handJointLocations.handScale;

            for (int i = 0; i < handJoints.Count; ++i)
            {
                if (handJoints[i] == null) continue;

                if (i == (int)HandJoint.JointWrist)
                {
                    handJoints[i].localPosition = _handJointLocations.jointLocations[i].pose.Position.ToVector3();
                    handJoints[i].localRotation = _handJointLocations.jointLocations[i].pose.Orientation.ToQuat();
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
                        parentPose = new UnityEngine.Pose(_handJointLocations.jointLocations[1].pose.Position.ToVector3(), _handJointLocations.jointLocations[1].pose.Orientation.ToQuat());
                    }
                    else
                    {
                        parentPose = new UnityEngine.Pose(_handJointLocations.jointLocations[i - 1].pose.Position.ToVector3(), _handJointLocations.jointLocations[i - 1].pose.Orientation.ToQuat());
                    }

                    var inverseParentRotation = Quaternion.Inverse(parentPose.rotation);
                    handJoints[i].localRotation = inverseParentRotation * _handJointLocations.jointLocations[i].pose.Orientation.ToQuat();
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
            if (_onHandPoseEnd.ContainsKey(CurrentHandPose))
            {
                _onHandPoseEnd[CurrentHandPose]?.Invoke();
            }
            HandlePoseEnd(CurrentHandPose);
            CurrentHandPose = handPose;
            HandlePoseStart(CurrentHandPose);
            if (_onHandPoseStart.ContainsKey(CurrentHandPose))
            {
                _onHandPoseStart[CurrentHandPose]?.Invoke();
            }
        }
    }


    private void HandlePoseStart(HandPose state)
    {

    }

    private void HandlePoseEnd(HandPose state)
    {

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
        if (!_onHandPoseStart.ContainsKey(handPose))
        {
            _onHandPoseStart[handPose] = callback;
        }
        else
        {
            _onHandPoseStart[handPose] += callback;
        }
    }

    public void BindHandPoseEndCallback(HandPose handPose, Action callback)
    {
        if (!_onHandPoseEnd.ContainsKey(handPose))
        {
            _onHandPoseEnd[handPose] = callback;
        }
        else
        {
            _onHandPoseEnd[handPose] += callback;
        }
    }

    public void BindHandPoseUpdateCallback(HandPose handPose, Action callback)
    {
        if (!_onHandPoseUpdate.ContainsKey(handPose))
        {
            _onHandPoseUpdate[handPose] = callback;
        }
        else
        {
            _onHandPoseUpdate[handPose] += callback;
        }
    }

    

    private void UpdateAimState()
    {
        Strength = Vector3.Distance(_handJointLocations.jointLocations[5].pose.Position.ToVector3() ,_handJointLocations.jointLocations[8].pose.Position.ToVector3());
    }


}