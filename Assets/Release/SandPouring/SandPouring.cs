using Modularify.LoadingBars3D;
using UnityEngine;


public class SandPouring
{
    public virtual float PouringVelocity { get; }
    public virtual float Alpha { get; }
 

    public float[] SandAmount => _sandAmount;
    public float Strength => _strength;
    public Vector3 PouringCenter => _pouringCenter;
    
    public bool EnablePouring = true;
    
    protected float[] _sandAmount = new float[4];

    protected ParticleSystem _sandEffect;
    protected LoadingBarSegments _loadingSegments;

    protected bool _enabled;
    protected float _strength;
    protected Vector3 _pouringCenter;
    

    public SandPouring(ParticleSystem sandEffect, LoadingBarSegments loadingSegments)
    {
        _enabled = false;
        _sandEffect = sandEffect;
        _loadingSegments = loadingSegments;
        _strength = 0.0f;
    }


    public void Enable()
    {
        _sandEffect.gameObject.SetActive(true);
        _loadingSegments.gameObject.SetActive(true);
        _enabled = true;
    }

    public void Disable()
    {
        _sandEffect.gameObject.SetActive(false);
        _loadingSegments.gameObject.SetActive(false);
        _enabled = false;
    }

    public void OnStart()
    {
        Disable();
    }

    public void OnUpdate(float strength, Vector3 pouringCenter, Color sandColor)
    {
        if (_enabled)
        {
            _strength = Mathf.Clamp(strength, 0.0f, 1.0f);
            _loadingSegments.SetPercentage(_strength);
            _loadingSegments.transform.position = pouringCenter;
            if (_strength > 0.0f && EnablePouring)
            {
                _sandEffect.gameObject.SetActive(true);
                _pouringCenter = pouringCenter;
                _sandEffect.gameObject.transform.position = pouringCenter;
                Color color = sandColor;
                color.a = _strength * Alpha;
                _sandAmount[3] = _strength * PouringVelocity;
                _sandAmount[0] = color.r;
                _sandAmount[1] = color.g;
                _sandAmount[2] = color.b;

                OnPouringUpdate(sandColor);
            }
            else
            {
                _sandEffect.gameObject.SetActive(false);
            }
        }
    }

    public virtual void OnPouringUpdate(Color sandColor)
    {

    }

    public void UpdateProgressor(float value)
    {

    }

}
