using Modularify.LoadingBars3D;
using UnityEngine;



public class SandSkinnyPouring : SandPouring
{
    public override float MaxStrength => _maxStrength;
    public override float MinStrength => _minStrength;
    public override float PouringVelocity => _skinnySandVelocity;
    public override float Alpha => _alpha;

    private float _maxStrength = 0.06f;
    private float _minStrength = 0.02f;
    private float _skinnySandVelocity = 0.5f;
    private float _alpha = 1.0f;

    public SandSkinnyPouring(ParticleSystem sandEffect, LoadingBarSegments loadingSegments) : base(sandEffect, loadingSegments)
    {

    }

    public override void OnPouringUpdate(Color sandColor)
    {
        var main = _sandEffect.main;
        sandColor.a = _strength;
        main.startColor = sandColor;
        main.simulationSpeed = _strength;

        _loadingSegments.transform.position = _pouringCenter + new Vector3(0.0f, 0.15f, 0);
        Vector3 cameraPos = Camera.main.transform.position;
        _loadingSegments.transform.LookAt(cameraPos);
    }

}