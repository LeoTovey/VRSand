using Modularify.LoadingBars3D;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;
using Color = UnityEngine.Color;

public class SandScatterPouring : MonoBehaviour, ISandPouring
{
    public Vector3 PouringCenter => _scatterCenter;

    public float PouringVelocity => _scatterSandVelocity;

    public ParticleSystem SandEffect => _scatterSand;

    public float Strenth => _strenth;

    public bool Enabled => _enabled;

    public float[] SandAmount => _sandAmount;

    [SerializeField] private Hand _hand;
    [SerializeField] private float _maxStrength = 0.06f;
    [SerializeField] private float _minStrength = 0.02f;
    [SerializeField] private float _scatterSandVelocity = 0.04f;
    [SerializeField] private ParticleSystem _scatterSand;
    [SerializeField] private SandColorController _sandColorController;
    [SerializeField] private SandProgressor _progressor;


    private float _strenth;
    private Vector3 _scatterCenter;
    private Color _startColor;
    private bool _enabled;
    private float[] _sandAmount = new float[4];

    public void Enable()
    {
        _scatterSand.gameObject.SetActive(true);
        _enabled = true;
        _progressor.Enable();
    }

    public void Disable()
    {
        _scatterSand.gameObject.SetActive(false);
        _enabled = false;
        _progressor.Disable();
    }

    private void Awake()
    {
       
    }

    private void Start()
    {
        Disable();
    }

    private void Update()
    {

        if (_enabled)
        {
            _strenth = Mathf.InverseLerp(_minStrength, _maxStrength, _hand.Strength);
            _startColor = _sandColorController.SandColor;
            _progressor.OnValueUpdate(_strenth);
            if (_strenth > 0.0f)
            {
                _scatterSand.gameObject.SetActive(true);

                _scatterCenter = _hand.PalmTransform.position;
                _scatterSand.gameObject.transform.position = _scatterCenter;
                _scatterSand.gameObject.transform.rotation = _hand.PalmTransform.rotation;
                _startColor.a = _strenth * 0.2f;

                var main = _scatterSand.main;
                main.startColor = _startColor;
                Color color = _startColor;

                _sandAmount[3] = (color.r + color.g + color.b) * _strenth * _scatterSandVelocity;
                _sandAmount[0] = color.r * _strenth * _scatterSandVelocity;
                _sandAmount[1] = color.g * _strenth * _scatterSandVelocity;
                _sandAmount[2] = color.b * _strenth * _scatterSandVelocity;
            }
            else
            {
                _scatterSand.gameObject.SetActive(false);
            }

        }
    }

}