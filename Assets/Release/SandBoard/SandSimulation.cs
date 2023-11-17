using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Mathematics;
using UnityEngine.UI;
using Unity.VisualScripting;
using System.IO;
using TMPro;
using System;
using static UnityEngine.EventSystems.EventTrigger;
using UnityEngine.InputSystem;

public class SandSimulation : MonoBehaviour
{

    public ComputeShader SandBoardComputeShader;

    public Hand[] Hands;
    public MeshRenderer Pen;
    public ToolsController tools;
    public TextMeshProUGUI testMesh;

    private int collisionMapWidth;
    private int collisionMapHeight;

    public RenderTexture HeightMap;
    public RenderTexture DisplacementHeightXMap;
    public RenderTexture DisplacementHeightYMap;
    public RenderTexture CollisionMap;
    public RenderTexture DisplacementHeightMap;

    public Texture2D InitTexture;  

    public RawImage ColorHandleImage;
    public Color SandColor;
    public ParticleSystem SkinnySand;
    public ParticleSystem ScatterSand;

    public Transform CollisionPlane;

    public int maxErosionRangeX = 10;
    public int maxErosionRangeY = 10;

    private int threadGroupsX;
    private int threadGroupsY;

    private int threadCountX = 8;
    private int threadCountY = 8;




    // for simulation
    public float InitHeight = 0.5f;
    public float MaxHeight = 1.0f;
    public Color InitColor;

    private float MaxStrength = 0.06f;
    private float MinStrength = 0.02f;
    private float SkinnySandVelocity = 0.5f;
    private float ScatterSandVelocity = 0.04f;
    private int SandRadius = 5;

    
    private int InitKernel = 0;
    private int CollisionTestKernel = 0;
    private int DisplacementVerticalKernel = 0;
    private int DisplacementHorizontalKernel = 0;
    private int DisplacementKernel = 0;
    private int ErosionKernel = 0;
    private int SkinnyPouringKernel = 0;
    private int ScatterPouringKernel = 0;

    void Start()
    {

        InitComputeShaderKernel();

        threadGroupsX = Mathf.CeilToInt(HeightMap.width / threadCountX);
        threadGroupsY = Mathf.CeilToInt(HeightMap.height / threadCountY);
        SandBoardComputeShader.Dispatch(0, threadGroupsX, threadGroupsY, 1);

        collisionMapWidth = CollisionMap.width;
        collisionMapHeight = CollisionMap.height;


        Hands[1].BindHandPoseEndCallback(HandPose.SkinnyPouring, () =>
        {
            SkinnySand.gameObject.SetActive(false);
        });

        Hands[1].BindHandPoseEndCallback(HandPose.ScatterPouring, () =>
        {
            ScatterSand.gameObject.SetActive(false);
        });
    }

    


    void Update()
    {
        
    }

