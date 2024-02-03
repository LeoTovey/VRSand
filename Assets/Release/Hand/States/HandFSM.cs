
using KevinCastejon.HierarchicalFiniteStateMachine;
using Michsky.MUIP;
using System.Buffers;

public enum HandState
{
    DRAW,
    UI,
    HEIGHT,
    TOOLS
}
public class HandFSM : AbstractHierarchicalFiniteStateMachine
{
    public Hand Hand => _hand;

    private Hand _hand;

    public HandFSM()
    {
        Init(HandState.DRAW,
            Create<DrawMainState, HandState>(HandState.DRAW, this),
            Create<UIState, HandState>(HandState.UI, this),
            Create<HeightState, HandState>(HandState.HEIGHT, this),
            Create<ToolState, HandState>(HandState.TOOLS, this)
        );
    }

    public void SetHand(Hand hand)
    {
        _hand = hand;
    }

    public override void OnExitFromSubStateMachine(AbstractHierarchicalFiniteStateMachine subStateMachine)
    {
        switch (Hand.CurrentHandPose)
        {
            case HandPose.HandUpward: 
                TransitionToState(HandState.HEIGHT);
                break;
            case HandPose.ToolHolding:
                TransitionToState(HandState.TOOLS);
                break;
            case HandPose.UIActivation:
                TransitionToState(HandState.UI);
                break;
            default:
                TransitionToState(HandState.DRAW);
                break;
        }
    }
     
}



