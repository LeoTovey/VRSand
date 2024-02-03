using KevinCastejon.HierarchicalFiniteStateMachine;

using UnityEngine;



public class ScatterPourState : AbstractState
{
    public Hand Hand => GetStateMachine<PourMainState>().Hand;

    public override void OnEnter()
    {
        Hand.SandScatterPouring?.Enable();
    }

    public override void OnExit()
    {
        Hand.SandScatterPouring?.Disable();
    }

    public override void OnUpdate()
    {
        Hand.SandScatterPouring?.OnUpdate(Hand.Strength, Hand.PalmTransform.position, Hand.SandColor);
        HandPose CurrentHandPose = Hand.CurrentHandPose;

        switch (CurrentHandPose)
        {
            case HandPose.ScatterPouring:
                break;
            case HandPose.Fist:
                break;
            default:
                TransitionToState(EXIT);
                break;

        }
    }
}