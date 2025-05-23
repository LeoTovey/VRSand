using Modularify.LoadingBars3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;
using UnityEngine.XR.Interaction.Toolkit.AffordanceSystem.Jobs;

public class SandScatterPouring : SandPouring
{


    public override float PouringVelocity => _scatterSandVelocity;
    public override float Alpha => _alpha;


    private float _scatterSandVelocity = 0.04f;
    private float _alpha = 0.2f;

    public SandScatterPouring(ParticleSystem sandEffect, LoadingBarSegments loadingSegments) : base(sandEffect, loadingSegments)
    {

    }

    public override void OnPouringUpdate(Color sandColor)
    {
        var main = _sandEffect.main;
        sandColor.a = _strength * _alpha;
        main.startColor = sandColor;

        _loadingSegments.transform.position = _pouringCenter + new Vector3(0.0f, 0.08f, 0);
        if (Camera.main != null)
        {
            Vector3 cameraPos = Camera.main.transform.position;
            _loadingSegments.transform.LookAt(cameraPos);
        }
    }

}