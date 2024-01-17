using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SandSkinnyPouring : MonoBehaviour, ISandPouring
{
    public Vector3 PouringCenter => _skinnyCenter;

    public float PouringVelocity => _skinnySandVelocity;

    public ParticleSystem SandEffect => _skinnySand;

    public float Strenth => _strenth;

    public bool Enabled => _enabled;

    public float[] SandAmount => _sandAmount;


    [SerializeField] private Hand _hand;
    [SerializeField] private float _maxStrength = 0.06f;
    [SerializeField] private float _minStrength = 0.02f;
    [SerializeField] private float _skinnySandVelocity = 0.5f;
    [SerializeField] private ParticleSystem _skinnySand;
    [SerializeField] private SandColorController _sandColorController;
    [SerializeField] private SandProgressor _progressor;


    private float _strenth;
    private Vector3 _skinnyCenter;
    private Color _startColor;
    private bool _enabled;
    private float[] _sandAmount = new float[4];

    private void Awake()
    {
        _skinnySand.gameObject.SetActive(false);

        _hand.BindHandPoseEndCallback(HandPose.SkinnyPouring, () =>
        {
            if (_hand.CurrentHandStatus == HandStatus.Draw)
            {
                _skinnySand.gameObject.SetActive(false);
                _enabled = false;
                _progressor.Disable();
            }
        });

        _hand.BindHandPoseStartCallback(HandPose.SkinnyPouring, () =>
        {
            _skinnySand.gameObject.SetActive(true);
            _enabled = true;
            _progressor.Enable();
        });
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
                _skinnySand.gameObject.SetActive(true);
                
                _skinnyCenter = _hand.SkinnyPouringCenter;
                _skinnySand.gameObject.transform.position = _skinnyCenter;

                var main = _skinnySand.main;
                main.simulationSpeed = _strenth;
                Color color = _startColor;
                color.a = _strenth;
                main.startColor = color;

                _sandAmount[3] = (color.r + color.g + color.b) * _strenth * _skinnySandVelocity;
                _sandAmount[0] = color.r * _strenth * _skinnySandVelocity;
                _sandAmount[1] = color.g * _strenth * _skinnySandVelocity;
                _sandAmount[2] = color.b * _strenth * _skinnySandVelocity;

            }
            else
            {
                _skinnySand.gameObject.SetActive(false);
            }

        }
    }
}