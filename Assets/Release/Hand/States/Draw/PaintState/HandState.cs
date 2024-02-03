using KevinCastejon.HierarchicalFiniteStateMachine;
using UnityEngine;


public class HandPaintState : AbstractState
{
    public Hand Hand => GetStateMachine<PaintMainState>().Hand;

    public override void OnEnter()
    {

    }
    public override void OnExit()
    {

    }

    public override void OnUpdate()
    {
        HandPose CurrentHandPose = Hand.CurrentHandPose;

        if (CurrentHandPose == HandPose.HandUpward || CurrentHandPose == HandPose.ToolHolding || CurrentHandPose == HandPose.UIActivation || CurrentHandPose == HandPose.ScatterPouring || CurrentHandPose == HandPose.SkinnyPouring)
        {
            TransitionToState(EXIT);
        }

        switch (CurrentHandPose)
        {
            case HandPose.FingertipTracing:
                TransitionToState(PaintState.FINGERTIP_TRACING);
                break;
            case HandPose.FingerCarving:
                TransitionToState(PaintState.FINGER_CARVING);
                break;
            default:
                break;
        }

    }
}
