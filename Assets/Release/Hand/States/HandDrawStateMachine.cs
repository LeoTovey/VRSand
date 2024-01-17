using KevinCastejon.HierarchicalFiniteStateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class HandDrawStateMachine : AbstractHierarchicalFiniteStateMachine
{
    public enum HandPoseState
    {
        None = 0,
        HandUpward = 1,
        HandDownward = 2,
        SkinnyPouring = 3,
        ScatterPouring = 4,
        FingertipTracing = 5,
        FingerCarving = 6,
        HandSweeping = 7,
        PalmRubbing = 8,
        UIActivation = 9,
        ToolHolding = 10,
        ToolRemoving = 11,
        Fist = 12,
    }


}