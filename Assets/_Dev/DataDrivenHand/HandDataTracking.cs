using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class HandDataTracking : MonoBehaviour
{
    public Hand Hand;
    public string CSVFilePath;
    public Transform TableTransform;


    private StreamReader reader;

    void Start()
    {
        try
        {
            reader = new StreamReader(CSVFilePath);
            string line = reader.ReadLine();
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }

    void Update()
    {
        try
        {
            if (!reader.EndOfStream)
            {
                string line = reader.ReadLine();
                string[] values = line.Split(',');
                Vector3 wristPosition = new Vector3(float.Parse(values[1]), float.Parse(values[2]), float.Parse(values[3]));
                Hand.handJoints[1].position = wristPosition + TableTransform.position;

                int index = 4;
                for (int i = 0; i < 26; i++)
                {
                    Quaternion jointRotation = new Quaternion(
                        float.Parse(values[index++]),
                        float.Parse(values[index++]),
                        float.Parse(values[index++]),
                        float.Parse(values[index++])
                    );
                    Hand.handJoints[i].localRotation = jointRotation;
                }
            }
            else
            {
                reader.Close();
            }
        }
        catch (Exception e)
        {
            Debug.LogException(e);
        }
    }
}
