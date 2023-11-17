using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SandBoardController : MonoBehaviour
{
    public GameObject Table;
    private float lastLeftHandHeight;
    public HandController Hands;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        float currentHeight = Hands.RightHand.PalmTransform.position.y;
        float deltaHeight = currentHeight - lastLeftHandHeight;
        lastLeftHandHeight = currentHeight;

        if (Hands.RightHand.CurrentHandPose == HandPose.HandUpward && Hands.LeftHand.CurrentHandPose != HandPose.HandUpward)
        {
            Transform tableTransform = Table.transform;
            Vector3 currentTablePosition = tableTransform.position;
            currentTablePosition.y += deltaHeight;
            tableTransform.position = currentTablePosition;
        }
    }
}
