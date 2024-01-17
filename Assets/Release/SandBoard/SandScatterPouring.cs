using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SandScatterPouring : MonoBehaviour, ISandPouring
{
    public Vector3 PouringCenter => _scatterCenter;

    public float PouringVelocity => _scatterSandVelocity;

    public ParticleSystem SandEffect => _scatterSand;

    public float Strenth => _strenth;

    public bool Enabled => _enabled;

    [SerializeField] private Hand _hand;
    [SerializeField] private float _maxStrength = 0.06f;
    [SerializeField] private float _minStrength = 0.02f;
    [SerializeField] private float _scatterSandVelocity = 0.04f;
    [SerializeField] private ParticleSystem _scatterSand;
    [SerializeField] private SandColorController _sandColorController;


    private float _strenth;
    private Vector3 _scatterCenter;
    private bool _enabled;
    private Color _startColor;


    private void Awake()
    {
        _scatterSand.gameObject.SetActive(false);

        _hand.BindHandPoseEndCallback(HandPose.ScatterPouring, () =>
        {
            _scatterSand.gameObject.SetActive(false);
        });

        _hand.BindHandPoseStartCallback(HandPose.ScatterPouring, () =>
        {
            _scatterSand.gameObject.SetActive(true);
        });
    }

    private void Update()
    {
        if (_enabled)
        {
            _strenth = Mathf.InverseLerp(_minStrength, _maxStrength, _hand.Strength);
            _startColor = _sandColorController.SandColor;

            if (_strenth > 0.0f)
            {
                _scatterSand.gameObject.SetActive(true);

                _scatterCenter = _hand.PalmTransform.position;
                transform.position = _scatterCenter;
                transform.rotation = _hand.PalmTransform.rotation;
                _startColor.a = _strenth * 0.2f;

                var main = _scatterSand.main;
                main.startColor = _startColor;
            }
            else
            {
                _scatterSand.gameObject.SetActive(false);
            }

        }
    }
}