using KevinCastejon.HierarchicalFiniteStateMachine;

using UnityEngine;

public class SkinnyPourState : AbstractState
{

    public Hand Hand => GetStateMachine<PourMainState>().Hand;

    public override void OnEnter()
    {
        Hand.SandSkinnyPouring?.Enable();
    }
    public override void OnExit()
    {
        Hand.SandSkinnyPouring?.Disable();
    }

    public override void OnUpdate()
    {
        Hand.SandSkinnyPouring?.OnUpdate(Hand.Strength, Hand.SkinnyPouringCenter, Hand.SandColor);

        HandPose CurrentHandPose = Hand.CurrentHandPose;

        switch (CurrentHandPose)
        {
            case HandPose.SkinnyPouring:
                break;
            case HandPose.Fist:
                break;
            default:
                TransitionToState(EXIT);
                break;
        }
    }
}
