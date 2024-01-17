using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using KevinCastejon.HierarchicalFiniteStateMachine;
public class HandStateMachineComponent : MonoBehaviour
{
    private HandStateMachine _stateMachine;
    private void Awake()
    {
        _stateMachine = AbstractHierarchicalFiniteStateMachine.CreateRootStateMachine<HandStateMachine>("HandStateMachine");

        
    }
    private void Start()
    {
        _stateMachine.OnEnter();
    }
    private void Update()
    {
        _stateMachine.OnUpdate();
    }
}
