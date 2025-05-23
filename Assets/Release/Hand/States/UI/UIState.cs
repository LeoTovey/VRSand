
using KevinCastejon.HierarchicalFiniteStateMachine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


public class UIState : AbstractState
{
    public override void OnUpdate()
    {
        if (GetStateMachine<HandFSM>().Hand.CurrentHandPose != HandPose.UIActivation)
        {
            TransitionToState(HandState.DRAW);
        }
    }
}
