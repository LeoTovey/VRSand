using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using Unity.VisualScripting;

[CustomEditor(typeof(GenerateCube))]
public class GenerateCubeEditor : Editor
{
    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();

        GenerateCube generator = (GenerateCube)target;

        if (GUILayout.Button("Generate Cylinders from HeightMap"))
        {
            GenerateCylinders(generator);
        }
    }
    void ApplyColorToCylinder(GameObject cylinder, Color color)
    {
        Renderer renderer = cylinder.GetComponent<Renderer>();
        if (renderer != null)
        {
            Material material = new Material(renderer.sharedMaterial); // �����µĲ���ʵ��
            material.color = color; // ������ɫ

            renderer.material = material; // Ӧ���µĲ���ʵ��
        }
    }

    void GenerateCylinders(GenerateCube generator)
    {
        //if (generator.heightMap == null)
        {
            //Debug.LogError("HeightMap texture not assigned!");
            //return;
        }

        int width = generator.HeightMapWidth;
        int height = generator.HeightMapHeight;

        float cubeWidth = generator.PlaneSize.x / width;
        float cubeHeight = generator.PlaneSize.y / height;


        GameObject parentObject = new GameObject("Cylinders"); // ����һ���ն�����Ϊ������

        float spacing = generator.spacing;
        float heightMultiplier = generator.heightMultiplier;

        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                //Color pixelColor = generator.heightMap.GetPixel(x, y);
                //float heightValue = pixelColor.grayscale * heightMultiplier;
                float heightValue = Mathf.PerlinNoise((float)x / width * 2f, (float)y / height * 2f);
                float u = (float)x / (float)width; // ���� UV ����
                float v = (float)y / (float)height;



                Color finalColor = Color.Lerp(generator.color1, generator.color2, u * v); // ���� UV ��������ɫ

                //float heightValue = 1.0f;
                Vector3 position = new Vector3(x * (spacing + cubeWidth), heightValue / 2, y * (spacing + cubeHeight));
                GameObject cylinder = PrefabUtility.InstantiatePrefab(generator.squareCylinderPrefab) as GameObject;
                cylinder.transform.position = position;
                cylinder.transform.localScale = new Vector3(cubeWidth, heightValue, cubeHeight);
                cylinder.transform.parent = parentObject.transform;
                ApplyColorToCylinder(cylinder, finalColor); // Ӧ����ɫ������

                Undo.RegisterCreatedObjectUndo(cylinder, "Create Cylinder");
            }
        }
    }
}
