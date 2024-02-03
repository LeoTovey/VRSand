using KevinCastejon.HierarchicalFiniteStateMachine;
using UnityEngine;

public class FingerCarvingState : AbstractState
{
    public Hand Hand => GetStateMachine<PaintMainState>().Hand;

    public override void OnEnter()
    {
        Hand.PhysicsMesh.SetActive(false);
        Hand.FingerCarving.SetActive(true);
    }
    public override void OnExit()
    {
        Hand.FingerCarving.SetActive(false);
        Hand.PhysicsMesh.SetActive(true);
    }

    public override void OnUpdate()
    {
        HandPose CurrentHandPose = Hand.CurrentHandPose;

        if (CurrentHandPose == HandPose.HandUpward || CurrentHandPose == HandPose.ToolHolding || CurrentHandPose == HandPose.UIActivation)
        {
            TransitionToState(EXIT);
        }

        switch (CurrentHandPose)
        {
            case HandPose.FingerCarving:
                break;
            case HandPose.FingertipTracing:
                TransitionToState(PaintState.FINGERTIP_TRACING);
                break;
            case HandPose.Fist:
                break;
            default:
                TransitionToState(PaintState.HAND);
                break;
        }

    }
}