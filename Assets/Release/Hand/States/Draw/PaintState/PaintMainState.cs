using KevinCastejon.HierarchicalFiniteStateMachine;

using UnityEngine;


public enum PaintState
{
    FINGERTIP_TRACING,
    FINGER_CARVING,
    HAND
}


public class PaintMainState : AbstractHierarchicalFiniteStateMachine
{
    public Hand Hand => GetStateMachine<DrawMainState>().Hand;

    public PaintMainState()
    {
        Init(PaintState.HAND,
            Create<FingerTipTracingState, PaintState>(PaintState.FINGERTIP_TRACING, this),
            Create<FingerCarvingState, PaintState>(PaintState.FINGER_CARVING, this),
            Create<HandPaintState, PaintState>(PaintState.HAND, this)
        );
    }
}