    private void FixedUpdate()
    {
        /*
        if (Hands[0].CurrentHandPose == HandPose.SkinnyPouring && !test)
        {
            int width = HeightMap.width;
            int height = HeightMap.height;
            Texture2D texture2D = new Texture2D(width, height, TextureFormat.RGBAFloat, false);
            RenderTexture.active = HeightMap;
            texture2D.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            texture2D.Apply();
            byte[] bytes = texture2D.EncodeToPNG();
            File.WriteAllBytes(Path.Combine(Application.persistentDataPath, "SavedScreen.png"), bytes);
            test = true;
        }
        */

        if (Hands[1].CurrentHandPose == HandPose.SkinnyPouring && Hands[1].CurrentHandStatus == HandStatus.Draw)
        {
            float Strength = Hands[1].Strength;
            float SandStrength = Mathf.InverseLerp(MinStrength, MaxStrength, Strength);
            Vector3 skinnyPouringCenter = Hands[1].SkinnyPouringCenter;
            SkinnySand.gameObject.transform.position = skinnyPouringCenter;

            var main = SkinnySand.main;
            main.simulationSpeed = SandStrength;
            //Color color = ColorHandleImage.color;
            Color color = SandColor;
            color.a = SandStrength;
            main.startColor = color;

            if (SandStrength > 0.0f)
            {
                SkinnySand.gameObject.SetActive(true);

                Vector3 collisionPlaneScale = CollisionPlane.lossyScale;
                Vector3 collisionPlaneCenter = CollisionPlane.position;
                



                Bounds bounds = Hands[1].HandBound;
                float halfWidth = collisionPlaneScale.x * 0.5f;
                float halfHeight = collisionPlaneScale.y * 0.5f;

                float planeMinX = collisionPlaneCenter.x - halfWidth;
                float planeMinZ = collisionPlaneCenter.z - halfHeight;
                float planeMaxX = collisionPlaneCenter.x + halfWidth;
                float planeMaxZ = collisionPlaneCenter.z + halfHeight;

                bool isIntersectX = bounds.min.x < planeMaxX && bounds.max.x > planeMinX;
                bool isIntersectZ = bounds.min.z < planeMaxZ && bounds.max.z > planeMinZ;
                bool isIntersectY = bounds.min.y < collisionPlaneCenter.y && bounds.max.y > collisionPlaneCenter.y;

                if (isIntersectX && isIntersectZ)
                {
                    Vector3 localMin = bounds.min - collisionPlaneCenter;
                    Vector3 localMax = bounds.max - collisionPlaneCenter;
                    Vector3 localCenter = skinnyPouringCenter - collisionPlaneCenter;

                    int minX = Mathf.Clamp(Mathf.CeilToInt((localMin.x / collisionPlaneScale.x + 0.5f) * (collisionMapWidth - 1)) - 1, 0, collisionMapWidth - 1);
                    int maxX = Mathf.Clamp(Mathf.CeilToInt((localMax.x / collisionPlaneScale.x + 0.5f) * (collisionMapWidth - 1)) + 1, 0, collisionMapWidth - 1);
                    int minY = Mathf.Clamp(Mathf.CeilToInt((localMin.z / collisionPlaneScale.y + 0.5f) * (collisionMapHeight - 1)) - 1, 0, collisionMapHeight - 1);
                    int maxY = Mathf.Clamp(Mathf.CeilToInt((localMax.z / collisionPlaneScale.y + 0.5f) * (collisionMapHeight - 1)) + 1, 0, collisionMapHeight - 1);

                    int centerX = Mathf.Clamp(Mathf.CeilToInt((localCenter.x / collisionPlaneScale.x + 0.5f) * (collisionMapWidth - 1)) - 1, 0, collisionMapWidth - 1);
                    int centerY = Mathf.Clamp(Mathf.CeilToInt((localCenter.z / collisionPlaneScale.y + 0.5f) * (collisionMapHeight - 1)) - 1, 0, collisionMapHeight - 1);



                    float uvCenterX = Mathf.Clamp(skinnyPouringCenter.x / collisionPlaneScale.x + 0.5f, 0.0f, 1.0f);
                    float uvCenterY = Mathf.Clamp(skinnyPouringCenter.z / collisionPlaneScale.y + 0.5f, 0.0f, 1.0f);
                    float uvMinX = Mathf.Clamp(localMin.x / collisionPlaneScale.x + 0.5f, 0.0f, 1.0f);
                    float uvMaxX = Mathf.Clamp(localMax.x / collisionPlaneScale.x + 0.5f, 0.0f, 1.0f);
                    float uvMinY = Mathf.Clamp(localMin.z / collisionPlaneScale.y + 0.5f, 0.0f, 1.0f);
                    float uvMaxY = Mathf.Clamp(localMax.z / collisionPlaneScale.y + 0.5f, 0.0f, 1.0f);
                    float radius = Mathf.Min(maxY - minY, maxX - minX) * 0.5f;


                    Vector4 SandAmount = new Vector4(color.r, color.g, color.b, 0);
                    SandAmount.w = SandAmount.x + SandAmount.y + SandAmount.z;
                    SandAmount *= SandStrength * SkinnySandVelocity;


                    //testMesh.text = SandAmount + "";
                    //testMesh.text += "\n " + Mathf.InverseLerp(MinStrength, MaxStrength, Strength);

                    //SandAmount = SandAmount * Mathf.Lerp(0.0f, 1.0f, (Strength - MinStrength) / (MaxStrength - MinStrength));
                    SandBoardComputeShader.SetInts("StartId", new int[] { minX, minY });
                    SandBoardComputeShader.SetFloat("SandRadius", SandRadius);
                    SandBoardComputeShader.SetFloats("SandAmount", new float[] { SandAmount[0], SandAmount[1], SandAmount[2], SandAmount[3] });
                    //SandBoardComputeShader.SetFloats("SandCenter", new float[] { (uvMinX + uvMaxX) * 0.5f, (uvMinY + uvMaxY) * 0.5f });
                    SandBoardComputeShader.SetFloats("SandCenter", new float[] { centerX, centerY});


                    int threadGroupsX_Dev = Mathf.FloorToInt((maxX - minX - 1.0f) / threadCountX);
                    int threadGroupsY_Dev = Mathf.FloorToInt((maxY - minY - 1.0f) / threadCountY);
                    SandBoardComputeShader.Dispatch(SkinnyPouringKernel, threadGroupsX_Dev, threadGroupsY_Dev, 1);
                }
            }
            else
            {
                SkinnySand.gameObject.SetActive(false);
            }
        }
        else if(Hands[1].CurrentHandPose == HandPose.ScatterPouring && Hands[1].CurrentHandStatus == HandStatus.Draw)
        {
            float Strength = Hands[1].Strength;
            float SandStrength = Mathf.InverseLerp(MinStrength, MaxStrength, Strength);
            Vector3 scatterPouringCenter = Hands[1].PalmTransform.position;
            ScatterSand.gameObject.transform.position = scatterPouringCenter;
            ScatterSand.gameObject.transform.rotation = Hands[1].PalmTransform.rotation;
            var main = ScatterSand.main;
            //main.simulationSpeed = SandStrength;
            //Color color = ColorHandleImage.color;
            Color color = SandColor;
            color.a = SandStrength * 0.2f;


            main.startColor = color;
            if (SandStrength > 0.0f)
            {
                ScatterSand.gameObject.SetActive(true);
                Vector3 collisionPlaneScale = CollisionPlane.lossyScale;
                Vector3 collisionPlaneCenter = CollisionPlane.position;

                Bounds bounds = Hands[1].ScatterPouringCenter;

                float halfWidth = collisionPlaneScale.x * 0.5f;
                float halfHeight = collisionPlaneScale.y * 0.5f;

                float planeMinX = collisionPlaneCenter.x - halfWidth;
                float planeMinZ = collisionPlaneCenter.z - halfHeight;
                float planeMaxX = collisionPlaneCenter.x + halfWidth;
                float planeMaxZ = collisionPlaneCenter.z + halfHeight;

                bool isIntersectX = bounds.min.x < planeMaxX && bounds.max.x > planeMinX;
                bool isIntersectZ = bounds.min.z < planeMaxZ && bounds.max.z > planeMinZ;
                bool isIntersectY = bounds.min.y < collisionPlaneCenter.y && bounds.max.y > collisionPlaneCenter.y;

                if (isIntersectX && isIntersectZ)
                {
                    Vector3 localMin = bounds.min - collisionPlaneCenter;
                    Vector3 localMax = bounds.max - collisionPlaneCenter;


                    int minX = Mathf.Clamp(Mathf.CeilToInt((localMin.x / collisionPlaneScale.x + 0.5f) * (collisionMapWidth - 1)) - 1, 0, collisionMapWidth - 1);
                    int maxX = Mathf.Clamp(Mathf.CeilToInt((localMax.x / collisionPlaneScale.x + 0.5f) * (collisionMapWidth - 1)) + 1, 0, collisionMapWidth - 1);
                    int minY = Mathf.Clamp(Mathf.CeilToInt((localMin.z / collisionPlaneScale.y + 0.5f) * (collisionMapHeight - 1)) - 1, 0, collisionMapHeight - 1);
                    int maxY = Mathf.Clamp(Mathf.CeilToInt((localMax.z / collisionPlaneScale.y + 0.5f) * (collisionMapHeight - 1)) + 1, 0, collisionMapHeight - 1);


                    Vector4 SandAmount = new Vector4(color.r, color.g, color.b, 0);
                    SandAmount.w = SandAmount.x + SandAmount.y + SandAmount.z;
                    SandAmount *= SandStrength * ScatterSandVelocity;
                    SandBoardComputeShader.SetFloats("SandCenter", new float[] { 0.5f * minX + 0.5f * maxX, 0.5f * minY + 0.5f * maxY });
                    SandBoardComputeShader.SetFloat("SandRadius", 0.5f * (maxY - minY));
                    SandBoardComputeShader.SetInts("StartId", new int[] { minX, minY });
                    SandBoardComputeShader.SetFloats("SandAmount", new float[] { SandAmount[0], SandAmount[1], SandAmount[2], SandAmount[3] });
                    int threadGroupsX_Dev = Mathf.FloorToInt((maxX - minX - 1.0f) / threadCountX);
                    int threadGroupsY_Dev = Mathf.FloorToInt((maxY - minY - 1.0f) / threadCountY);
                    SandBoardComputeShader.Dispatch(ScatterPouringKernel, threadGroupsX_Dev, threadGroupsY_Dev, 1);
                }

            }
            else
            {
                ScatterSand.gameObject.SetActive(false);
            }
        }
        else if (Hands[1].CurrentHandStatus == HandStatus.Tools)
        {
            PenDisplacement();
        }
        else
        {
            Displacement();
        }
        //Collision();
        //SandBoardComputeShader.Dispatch(ErosionKernel, threadGroupsX, threadGroupsY, 1);
    }
    void PenDisplacement()
    {
        Vector3 collisionPlaneScale = CollisionPlane.lossyScale;
        Vector3 collisionPlaneCenter = CollisionPlane.position;
        float halfWidth = collisionPlaneScale.x * 0.5f;
        float halfHeight = collisionPlaneScale.y * 0.5f;
        float planeMinX = collisionPlaneCenter.x - halfWidth;
        float planeMinZ = collisionPlaneCenter.z - halfHeight;
        float planeMaxX = collisionPlaneCenter.x + halfWidth;
        float planeMaxZ = collisionPlaneCenter.z + halfHeight;

        int updateAreaMinX = collisionMapWidth - 1;
        int updateAreaMinY = collisionMapHeight - 1;
        int updateAreaMaxX = 0;
        int updateAreaMaxY = 0;

        bool isCollision = false;

        Bounds bounds = Pen.bounds;

        bool isIntersectX = bounds.min.x < planeMaxX && bounds.max.x > planeMinX;
        bool isIntersectZ = bounds.min.z < planeMaxZ && bounds.max.z > planeMinZ;
        bool isIntersectY = bounds.min.y < collisionPlaneCenter.y && bounds.max.y > collisionPlaneCenter.y;

        if (isIntersectX && isIntersectY && isIntersectZ)
        {
            isCollision = true;

            Vector3 localMin = bounds.min - collisionPlaneCenter;
            Vector3 localMax = bounds.max - collisionPlaneCenter;

            int minX = Mathf.Clamp(Mathf.CeilToInt((localMin.x / collisionPlaneScale.x + 0.5f) * (collisionMapWidth - 1)) - 1, 0, collisionMapWidth - 1);
            int maxX = Mathf.Clamp(Mathf.CeilToInt((localMax.x / collisionPlaneScale.x + 0.5f) * (collisionMapWidth - 1)) + 1, 0, collisionMapWidth - 1);
            int minY = Mathf.Clamp(Mathf.CeilToInt((localMin.z / collisionPlaneScale.y + 0.5f) * (collisionMapHeight - 1)) - 1, 0, collisionMapHeight - 1);
            int maxY = Mathf.Clamp(Mathf.CeilToInt((localMax.z / collisionPlaneScale.y + 0.5f) * (collisionMapHeight - 1)) + 1, 0, collisionMapHeight - 1);


            SandBoardComputeShader.SetInts("StartId", new int[] { minX, minY });


            updateAreaMinX = Mathf.Min(updateAreaMinX, minX);
            updateAreaMinY = Mathf.Min(updateAreaMinY, minY);
            updateAreaMaxX = Mathf.Max(updateAreaMaxX, maxX);
            updateAreaMaxY = Mathf.Max(updateAreaMaxY, maxY);

            Vector3 velocity = tools.Velocity;

            float absX = math.abs(velocity.x) + 0.0001f;
            float absY = math.abs(velocity.z) + 0.0001f;

            float HorizontalRatio = absX / (absX + absY);
            float VerticalRatio = absY / (absY + absX);

            SandBoardComputeShader.SetFloats("DisplacementRatio", new float[] { HorizontalRatio, VerticalRatio });

            int threadGroupsX_Dev = Mathf.FloorToInt((maxX - minX - 1.0f) / threadCountX);
            int threadGroupsY_Dev = Mathf.FloorToInt((maxY - minY - 1.0f) / threadCountY);
            SandBoardComputeShader.Dispatch(CollisionTestKernel, threadGroupsX_Dev, threadGroupsY_Dev, 1);

            int startX, endX, stepX;
            int startY, endY, stepY;

            if (velocity.x < 0)
            {
                startX = maxX - 1;
                endX = minX;
                stepX = -1;
            }
            else
            {
                startX = minX + 1;
                endX = maxX;
                stepX = 1;
            }

            if (velocity.z < 0)
            {
                startY = maxY - 1;
                endY = minY;
                stepY = -1;
            }
            else
            {
                startY = minY + 1;
                endY = maxY;
                stepY = 1;

            }


            int nextDisplacementRaw = 0;
            for (int i = startY; i != endY; i += stepY)
            {
                nextDisplacementRaw = Mathf.Min(collisionMapHeight - 1, Mathf.Max(i + stepY * maxErosionRangeY, 0));
                SandBoardComputeShader.SetInt("DisplacementRaw", i);
                SandBoardComputeShader.SetInt("NextDisplacementRaw", nextDisplacementRaw);
                SandBoardComputeShader.Dispatch(DisplacementVerticalKernel, threadGroupsX_Dev, 1, 1);
            }

            updateAreaMinY = Mathf.Min(updateAreaMinY, nextDisplacementRaw);
            updateAreaMaxY = Mathf.Max(updateAreaMaxY, nextDisplacementRaw);
            updateAreaMinY = Mathf.Min(updateAreaMinY, startY);
            updateAreaMaxY = Mathf.Max(updateAreaMaxY, startY);

            int nextDisplacementColumn = 0;
            for (int i = startX; i != endX; i += stepX)
            {
                nextDisplacementColumn = Mathf.Min(collisionMapWidth - 1, Mathf.Max(i + stepX * maxErosionRangeX, 0));
                SandBoardComputeShader.SetInt("DisplacementColumn", i);
                SandBoardComputeShader.SetInt("NextDisplacementColumn", nextDisplacementColumn);
                SandBoardComputeShader.Dispatch(DisplacementHorizontalKernel, 1, threadGroupsY_Dev, 1);
            }

            updateAreaMinX = Mathf.Min(updateAreaMinX, nextDisplacementColumn);
            updateAreaMaxX = Mathf.Max(updateAreaMaxX, nextDisplacementColumn);
            updateAreaMinX = Mathf.Min(updateAreaMinX, startX);
            updateAreaMaxX = Mathf.Max(updateAreaMaxX, startX);
        }


        if (isCollision)
        {

            SandBoardComputeShader.SetInts("StartId", new int[] { updateAreaMinX, updateAreaMinY });

            int threadGroupsX_Dev = Mathf.FloorToInt((updateAreaMaxX - updateAreaMinX - 1.0f) / threadCountX);
            int threadGroupsY_Dev = Mathf.FloorToInt((updateAreaMaxY - updateAreaMinY - 1.0f) / threadCountY);

            SandBoardComputeShader.Dispatch(DisplacementKernel, threadGroupsX_Dev, threadGroupsY_Dev, 1);
        }


    }

