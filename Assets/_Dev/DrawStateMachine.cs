using KevinCastejon.HierarchicalFiniteStateMachine;
public class DrawStateMachine : AbstractHierarchicalFiniteStateMachine
{
    public enum MyState
    {
        NONE,
        HAND_UPWARD,
        HAND_DOWNWARD,
        SKINNY__POURING,
        SCATTER_POURING,
        FINGERTIP_TRACING,
        FINGER_CARVING,
        HAND_SWEEPING,
        PALM_RUBBING,
        UI_ACTIVATION,
        TOOL_HOLDING,
        TOOL_REMOVING,
        FIST
    }
    public DrawStateMachine()
    {
        Init(MyState.NONE,
            Create<NoneState, MyState>(MyState.NONE, this),
            Create<HandUpwardState, MyState>(MyState.HAND_UPWARD, this),
            Create<HandDownwardState, MyState>(MyState.HAND_DOWNWARD, this),
            Create<SkinnyPouringState, MyState>(MyState.SKINNY__POURING, this),
            Create<ScatterPouringState, MyState>(MyState.SCATTER_POURING, this),
            Create<FingertipTracingState, MyState>(MyState.FINGERTIP_TRACING, this),
            Create<FingerCarvingState, MyState>(MyState.FINGER_CARVING, this),
            Create<HandSweepingState, MyState>(MyState.HAND_SWEEPING, this),
            Create<PalmRubbingState, MyState>(MyState.PALM_RUBBING, this),
            Create<UiActivationState, MyState>(MyState.UI_ACTIVATION, this),
            Create<ToolHoldingState, MyState>(MyState.TOOL_HOLDING, this),
            Create<ToolRemovingState, MyState>(MyState.TOOL_REMOVING, this),
            Create<FistState, MyState>(MyState.FIST, this)
        );
    }
    public override void OnStateMachineEntry()
    {
    }
    public override void OnStateMachineExit()
    {
    }
    public class NoneState : AbstractState
    {
        public override void OnEnter()
        {
        }
        public override void OnUpdate()
        {
        }
        public override void OnExit()
        {
        }
    }
    public class HandUpwardState : AbstractState
    {
        public override void OnEnter()
        {
        }
        public override void OnUpdate()
        {
        }
        public override void OnExit()
        {
        }
    }
    public class HandDownwardState : AbstractState
    {
        public override void OnEnter()
        {
        }
        public override void OnUpdate()
        {
        }
        public override void OnExit()
        {
        }
    }
    public class SkinnyPouringState : AbstractState
    {
        public override void OnEnter()
        {
        }
        public override void OnUpdate()
        {
        }
        public override void OnExit()
        {
        }
    }
    public class ScatterPouringState : AbstractState
    {
        public override void OnEnter()
        {
        }
        public override void OnUpdate()
        {
        }
        public override void OnExit()
        {
        }
    }
    public class FingertipTracingState : AbstractState
    {
        public override void OnEnter()
        {
        }
        public override void OnUpdate()
        {
        }
        public override void OnExit()
        {
        }
    }
    public class FingerCarvingState : AbstractState
    {
        public override void OnEnter()
        {
        }
        public override void OnUpdate()
        {
        }
        public override void OnExit()
        {
        }
    }
    public class HandSweepingState : AbstractState
    {
        public override void OnEnter()
        {
        }
        public override void OnUpdate()
        {
        }
        public override void OnExit()
        {
        }
    }
    public class PalmRubbingState : AbstractState
    {
        public override void OnEnter()
        {
        }
        public override void OnUpdate()
        {
        }
        public override void OnExit()
        {
        }
    }
    public class UiActivationState : AbstractState
    {
        public override void OnEnter()
        {
        }
        public override void OnUpdate()
        {
        }
        public override void OnExit()
        {
        }
    }
    public class ToolHoldingState : AbstractState
    {
        public override void OnEnter()
        {
        }
        public override void OnUpdate()
        {
        }
        public override void OnExit()
        {
        }
    }
    public class ToolRemovingState : AbstractState
    {
        public override void OnEnter()
        {
        }
        public override void OnUpdate()
        {
        }
        public override void OnExit()
        {
        }
    }
    public class FistState : AbstractState
    {
        public override void OnEnter()
        {
        }
        public override void OnUpdate()
        {
        }
        public override void OnExit()
        {
        }
    }
}
