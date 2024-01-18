using KevinCastejon.HierarchicalFiniteStateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class ToolState : AbstractState
{

    public override void OnUpdate()
    {
        if (GetStateMachine<HandFSM>().Hand.CurrentHandPose == HandPose.ToolRemoving)
        {
            TransitionToState(HandState.DRAW);
        }
    }
}

