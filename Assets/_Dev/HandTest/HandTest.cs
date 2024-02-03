using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Unity.XR.PXR;
using System.IO;
using System;
using UnityEngine.Rendering.Universal;
using static UnityEngine.Rendering.DebugUI;

public class HandTest : MonoBehaviour
{
   [SerializeField] public TextMeshProUGUI _textMesh;

    // 0 for left 1 for right
    private int _handType = 1;
    private StreamWriter _localWriter;
    private int _totalFrame = 0;
    private int _frameCount = 0;
    [SerializeField] private Hand[] _hands;
    private int _currentHandPoseIndex = 0;
    private HandPose _currentHandPose = HandPose.None;
    private String _localDataFilePath;

    void Start()
    {
        DateTime currentTime = DateTime.Now;
        string uniqueTimeString = currentTime.ToString("yyyyMMddHHmm");
        String LocalDataFileName = $"local_{uniqueTimeString}.csv";
        _localDataFilePath = Path.Combine(Application.persistentDataPath, LocalDataFileName);


        if (File.Exists(_localDataFilePath))
        {
            try
            {
                File.Delete(_localDataFilePath);
                Debug.Log("已删除现有文件：" + _localDataFilePath);
            }
            catch (Exception e)
            {
                Debug.LogError("删除文件时发生错误：" + e.Message);
            }
        }

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(_localDataFilePath));
            _localWriter = new StreamWriter(_localDataFilePath, false);

            _localWriter.Write("Frame,HandPoseType,HandType,Result");
            _localWriter.WriteLine();

            Debug.Log("导出到：" + _localDataFilePath);
        }
        catch (Exception e)
        {
            Debug.LogError("导出文件时发生错误：" + e.Message);
        }
    }

    private void OnDestroy()
    {
        _localWriter.Close();
    }

    private void Update()
    {
        _frameCount++;
        if (_frameCount == 2000)
        {
            _frameCount = 0;
            if (_currentHandPoseIndex == 12)
            {
                _handType = (_handType + 1) % 2;
            }
            _currentHandPoseIndex = (_currentHandPoseIndex + 1) % 13;
            _currentHandPose = (HandPose)(_currentHandPoseIndex);
        }
        else if (_frameCount >= 500 && _frameCount < 1500)
        {
            _localWriter.Write($"{_totalFrame}, {_currentHandPoseIndex}, {_handType}, {(int)_hands[_handType].CurrentHandPose}");
            _localWriter.WriteLine();
            _totalFrame++;
            _textMesh.text = "recording " + (HandType) _handType + _currentHandPose + _frameCount;

        }
        else if (_frameCount < 500)
        {
            _textMesh.text = "prepare pose " + (HandType)_handType + _currentHandPose + _frameCount;
        }
        else if (_frameCount > 1500)
        {
            int nextHandPoseIndex = (_currentHandPoseIndex + 1) % 13;
            _textMesh.text = "next pose is " + (HandType)_handType + (HandPose)(nextHandPoseIndex) + _frameCount;
        }
    }
}
