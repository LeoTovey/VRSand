using KevinCastejon.HierarchicalFiniteStateMachine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
public enum DrawState
{
    SCATTER_POUR,
    SKINNY_POUR,
    PAINT
}

public class DrawMainState : AbstractHierarchicalFiniteStateMachine
{
    public Hand Hand => GetStateMachine<HandFSM>().Hand;

    public DrawMainState()
    {
        Init(DrawState.PAINT,
            Create<PaintState, DrawState>(DrawState.PAINT, this),
            Create<ScatterPourState, DrawState>(DrawState.SCATTER_POUR, this),
            Create<SkinnyPourState, DrawState>(DrawState.SKINNY_POUR, this)
        );
    }

    public override void OnStateMachineEntry()
    {
    }

    public override void OnStateMachineExit()
    {
    }
}




public class PaintState : AbstractState
{
    public override void OnUpdate()
    {

        Debug.WriteLine(GetStateMachine<DrawMainState>().Hand.CurrentHandPose);
        HandPose CurrentHandPose = GetStateMachine<DrawMainState>().Hand.CurrentHandPose;
        if (CurrentHandPose == HandPose.HandUpward || CurrentHandPose == HandPose.ToolHolding || CurrentHandPose == HandPose.UIActivation)
        {
            TransitionToState(EXIT);
        }
        else
        {
            switch (CurrentHandPose)
            {
                case HandPose.ScatterPouring:
                    TransitionToState(DrawState.SCATTER_POUR);
                    break;
                case HandPose.SkinnyPouring:
                    TransitionToState(DrawState.SKINNY_POUR);
                    break;
            }
        }
    }
}


public class SkinnyPourState : AbstractState
{
    public override void OnEnter()
    {

        GetStateMachine<DrawMainState>().Hand.SandSkinnyPouring?.Enable();
    }
    public override void OnExit()
    {
        GetStateMachine<DrawMainState>().Hand.SandSkinnyPouring?.Disable();
    }

    public override void OnUpdate()
    {
        HandPose CurrentHandPose = GetStateMachine<DrawMainState>().Hand.CurrentHandPose;
        if (CurrentHandPose == HandPose.HandUpward || CurrentHandPose == HandPose.ToolHolding || CurrentHandPose == HandPose.UIActivation)
        {
            TransitionToState(EXIT);
        }
        else
        {
            switch (CurrentHandPose)
            {
                case HandPose.ScatterPouring:
                    TransitionToState(DrawState.SCATTER_POUR);
                    break;
                case HandPose.SkinnyPouring:
                    break;
                default:
                    TransitionToState(DrawState.PAINT);
                    break;
            }
        }
    }
}

public class ScatterPourState : AbstractState
{
    public override void OnEnter()
    {
        GetStateMachine<DrawMainState>().Hand.SandScatterPouring?.Enable();
    }
    public override void OnExit()
    {
        GetStateMachine<DrawMainState>().Hand.SandScatterPouring?.Disable();
    }

    public override void OnUpdate()
    {
        HandPose CurrentHandPose = GetStateMachine<DrawMainState>().Hand.CurrentHandPose;
        if (CurrentHandPose == HandPose.HandUpward || CurrentHandPose == HandPose.ToolHolding || CurrentHandPose == HandPose.UIActivation)
        {
            TransitionToState(EXIT);
        }
        else
        {
            switch (CurrentHandPose)
            {
                case HandPose.SkinnyPouring:
                    TransitionToState(DrawState.SKINNY_POUR);
                    break;
                case HandPose.ScatterPouring:
                    break;
                default:
                    TransitionToState(DrawState.PAINT);
                    break;
            }
        }

    }
}