    void Displacement()
    {
        Vector3 collisionPlaneScale = CollisionPlane.lossyScale;
        Vector3 collisionPlaneCenter = CollisionPlane.position;
        float halfWidth = collisionPlaneScale.x * 0.5f;
        float halfHeight = collisionPlaneScale.y * 0.5f;
        float planeMinX = collisionPlaneCenter.x - halfWidth;
        float planeMinZ = collisionPlaneCenter.z - halfHeight;
        float planeMaxX = collisionPlaneCenter.x + halfWidth;
        float planeMaxZ = collisionPlaneCenter.z + halfHeight;

        int updateAreaMinX = collisionMapWidth - 1;
        int updateAreaMinY = collisionMapHeight - 1;
        int updateAreaMaxX = 0;
        int updateAreaMaxY = 0;

        bool isCollision = false;

        for (int c = 0; c < Hands.Length; c++)
        {
            Bounds bounds = Hands[c].HandBound;

            if (Hands[c].CurrentHandStatus == HandStatus.Draw)
            {
                bool isIntersectX = bounds.min.x < planeMaxX && bounds.max.x > planeMinX;
                bool isIntersectZ = bounds.min.z < planeMaxZ && bounds.max.z > planeMinZ;
                bool isIntersectY = bounds.min.y < collisionPlaneCenter.y && bounds.max.y > collisionPlaneCenter.y;

                if (isIntersectX && isIntersectY && isIntersectZ)
                {
                    isCollision = true;

                    Vector3 localMin = bounds.min - collisionPlaneCenter;
                    Vector3 localMax = bounds.max - collisionPlaneCenter;

                    int minX = Mathf.Clamp(Mathf.CeilToInt((localMin.x / collisionPlaneScale.x + 0.5f) * (collisionMapWidth - 1)) - 1, 0, collisionMapWidth - 1);
                    int maxX = Mathf.Clamp(Mathf.CeilToInt((localMax.x / collisionPlaneScale.x + 0.5f) * (collisionMapWidth - 1)) + 1, 0, collisionMapWidth - 1);
                    int minY = Mathf.Clamp(Mathf.CeilToInt((localMin.z / collisionPlaneScale.y + 0.5f) * (collisionMapHeight - 1)) - 1, 0, collisionMapHeight - 1);
                    int maxY = Mathf.Clamp(Mathf.CeilToInt((localMax.z / collisionPlaneScale.y + 0.5f) * (collisionMapHeight - 1)) + 1, 0, collisionMapHeight - 1);


                    SandBoardComputeShader.SetInts("StartId", new int[] {minX, minY});


                    updateAreaMinX = Mathf.Min(updateAreaMinX, minX);
                    updateAreaMinY = Mathf.Min(updateAreaMinY, minY);
                    updateAreaMaxX = Mathf.Max(updateAreaMaxX, maxX);
                    updateAreaMaxY = Mathf.Max(updateAreaMaxY, maxY);

                    Vector3 velocity = Hands[c].Velocity;

                    float absX = math.abs(velocity.x) + 0.0001f;
                    float absY = math.abs(velocity.z) + 0.0001f;

                    float HorizontalRatio = absX / (absX + absY);
                    float VerticalRatio = absY / (absY + absX);

                    SandBoardComputeShader.SetFloats("DisplacementRatio", new float[]{ HorizontalRatio, VerticalRatio});

                    int threadGroupsX_Dev = Mathf.FloorToInt((maxX - minX - 1.0f) / threadCountX);
                    int threadGroupsY_Dev = Mathf.FloorToInt((maxY - minY - 1.0f) / threadCountY);
                    SandBoardComputeShader.Dispatch(CollisionTestKernel, threadGroupsX_Dev, threadGroupsY_Dev, 1);

                    int startX, endX, stepX;
                    int startY, endY, stepY;

                    if (velocity.x < 0)
                    {
                        startX = maxX - 1;
                        endX = minX;
                        stepX = -1;
                    }
                    else
                    {
                        startX = minX + 1;
                        endX = maxX;
                        stepX = 1;
                    }

                    if (velocity.z < 0)
                    {
                        startY = maxY - 1;
                        endY = minY;
                        stepY = -1;
                    }
                    else
                    {
                        startY = minY + 1;
                        endY = maxY;
                        stepY = 1;

                    }


                    int nextDisplacementRaw = 0;
                    for (int i = startY; i != endY; i += stepY)
                    {
                        nextDisplacementRaw = Mathf.Min(collisionMapHeight - 1, Mathf.Max(i + stepY * maxErosionRangeY, 0));
                        SandBoardComputeShader.SetInt("DisplacementRaw", i);
                        SandBoardComputeShader.SetInt("NextDisplacementRaw", nextDisplacementRaw);
                        SandBoardComputeShader.Dispatch(DisplacementVerticalKernel, threadGroupsX_Dev, 1, 1);
                    }

                    updateAreaMinY = Mathf.Min(updateAreaMinY, nextDisplacementRaw);
                    updateAreaMaxY = Mathf.Max(updateAreaMaxY, nextDisplacementRaw);
                    updateAreaMinY = Mathf.Min(updateAreaMinY, startY);
                    updateAreaMaxY = Mathf.Max(updateAreaMaxY, startY);

                    int nextDisplacementColumn = 0;
                    for (int i = startX; i != endX; i += stepX)
                    {
                        nextDisplacementColumn = Mathf.Min(collisionMapWidth - 1, Mathf.Max(i + stepX * maxErosionRangeX, 0));
                        SandBoardComputeShader.SetInt("DisplacementColumn", i);
                        SandBoardComputeShader.SetInt("NextDisplacementColumn", nextDisplacementColumn);
                        SandBoardComputeShader.Dispatch(DisplacementHorizontalKernel, 1, threadGroupsY_Dev, 1);
                    }

                    updateAreaMinX = Mathf.Min(updateAreaMinX, nextDisplacementColumn);
                    updateAreaMaxX = Mathf.Max(updateAreaMaxX, nextDisplacementColumn);
                    updateAreaMinX = Mathf.Min(updateAreaMinX, startX);
                    updateAreaMaxX = Mathf.Max(updateAreaMaxX, startX);
                }
            }

        }

        if (isCollision)
        {

            SandBoardComputeShader.SetInts("StartId", new int[] { updateAreaMinX, updateAreaMinY });

            int threadGroupsX_Dev = Mathf.FloorToInt((updateAreaMaxX - updateAreaMinX - 1.0f) / threadCountX);
            int threadGroupsY_Dev = Mathf.FloorToInt((updateAreaMaxY - updateAreaMinY - 1.0f) / threadCountY);

            SandBoardComputeShader.Dispatch(DisplacementKernel, threadGroupsX_Dev, threadGroupsY_Dev, 1);
        }


    }


