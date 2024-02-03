using Modularify.LoadingBars3D;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class AiTip : MonoBehaviour
{

    public bool Finished => _currentTime > _maxTime;
    [SerializeField] private TextMeshProUGUI _text;
    [SerializeField] private LoadingBarStraight _barStraight;
    [SerializeField] private float _maxTime;
    [SerializeField] private bool _showProgress;

    private float _currentTime = 0.0f;

    private void Awake()
    {
        if (!_showProgress)
        {
            _barStraight.gameObject.SetActive(false);
        }

    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void SetTime(float time)
    {
        _currentTime = time;
        _barStraight.SetPercentage(time / _maxTime);
    }
}
