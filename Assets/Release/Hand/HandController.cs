using KevinCastejon.HierarchicalFiniteStateMachine;
using Modularify.LoadingBars3D;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class HandController : MonoBehaviour
{
    public Hand LeftHand;
    public Hand RightHand;

    private HandFSM _rightFSM;
    private HandFSM _leftFSM;
    public TextMeshProUGUI _leftText;
    public TextMeshProUGUI _rightText;

    [SerializeField] private ParticleSystem _sandScatter;
    [SerializeField] private ParticleSystem _sandSkinny;
    [SerializeField] private LoadingBarSegments _loadingSegments;
    [SerializeField] private SandColorController _sandColorController;

    private void Awake()
    {
        _rightFSM = AbstractHierarchicalFiniteStateMachine.CreateRootStateMachine<HandFSM>("RightHandStateMachine");
        _leftFSM = AbstractHierarchicalFiniteStateMachine.CreateRootStateMachine<HandFSM>("LeftHandStateMachine");
    }

    // Start is called before the first frame update
    void Start()
    {
        _rightFSM.SetHand(RightHand);
        _rightFSM.OnEnter();

        _leftFSM.SetHand(LeftHand);
        _leftFSM.OnEnter();

        RightHand.SandScatterPouring = new SandScatterPouring(_sandScatter, _loadingSegments);
        RightHand.SandSkinnyPouring = new SandSkinnyPouring(_sandSkinny, _loadingSegments);
    }

    // Update is called once per frame
    void Update()
    {
        _leftText.text = _leftFSM.GetCurrentHierarchicalStatesNamesString();
        _rightText.text = _rightFSM.GetCurrentHierarchicalStatesNamesString();
        _leftFSM.OnUpdate();
        _rightFSM.OnUpdate();
        LeftHand.CurrentHandState = _leftFSM.GetCurrentStateEnumValue<HandState>();
        RightHand.CurrentHandState = _rightFSM.GetCurrentStateEnumValue<HandState>();
        LeftHand.SandColor = _sandColorController.SandColor;
        RightHand.SandColor = _sandColorController.SandColor;
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
