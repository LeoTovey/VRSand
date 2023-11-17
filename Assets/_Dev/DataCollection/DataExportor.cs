using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using TMPro;
using Unity.XR.PXR;
using UnityEngine;

public class DataExportor : MonoBehaviour
{
    private HandType handType = HandType.HandLeft;
    private String localDataFilePath;
    public Transform table;
    public List<Transform> leftHandJoints = new List<Transform>(new Transform[(int)HandJoint.JointMax]);
    public List<Transform> rightHandJoints = new List<Transform>(new Transform[(int)HandJoint.JointMax]);
    private StreamWriter localWriter;

    private int totalFrame = 0;
    private int frameCount = 0;

    private int currentHandPoseIndex = 0;
    private HandPose currentHandPose = HandPose.None;
    public TextMeshProUGUI TextMesh;

    void Start()
    {
        String LocalDataFileName = $"local_{handType}.csv";
        localDataFilePath = Path.Combine(Application.persistentDataPath, LocalDataFileName);

        // 检查文件是否已经存在并删除
        if (File.Exists(localDataFilePath))
        {
            try
            {
                File.Delete(localDataFilePath);
                Debug.Log("已删除现有文件：" + localDataFilePath);
            }
            catch (Exception e)
            {
                Debug.LogError("删除文件时发生错误：" + e.Message);
            }
        }

        try
        {
            Directory.CreateDirectory(Path.GetDirectoryName(localDataFilePath));
            localWriter = new StreamWriter(localDataFilePath, false);
            localWriter.Write("Frame,HandPoseType,HandType,PositionX,PositionY,PositionZ,");
            for (int i = 0; i < leftHandJoints.Count; i++)
            {
                localWriter.Write($"OrientationX_{i},OrientationY_{i},OrientationZ_{i},OrientationW_{i},");
            }
            localWriter.WriteLine();

            Debug.Log("导出到：" + localDataFilePath);
        }
        catch (Exception e)
        {
            Debug.LogError("导出文件时发生错误：" + e.Message);
        }





    }

    private void OnDestroy()
    {
        localWriter.Close();
    }

    // Update is called once per frame
    void Update()
    {
        frameCount++;
        if (frameCount == 2000)
        {
            frameCount = 0;
            currentHandPoseIndex = (currentHandPoseIndex + 1) % 13;
            currentHandPose = (HandPose)(currentHandPoseIndex);
        }
        else if (frameCount >= 500 && frameCount < 1500)
        {
            localWriter.Write($"{totalFrame}, {currentHandPoseIndex}, {handType},");
            if (handType == HandType.HandLeft)
            {
                Vector3 twistPosition = leftHandJoints[1].position - table.position;

                localWriter.Write($"{twistPosition.x}, {twistPosition.y}, {twistPosition.z},");
                for (int i = 0; i < leftHandJoints.Count; ++i)
                {
                    Transform jointTransform = leftHandJoints[i];
                    Quaternion localRotation = jointTransform.localRotation;
                    localWriter.Write($"{localRotation.x},{localRotation.y}, {localRotation.z},{localRotation.w},");

                }
                localWriter.WriteLine();
            }
            else if (handType == HandType.HandRight)
            {
                Vector3 twistPosition = rightHandJoints[1].position - table.position;
                localWriter.Write($"{twistPosition.x}, {twistPosition.y}, {twistPosition.z},");

                for (int i = 0; i < rightHandJoints.Count; ++i)
                {
                    Transform jointTransform = rightHandJoints[i];
                    Quaternion localRotation = jointTransform.localRotation;
                    localWriter.Write($"{localRotation.x},{localRotation.y}, {localRotation.z},{localRotation.w},");

                }
                localWriter.WriteLine();
            }
            totalFrame++;
            TextMesh.text = "recording " + currentHandPose + frameCount;

        }
        else if (frameCount < 500)
        {
            TextMesh.text = "prepare pose " + currentHandPose + frameCount;
        }
        else if (frameCount > 1500)
        {
            int nextHandPoseIndex = (currentHandPoseIndex + 1) % 13;
            TextMesh.text = "next pose is " + (HandPose)(nextHandPoseIndex) + frameCount;
        }
    }
}