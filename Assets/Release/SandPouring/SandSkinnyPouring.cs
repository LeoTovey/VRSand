using Modularify.LoadingBars3D;
using UnityEngine;



public class SandSkinnyPouring : SandPouring
{

    public override float PouringVelocity => _skinnySandVelocity;
    public override float Alpha => _alpha;


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
        if (Camera.main != null)
        {
            Vector3 cameraPos = Camera.main.transform.position;
            _loadingSegments.transform.LookAt(cameraPos);
        }
    }

}