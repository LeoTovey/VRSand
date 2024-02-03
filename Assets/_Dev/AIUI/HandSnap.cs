using System.Collections;
using System.Collections.Generic;
using System.Xml;
using Unity.XR.PXR;
using UnityEngine;
using System.IO;
using System;
using UnityEngine.Rendering.Universal;
using UnityEngine.PlayerLoop;
using UnityEngine.UIElements;


[Serializable]
public class TransformData
{
    public Vector3 position;
    public Quaternion rotation;
    public Vector3 scale;
}


[Serializable]
public class Serialization<T>
{
    [SerializeField]
    List<T> target;
    public List<T> ToList() { return target; }

    public Serialization(List<T> target)
    {
        this.target = target;
    }
}



public class HandSnap : MonoBehaviour
{
    public List<Transform> handJoints = new List<Transform>(new Transform[(int)HandJoint.JointMax]);
    private String _localDataFilePath;
    [SerializeField] Hand _rightHand;
    [SerializeField] Hand _leftHand;
    [SerializeField] public List<TransformData> _handJoints = new List<TransformData>(new TransformData[(int)HandJoint.JointMax]);




    void Update()
    {
        if(_leftHand.CurrentHandPose == HandPose.UIActivation)
        {
            ExportTransformsToJson();
        }
    }
    void ExportTransformsToJson()
    {
        String LocalDataFileName = $"local_{_rightHand.CurrentHandPose}.json";
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
            Debug.Log(_handJoints.Count);
            for (int i = 0; i < _handJoints.Count; i++)
            {
                _handJoints[i].position = handJoints[i].localPosition;
                _handJoints[i].rotation = handJoints[i].localRotation;
                _handJoints[i].scale = handJoints[i].localScale;
            }

            string json = JsonUtility.ToJson(new Serialization<TransformData>(_handJoints), true);
            Debug.Log(json);
            File.WriteAllText(_localDataFilePath, json);
            Debug.Log("导出到：" + _localDataFilePath);
        }
        catch (Exception e)
        {
            Debug.LogError("导出文件时发生错误：" + e.Message);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
        
    }

    public void ImportTransformsFromJson(string jsonPath)
    {
        string json = File.ReadAllText(jsonPath);
        _handJoints = JsonUtility.FromJson<Serialization<TransformData>>(json).ToList();

        for (int i = 0; i < _handJoints.Count; i++)
        {
            TransformData transformData = _handJoints[i];
            handJoints[i].localRotation = transformData.rotation;
        }

        Debug.Log("Transforms imported from JSON successfully!");
    }


}
