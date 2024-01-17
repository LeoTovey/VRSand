using Modularify.LoadingBars3D;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SandProgressor : MonoBehaviour
{
    [SerializeField] private LoadingBarSegments _loadingSegments;
    [SerializeField] private Hand _hand;

    private bool _enable = false;

    public void Enable()
    {
        _enable = true;
        _loadingSegments.SetPercentage(0.0f);
        _loadingSegments.gameObject.SetActive(true);
    }

    public void Disable()
    {
        _enable = false;
        _loadingSegments.SetPercentage(0.0f);
        _loadingSegments.gameObject.SetActive(false);
    }

    public void OnValueUpdate(float value)
    {
        if (_enable)
        {
            _loadingSegments.SetPercentage(value);
        }
    }

    private void Update()
    {
        if (_enable)
        {
            _loadingSegments.transform.position = _hand.PalmTransform.position + new Vector3(0.0f, 0.08f, 0);
            Vector3 cameraPos = Camera.main.transform.position;
            _loadingSegments.transform.LookAt(cameraPos);
            //_loadingSegments.transform.Rotate(0, 180, 0);
        }
    }
}