using KevinCastejon.HierarchicalFiniteStateMachine;

using UnityEngine;

public enum PourState
{
    SCATTER_POUR,
    SKINNY_POUR,
    INIT
}
public class PourMainState : AbstractHierarchicalFiniteStateMachine
{
    public Hand Hand => GetStateMachine<DrawMainState>().Hand;

    public PourMainState()
    {
        Init(PourState.INIT,
            Create<ScatterPourState, PourState>(PourState.SCATTER_POUR, this),
            Create<SkinnyPourState, PourState>(PourState.SKINNY_POUR, this),
            Create<InitPourState, PourState>(PourState.INIT, this)
        );
    }
}

