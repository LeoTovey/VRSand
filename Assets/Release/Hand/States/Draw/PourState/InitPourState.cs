using KevinCastejon.HierarchicalFiniteStateMachine;

using UnityEngine;

public class InitPourState : AbstractState
{
    public Hand Hand => GetStateMachine<PourMainState>().Hand;

    public override void OnEnter()
    {
    }

    public override void OnExit()
    {
    }

    public override void OnUpdate()
    {
        HandPose CurrentHandPose = Hand.CurrentHandPose;

        switch (CurrentHandPose)
        {
            case HandPose.SkinnyPouring:
                TransitionToState(PourState.SKINNY_POUR);
                break;
            case HandPose.ScatterPouring:
                TransitionToState(PourState.SCATTER_POUR);
                break;
            default:
                TransitionToState(EXIT);
                break;
        }
    }
}
