using KevinCastejon.HierarchicalFiniteStateMachine;
public class HandStateMachine : AbstractHierarchicalFiniteStateMachine
{
    public enum HandState
    {
        DRAW,
        UI,
        HEIGHT,
        INVALID,
        TOOLS
    }
    public HandStateMachine()
    {
        Init(HandState.DRAW,
            Create<DrawStateMachine, HandState>(HandState.DRAW, this),
            Create<UiState, HandState>(HandState.UI, this),
            Create<HeightState, HandState>(HandState.HEIGHT, this),
            Create<InvalidState, HandState>(HandState.INVALID, this),
            Create<ToolsState, HandState>(HandState.TOOLS, this)
        );
    }
    public override void OnExitFromSubStateMachine(AbstractHierarchicalFiniteStateMachine subStateMachine)
    {
    }
    public override void OnStateMachineEntry()
    {
    }
    public override void OnStateMachineExit()
    {
    }
    public class UiState : AbstractState
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
    public class HeightState : AbstractState
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
    public class InvalidState : AbstractState
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
    public class ToolsState : AbstractState
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