    void InitComputeShaderKernel()
    {
        InitKernel = SandBoardComputeShader.FindKernel("Init");
        CollisionTestKernel = SandBoardComputeShader.FindKernel("CollisionTest");
        DisplacementVerticalKernel = SandBoardComputeShader.FindKernel("DisplacementVertical");
        DisplacementHorizontalKernel = SandBoardComputeShader.FindKernel("DisplacementHorizontal");
        DisplacementKernel = SandBoardComputeShader.FindKernel("Displacement");
        ErosionKernel = SandBoardComputeShader.FindKernel("Erosion");
        SkinnyPouringKernel = SandBoardComputeShader.FindKernel("SkinnyPouring");
        ScatterPouringKernel = SandBoardComputeShader.FindKernel("ScatterPouring");

        // Init
        SandBoardComputeShader.SetTexture(InitKernel, "Height", HeightMap);
        SandBoardComputeShader.SetTexture(InitKernel, "DisplacementHeightX", DisplacementHeightXMap);
        SandBoardComputeShader.SetTexture(InitKernel, "DisplacementHeightY", DisplacementHeightYMap);
        SandBoardComputeShader.SetTexture(InitKernel, "InitMap", InitTexture);
        SandBoardComputeShader.SetFloat("MaxHeight", MaxHeight);
        SandBoardComputeShader.SetFloat("InitHeight", InitHeight);
        SandBoardComputeShader.SetFloats("InitColor", new float[] { InitColor.r, InitColor.g, InitColor.b});

        // CollisionTest
        SandBoardComputeShader.SetTexture(CollisionTestKernel, "Height", HeightMap);
        SandBoardComputeShader.SetTexture(CollisionTestKernel, "DisplacementHeightX", DisplacementHeightXMap);
        SandBoardComputeShader.SetTexture(CollisionTestKernel, "DisplacementHeightY", DisplacementHeightYMap);
        SandBoardComputeShader.SetTexture(CollisionTestKernel, "Collision", CollisionMap);

        // DisplacementVertical
        SandBoardComputeShader.SetTexture(DisplacementVerticalKernel, "DisplacementHeightY", DisplacementHeightYMap);
        SandBoardComputeShader.SetTexture(DisplacementVerticalKernel, "Collision", CollisionMap);

        // DisplacementHorizontal
        SandBoardComputeShader.SetTexture(DisplacementHorizontalKernel, "DisplacementHeightX", DisplacementHeightXMap);
        SandBoardComputeShader.SetTexture(DisplacementHorizontalKernel, "Collision", CollisionMap);

        // Displacement
        SandBoardComputeShader.SetTexture(DisplacementKernel, "Height", HeightMap);
        SandBoardComputeShader.SetTexture(DisplacementKernel, "DisplacementHeightX", DisplacementHeightXMap);
        SandBoardComputeShader.SetTexture(DisplacementKernel, "DisplacementHeightY", DisplacementHeightYMap);
        SandBoardComputeShader.SetInts("CollisionMapSize", new int[] { CollisionMap.width, CollisionMap.height});


        // Sand Pouring
        SandBoardComputeShader.SetTexture(SkinnyPouringKernel, "Height", HeightMap);
        SandBoardComputeShader.SetTexture(ScatterPouringKernel, "Height", HeightMap);
        SandBoardComputeShader.SetInts("HeightMapSize", new int[] { HeightMap.width, HeightMap.height });

        SandBoardComputeShader.SetTexture(ErosionKernel, "Height", HeightMap);
        SandBoardComputeShader.SetInts("HeightMapSize", new int[] { HeightMap.width, HeightMap.height });
    }
}
