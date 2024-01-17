using KevinCastejon.HierarchicalFiniteStateMachine;



public class HandStateMachine : AbstractHierarchicalFiniteStateMachine
{
    public enum HandState
    {
        Draw,
        Tools,
        UI,
        HeightAdjusting,
    }


    public class ToolState : AbstractState
    {

    }

    public class UIState : AbstractState
    {

    }

    public class HeightState : AbstractState
    { 

    }
}