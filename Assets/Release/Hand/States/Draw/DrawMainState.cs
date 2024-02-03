using KevinCastejon.HierarchicalFiniteStateMachine;

using UnityEngine;

public enum DrawState
{
    PAINT,
    POUR
}




public class DrawMainState : AbstractHierarchicalFiniteStateMachine
{
    public Hand Hand => GetStateMachine<HandFSM>().Hand;

    public DrawMainState()
    {
        Init(DrawState.PAINT,
            Create<PaintMainState, DrawState>(DrawState.PAINT, this),
            Create<PourMainState, DrawState>(DrawState.POUR, this)
        );
    }

    public override void OnStateMachineEntry()
    {
    }

    public override void OnStateMachineExit()
    {
    }

    public override void OnExitFromSubStateMachine(AbstractHierarchicalFiniteStateMachine subStateMachine)
    {
        HandPose CurrentHandPose = Hand.CurrentHandPose;
        if (CurrentHandPose == HandPose.HandUpward || CurrentHandPose == HandPose.ToolHolding || CurrentHandPose == HandPose.UIActivation)
        {
            TransitionToState(EXIT);
        }

        switch (CurrentHandPose)
        {
            case HandPose.ScatterPouring:
                TransitionToState(DrawState.POUR);
                break;
            case HandPose.SkinnyPouring:
                TransitionToState(DrawState.POUR);
                break;
            default:
                TransitionToState(DrawState.PAINT);
                break;
        }
    }
}










