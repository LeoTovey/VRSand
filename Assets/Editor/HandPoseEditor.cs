using LitJson;
using System.Collections;
using System.Collections.Generic;
using Unity.XR.PXR;
using UnityEditor;
using UnityEngine;
using System.IO;
[CustomEditor(typeof(HandSnap))]
public class HandPoseEditor : Editor
{


    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        HandSnap generator = (HandSnap)target;

        if (GUILayout.Button("Load Hand Poses from JSON"))
        {
            LoadHandPosesFromJson(generator);
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

    void LoadHandPosesFromJson(HandSnap generator)
    {
        Debug.Log("1233");
        string path = EditorUtility.OpenFilePanel("Load Hand Poses", "", "json");
        if (!string.IsNullOrEmpty(path))
        {
            generator.ImportTransformsFromJson(path);
            Debug.Log("Hand Poses loaded from JSON successfully!");
        }
    }
}


[CustomEditor(typeof(AiTipHand))]
public class AiTipHandPoseEditor : Editor
{
    [SerializeField] public List<TransformData> _handJoints = new List<TransformData>(new TransformData[(int)HandJoint.JointMax]);

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        AiTipHand generator = (AiTipHand)target;
        if (GUILayout.Button("Load Hand Poses from JSON"))
        {
            LoadHandPosesFromJson(generator);
        }
    }

    void LoadHandPosesFromJson(AiTipHand generator)
    {
        string path = EditorUtility.OpenFilePanel("Load Hand Poses", "", "json");
        if (!string.IsNullOrEmpty(path))
        {
            string json = File.ReadAllText(path);
            _handJoints = JsonUtility.FromJson<Serialization<TransformData>>(json).ToList();
            for (int i = 0; i < _handJoints.Count; i++)
            {
                TransformData transformData = _handJoints[i];
                generator.HandJoints[i].localRotation = transformData.rotation;
            }
            Debug.Log("Hand Poses loaded from JSON successfully!");
        }
    }
}
