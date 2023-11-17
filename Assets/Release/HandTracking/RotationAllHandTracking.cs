using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Barracuda;
using UnityEngine;


public class RotationAllHandTracking: MonoBehaviour
{
    public NNModel ModelAsset;

    public Hand RightHand;
    public Hand LeftHand;
    public TextMeshProUGUI TestMesh;
    private Model runtimeModel;
    private IWorker worker;
    private int num_classes = 12;
    private int features = 26 * 4;
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
        catch (Exception e)
        {
            Debug.LogError(e);
        }

        return (HandPose)(max_idx + 1);

    }

    // Update is called once per frame
    void Update()
    {
        TestMesh.text += "\n";
        if (LeftHand.IsValid)
        {
            HandPose leftPose = RunModel(ref LeftHand);
            LeftHand.SetHandPose(leftPose);
            TestMesh.text += "BOTH : Left Hand: " + leftPose + "\n";
        }

        if (RightHand.IsValid)
        {
            HandPose rightPose = RunModel(ref RightHand);
            RightHand.SetHandPose(rightPose);
            TestMesh.text += "Both : Right Hand: " + rightPose + "\n";
        }

    }

}
