using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Barracuda;
using UnityEngine;


public class HandTrackingNetwork : MonoBehaviour
{
    public NNModel ModelAsset;
    public Hand RightHand;
    public Hand LeftHand;
    public Transform TableTransform;


    public TextMeshProUGUI TestMesh;


    private Model runtimeModel;
    private IWorker worker;
    private int num_classes = 13;
    private int features = 26 * 4 + 3;
    private Tensor input, output;

    // Start is called before the first frame update
    void Start()
    {
        runtimeModel = ModelLoader.Load(ModelAsset);
        worker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, runtimeModel);
        input = new Tensor(1, 1, 1, features);
        output = new Tensor(1, 1, 1, num_classes);
    }

    public void OnDestroy()
    {
        worker?.Dispose();
        input?.Dispose();
        output?.Dispose();
    }


    public HandPose RunModel(ref Hand hand)
    {
        int index = 0;
        Transform wristTransform = hand.handJoints[1];
        Vector3 localPosition = wristTransform.position - TableTransform.position;
        input[0, 0, 0, index++] = localPosition[0];
        input[0, 0, 0, index++] = localPosition[1];
        input[0, 0, 0, index++] = localPosition[2];

        for (int i = 0; i < hand.handJoints.Count; ++i)
        {
            Transform jointTransform = hand.handJoints[i];
            Quaternion localRotation = jointTransform.localRotation;
            input[0, 0, 0, index++] = localRotation[0];
            input[0, 0, 0, index++] = localRotation[1];
            input[0, 0, 0, index++] = localRotation[2];
            input[0, 0, 0, index++] = localRotation[3];
        }

        int max_idx = 0;
        try
        {
            worker.Execute(input);
            output = worker.PeekOutput();
            float max_date = output[0, 0, 0, 0];
            Debug.Log(output);
            for (int i = 0; i < num_classes; i++)
            {
                if (output[0, 0, 0, i] > max_date)
                {
                    max_date = output[0, 0, 0, i];
                    max_idx = i;
                }
            }
        }
        catch(Exception e)
        {
            Debug.LogError(e);
        }


        return (HandPose)(max_idx);

    }

    // Update is called once per frame
    void Update()
    {
        TestMesh.text = 
            "LeftHand Is Valid: " + LeftHand.IsValid +" " + LeftHand.Strength + "\n" + 
            "RightHand Is Valid: " + RightHand.IsValid + " " + RightHand.Strength+ "\n";

        if (LeftHand.IsValid)
        {
            HandPose leftPose = RunModel(ref LeftHand);
            LeftHand.SetHandPose(leftPose);
            TestMesh.text += "Left Hand: " + leftPose + "\n";
        }
 
        if (RightHand.IsValid)
        {
            HandPose rightPose = RunModel(ref RightHand);
            RightHand.SetHandPose(rightPose);
            TestMesh.text += "Right Hand: " + rightPose + "\n";
        }


    }

}

