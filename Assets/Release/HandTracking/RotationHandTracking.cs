using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using Unity.Barracuda;
using UnityEngine;


public class RotationHandTracking : MonoBehaviour
{
    public NNModel LeftModelAsset;
    public NNModel RightModelAsset;
    public Hand RightHand;
    public Hand LeftHand;
    public TextMeshProUGUI TestMesh;
    private Model runtimeLeftModel;
    private Model runtimeRightModel;
    private IWorker leftWorker, rightWorker;
    private int num_classes = 12;
    private int features = 26 * 4;
    private Tensor leftInput, leftOutput;
    private Tensor rightInput, rightOutput;

    // Start is called before the first frame update
    void Start()
    {
        runtimeLeftModel = ModelLoader.Load(LeftModelAsset);
        leftWorker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, runtimeLeftModel);
        runtimeRightModel = ModelLoader.Load(RightModelAsset);
        rightWorker = WorkerFactory.CreateWorker(WorkerFactory.Type.CSharpBurst, runtimeRightModel);

        leftInput = new Tensor(1, 1, 1, features);
        leftOutput = new Tensor(1, 1, 1, num_classes);
        rightInput = new Tensor(1, 1, 1, features);
        rightOutput = new Tensor(1, 1, 1, num_classes);
    }

    public void OnDestroy()
    {
        leftWorker?.Dispose();
        rightWorker?.Dispose();
        leftInput?.Dispose();
        leftOutput?.Dispose();
        rightInput?.Dispose();
        rightOutput?.Dispose();
    }


    public HandPose RunLeftModel(ref Hand hand)
    {
        int index = 0;

        for (int i = 0; i < hand.handJoints.Count; ++i)
        {
            Transform jointTransform = hand.handJoints[i];
            Quaternion localRotation = jointTransform.localRotation;
            leftInput[0, 0, 0, index++] = localRotation[0];
            leftInput[0, 0, 0, index++] = localRotation[1];
            leftInput[0, 0, 0, index++] = localRotation[2];
            leftInput[0, 0, 0, index++] = localRotation[3];
        }

        int max_idx = 0;
        try
        {
            leftWorker.Execute(leftInput);
            leftOutput = leftWorker.PeekOutput();
            float max_date = leftOutput[0, 0, 0, 0];
            Debug.Log(leftOutput);
            for (int i = 0; i < num_classes; i++)
            {
                if (leftOutput[0, 0, 0, i] > max_date)
                {
                    max_date = leftOutput[0, 0, 0, i];
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
    public HandPose RunRightModel(ref Hand hand)
    {
        int index = 0;

        for (int i = 0; i < hand.handJoints.Count; ++i)
        {
            Transform jointTransform = hand.handJoints[i];
            Quaternion localRotation = jointTransform.localRotation;
            rightInput[0, 0, 0, index++] = localRotation[0];
            rightInput[0, 0, 0, index++] = localRotation[1];
            rightInput[0, 0, 0, index++] = localRotation[2];
            rightInput[0, 0, 0, index++] = localRotation[3];
        }

        int max_idx = 0;
        try
        {
            rightWorker.Execute(rightInput);
            rightOutput = rightWorker.PeekOutput();
            float max_date = rightOutput[0, 0, 0, 0];
            Debug.Log(rightOutput);
            for (int i = 0; i < num_classes; i++)
            {
                if (rightOutput[0, 0, 0, i] > max_date)
                {
                    max_date = rightOutput[0, 0, 0, i];
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
        TestMesh.text =
            "LeftHand Is Valid: " + LeftHand.IsValid + " " + LeftHand.Strength + "\n" +
            "RightHand Is Valid: " + RightHand.IsValid + " " + RightHand.Strength + "\n";

        if (LeftHand.IsValid)
        {
            HandPose leftPose = RunLeftModel(ref LeftHand);
            LeftHand.SetHandPose(leftPose);
            TestMesh.text += "Left Hand: " + leftPose + "\n";
        }

        if (RightHand.IsValid)
        {
            HandPose rightPose = RunRightModel(ref RightHand);
            RightHand.SetHandPose(rightPose);
            TestMesh.text += "Right Hand: " + rightPose + "\n";
        }

        TestMesh.text += "scale" + RightHand.transform.localScale;
    }

}
