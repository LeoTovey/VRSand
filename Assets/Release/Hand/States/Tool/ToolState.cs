using KevinCastejon.HierarchicalFiniteStateMachine;
using Modularify.LoadingBars3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
public enum ToolLoadState
{
    PEN_LOADING,
    PEN_DRAWING,
}

public class PenLoading : AbstractState
{

    public Hand Hand => GetStateMachine<ToolState>().Hand;
    
    public override void OnEnter()
    {
        Hand.CurrentLoadingTime = 0.0f;
        Hand.PenLoading.gameObject.SetActive(true);
        Hand.PenLoading.transform.position = Hand.IndexTipTransform.position;
    }

    public override void OnUpdate()
    {
        HandPose CurrentHandPose = Hand.CurrentHandPose;

        if (CurrentHandPose != HandPose.ToolHolding)
        {
            TransitionToState(EXIT);
        }

        Hand.CurrentLoadingTime += Time.deltaTime;

        Hand.PenLoading.SetPercentage(Hand.CurrentLoadingTime / Hand.MaxLoadingTime);
        Hand.PenLoading.transform.position = Hand.IndexTipTransform.position;
        if (Hand.CurrentLoadingTime > Hand.MaxLoadingTime)
        {
            TransitionToState(ToolLoadState.PEN_DRAWING);
        }
    }

    public override void OnExit()
    {
        Hand.CurrentLoadingTime = 0.0f;
        Hand.PenLoading.gameObject.SetActive(false);
    }
}

public class PenDrawing : AbstractState
{
    public Hand Hand => GetStateMachine<ToolState>().Hand;

    public override void OnEnter()
    {
        Hand.Pen.gameObject.SetActive(true);
        Hand.PenLoading.gameObject.SetActive(true);
        Hand.HandRenderer.enabled = false;
        Hand.CurrentLoadingTime = 0.0f;
    }

    public override void OnUpdate()
    {
        HandPose CurrentHandPose = Hand.CurrentHandPose;

        if (CurrentHandPose == HandPose.ToolRemoving)
        {
            Hand.CurrentLoadingTime += Time.deltaTime;
            Hand.PenLoading.SetPercentage(Hand.CurrentLoadingTime / Hand.MaxLoadingTime);
            Hand.PenLoading.transform.position = Hand.IndexTipTransform.position;
            if (Hand.CurrentLoadingTime > Hand.MaxLoadingTime)
            {
                TransitionToState(EXIT);
            }
        }
        else
        {
            Hand.PenLoading.SetPercentage(0.0f);
            Hand.CurrentLoadingTime = 0.0f;
            Hand.Pen.SetVelocity(Hand.PalmVelocity);
            Hand.Pen.transform.position = Hand.PalmTransform.position;
        }

    }

    public override void OnExit()
    {
        Hand.Pen.gameObject.SetActive(false);
        Hand.HandRenderer.enabled = true;
        Hand.PenLoading.SetPercentage(0.0f);
        Hand.PenLoading.gameObject.SetActive(false);
    }
}


public class ToolState : AbstractHierarchicalFiniteStateMachine
{
    public Hand Hand => GetStateMachine<HandFSM>().Hand;
    

    public ToolState()
    {
        Init(
            ToolLoadState.PEN_LOADING,
            Create<PenLoading, ToolLoadState>(ToolLoadState.PEN_LOADING, this),
            Create<PenDrawing, ToolLoadState>(ToolLoadState.PEN_DRAWING, this)
            );
    }
}

