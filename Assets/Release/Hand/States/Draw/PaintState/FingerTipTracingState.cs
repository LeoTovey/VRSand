using KevinCastejon.HierarchicalFiniteStateMachine;
using UnityEngine;


public class FingerTipTracingState : AbstractState
{
    public Hand Hand => GetStateMachine<PaintMainState>().Hand;



    public override void OnEnter()
    {
        Hand.PhysicsMesh.SetActive(false);
        Hand.FingertipTracing.SetActive(true);
    }
    public override void OnExit()
    {
        Hand.PhysicsMesh.SetActive(true);
        Hand.FingertipTracing.SetActive(false);
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
            case HandPose.FingertipTracing:
                break;
            case HandPose.FingerCarving:
                TransitionToState(PaintState.FINGER_CARVING);
                break;
            case HandPose.Fist:
                break;
            default:
                TransitionToState(PaintState.HAND);
                break;
        }
    }
}
