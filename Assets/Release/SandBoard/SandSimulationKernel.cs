using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Unity.Mathematics;
using UnityEngine;

public class SandSimulationKernel : MonoBehaviour 
{
    [SerializeField] private ComputeShader SandBoardComputeShader;
    [SerializeField] private RenderTexture HeightMap;
    [SerializeField] private RenderTexture DisplacementHeightXMap;
    [SerializeField] private RenderTexture DisplacementHeightYMap;
    [SerializeField] private RenderTexture CollisionMap;
    [SerializeField] private RenderTexture DisplacementHeightMap;
    [SerializeField] private Texture2D InitTexture;

    public int ThreadCountX => _threadCountX;
    public int ThreadCountY => _threadCountY;


    private int _initKernel = 0;
    private int _collisionTestKernel = 0;
    private int _displacementVerticalKernel = 0;
    private int _displacementHorizontalKernel = 0;
    private int _displacementKernel = 0;
    private int _erosionKernel = 0;
    private int _skinnyPouringKernel = 0;
    private int _scatterPouringKernel = 0;

    private int _threadGroupsX;
    private int _threadGroupsY;

    private int _threadCountX = 8;
    private int _threadCountY = 8;

    private float[] _displacementRatio = new float[2];
    private int[] _skinnyCenter = new int[2];
    private float _epsilon = 0.0001f;
    private int _maxErosionRangeX = 10;
    private int _maxErosionRangeY = 10;

    private int _collisionMapWidth;
    private int _collisionMapHeight;

    public int MapWidth => _collisionMapWidth;
    public int MapHeight => _collisionMapHeight;
    public float UpdateArea => _updateArea.Area;

    // for simulation
    public float InitHeight = 0.5f;
    public float MaxHeight = 1.0f;
    public Color InitColor;

    private SandPixelArea _updateArea = new SandPixelArea(0, 0, 0, 0);
    private SandPixelArea _area = new SandPixelArea(0, 0, 0, 0);

    private void Start()
    {
        InitComputeShaderKernel();
        InitThread();

        _collisionMapWidth = CollisionMap.width;
        _collisionMapHeight = CollisionMap.height;

    }

    public void ResetUpdateArea()
    {
        _updateArea.SetBounds(_collisionMapWidth - 1, _collisionMapHeight - 1, 0, 0);
    }

    public bool IsAreaUpdated()
    {
        return !_updateArea.IsEmpty();
    }

    void InitThread()
    {
        _threadGroupsX = Mathf.CeilToInt(HeightMap.width / _threadCountX);
        _threadGroupsY = Mathf.CeilToInt(HeightMap.height / _threadCountY);
    }

    public void Init()
    {
        SandBoardComputeShader.Dispatch(_initKernel, _threadGroupsX, _threadGroupsY, 1);
    }

    public void World2Pixel(Vector3 pos, Transform planeTransform, int[] pixelPos)
    {
        Vector3 localCenter = pos - planeTransform.position;
        Vector3 collisionPlaneScale = planeTransform.lossyScale;
        pixelPos[0] = Mathf.Clamp(Mathf.CeilToInt((localCenter.x / collisionPlaneScale.x + 0.5f) * (_collisionMapWidth - 1)) - 1, 0, _collisionMapWidth - 1);
        pixelPos[1] = Mathf.Clamp(Mathf.CeilToInt((localCenter.z / collisionPlaneScale.y + 0.5f) * (_collisionMapHeight - 1)) - 1, 0, _collisionMapHeight - 1);
    }

    public void World2Pixel(Bounds bounds, Transform planeTransform, ref SandPixelArea pixelArea)
    {
        Vector3 localMin = bounds.min - planeTransform.position;
        Vector3 localMax = bounds.max - planeTransform.position;
        Vector3 collisionPlaneScale = planeTransform.lossyScale;
        int minX = Mathf.Clamp(Mathf.CeilToInt((localMin.x / collisionPlaneScale.x + 0.5f) * (_collisionMapWidth - 1)) - 1, 0, _collisionMapWidth - 1);
        int maxX = Mathf.Clamp(Mathf.CeilToInt((localMax.x / collisionPlaneScale.x + 0.5f) * (_collisionMapWidth - 1)) + 1, 0, _collisionMapWidth - 1);
        int minY = Mathf.Clamp(Mathf.CeilToInt((localMin.z / collisionPlaneScale.y + 0.5f) * (_collisionMapHeight - 1)) - 1, 0, _collisionMapHeight - 1);
        int maxY = Mathf.Clamp(Mathf.CeilToInt((localMax.z / collisionPlaneScale.y + 0.5f) * (_collisionMapHeight - 1)) + 1, 0, _collisionMapHeight - 1);
        pixelArea.SetBounds(minX, minY, maxX, maxY);
    }

    public void CollisionTest(Bounds bounds, Transform planeTransform, ICollision collision, bool updateArea = true)
    {
        World2Pixel(bounds, planeTransform, ref _area);

        SandBoardComputeShader.SetInts("StartId", _area.StartId);
        Vector3 velocity = collision.CenterVelocity;
        float absX = math.abs(velocity.x) + _epsilon;
        float absY = math.abs(velocity.z) + _epsilon;
        _displacementRatio[0] = absX / (absX + absY);
        _displacementRatio[1] = absY / (absY + absX);
        SandBoardComputeShader.SetFloats("DisplacementRatio", _displacementRatio);
        int threadGroupsX_Dev = Mathf.FloorToInt(_area.Width / _threadCountX);
        int threadGroupsY_Dev = Mathf.FloorToInt(_area.Height / _threadCountY);
        SandBoardComputeShader.Dispatch(_collisionTestKernel, threadGroupsX_Dev, threadGroupsY_Dev, 1);

        if (updateArea)
        {
            _updateArea.Merge(_area);
        }
    }


    public void ScatterPouring(Bounds bounds, Transform planeTransform,  float[] sandAmount)
    {
        World2Pixel(bounds, planeTransform, ref _area);
  
        SandBoardComputeShader.SetInts("SandCenter", _area.Center);
        SandBoardComputeShader.SetFloat("SandRadius", 0.5f * (_area.MaxY - _area.MinY));
        SandBoardComputeShader.SetInts("StartId", _area.StartId);
        SandBoardComputeShader.SetFloats("SandAmount", sandAmount);

        int threadGroupsX_Dev = Mathf.FloorToInt(_area.Width / ThreadCountX);
        int threadGroupsY_Dev = Mathf.FloorToInt(_area.Height / ThreadCountY);
        SandBoardComputeShader.Dispatch(_scatterPouringKernel, threadGroupsX_Dev, threadGroupsY_Dev, 1);
    }

    public void SkinnyPouring(Bounds bounds, Transform planeTransform, Vector3 skinnyPouringCenter, float[] sandAmount, float sandRadius)
    {

        World2Pixel(bounds, planeTransform, ref _area);
        World2Pixel(skinnyPouringCenter, planeTransform, _skinnyCenter);

        SandBoardComputeShader.SetInts("StartId", _area.StartId);
        SandBoardComputeShader.SetFloat("SandRadius", sandRadius);
        SandBoardComputeShader.SetFloats("SandAmount", sandAmount);
        SandBoardComputeShader.SetInts("SandCenter", _skinnyCenter);

        int threadGroupsX_Dev = Mathf.FloorToInt(_area.Width / ThreadCountX);
        int threadGroupsY_Dev = Mathf.FloorToInt(_area.Height / ThreadCountY);
        SandBoardComputeShader.Dispatch(_skinnyPouringKernel, threadGroupsX_Dev, threadGroupsY_Dev, 1);
    }


    public void Sweep(Vector3 direction)
    {
        int startX, endX, stepX;
        int startY, endY, stepY;

        int threadGroupsX_Dev = Mathf.FloorToInt(_updateArea.Width / _threadCountX);
        int threadGroupsY_Dev = Mathf.FloorToInt(_updateArea.Height / _threadCountY);

        if (direction.x < 0)
        {
            startX = _updateArea.MaxX - 1;
            endX = _updateArea.MinX;
            stepX = -1;
        }
        else
        {
            startX = _updateArea.MinX + 1;
            endX = _updateArea.MaxX;
            stepX = 1;
        }

        if (direction.z < 0)
        {
            startY = _updateArea.MaxY - 1;
            endY = _updateArea.MinY;
            stepY = -1;
        }
        else
        {
            startY = _updateArea.MinY + 1;
            endY = _updateArea.MaxY;
            stepY = 1;
        }

        int nextDisplacementRaw = 0;
        for (int i = startY; i != endY; i += stepY)
        {
            nextDisplacementRaw = Mathf.Min(_collisionMapHeight - 1, Mathf.Max(i + stepY * _maxErosionRangeY, 0));
            SandBoardComputeShader.SetInt("DisplacementRaw", i);
            SandBoardComputeShader.SetInt("NextDisplacementRaw", nextDisplacementRaw);
            SandBoardComputeShader.Dispatch(_displacementVerticalKernel, threadGroupsX_Dev, 1, 1);
        }


        _updateArea.ExtendY(nextDisplacementRaw);
        _updateArea.ExtendY(startY);

        int nextDisplacementColumn = 0;
        for (int i = startX; i != endX; i += stepX)
        {
            nextDisplacementColumn = Mathf.Min(_collisionMapWidth - 1, Mathf.Max(i + stepX * _maxErosionRangeX, 0));
            SandBoardComputeShader.SetInt("DisplacementColumn", i);
            SandBoardComputeShader.SetInt("NextDisplacementColumn", nextDisplacementColumn);
            SandBoardComputeShader.Dispatch(_displacementHorizontalKernel, 1, threadGroupsY_Dev, 1);
        }
        _updateArea.ExtendX(nextDisplacementColumn);
        _updateArea.ExtendX(startX);




    }

    public void Displacement()
    {
        SandBoardComputeShader.SetInts("StartId", _updateArea.StartId);
        int threadGroupsX_Dev = Mathf.FloorToInt(_updateArea.Width / ThreadCountX);
        int threadGroupsY_Dev = Mathf.FloorToInt(_updateArea.Height / ThreadCountY);
        SandBoardComputeShader.Dispatch(_displacementKernel, threadGroupsX_Dev, threadGroupsY_Dev, 1);
    }




    void InitComputeShaderKernel()
    {
        _initKernel = SandBoardComputeShader.FindKernel("Init");
        _collisionTestKernel = SandBoardComputeShader.FindKernel("CollisionTest");
        _displacementVerticalKernel = SandBoardComputeShader.FindKernel("DisplacementVertical");
        _displacementHorizontalKernel = SandBoardComputeShader.FindKernel("DisplacementHorizontal");
        _displacementKernel = SandBoardComputeShader.FindKernel("Displacement");
        _erosionKernel = SandBoardComputeShader.FindKernel("Erosion");
        _skinnyPouringKernel = SandBoardComputeShader.FindKernel("SkinnyPouring");
        _scatterPouringKernel = SandBoardComputeShader.FindKernel("ScatterPouring");

        // Init
        SandBoardComputeShader.SetTexture(_initKernel, "Height", HeightMap);
        SandBoardComputeShader.SetTexture(_initKernel, "DisplacementHeightX", DisplacementHeightXMap);
        SandBoardComputeShader.SetTexture(_initKernel, "DisplacementHeightY", DisplacementHeightYMap);
        SandBoardComputeShader.SetTexture(_initKernel, "InitMap", InitTexture);


        // CollisionTest
        SandBoardComputeShader.SetTexture(_collisionTestKernel, "Height", HeightMap);
        SandBoardComputeShader.SetTexture(_collisionTestKernel, "DisplacementHeightX", DisplacementHeightXMap);
        SandBoardComputeShader.SetTexture(_collisionTestKernel, "DisplacementHeightY", DisplacementHeightYMap);
        SandBoardComputeShader.SetTexture(_collisionTestKernel, "Collision", CollisionMap);

        // DisplacementVertical
        SandBoardComputeShader.SetTexture(_displacementVerticalKernel, "DisplacementHeightY", DisplacementHeightYMap);
        SandBoardComputeShader.SetTexture(_displacementVerticalKernel, "Collision", CollisionMap);

        // DisplacementHorizontal
        SandBoardComputeShader.SetTexture(_displacementHorizontalKernel, "DisplacementHeightX", DisplacementHeightXMap);
        SandBoardComputeShader.SetTexture(_displacementHorizontalKernel, "Collision", CollisionMap);

        // Displacement
        SandBoardComputeShader.SetTexture(_displacementKernel, "Height", HeightMap);
        SandBoardComputeShader.SetTexture(_displacementKernel, "DisplacementHeightX", DisplacementHeightXMap);
        SandBoardComputeShader.SetTexture(_displacementKernel, "DisplacementHeightY", DisplacementHeightYMap);


        // Sand Pouring
        SandBoardComputeShader.SetTexture(_skinnyPouringKernel, "Height", HeightMap);
        SandBoardComputeShader.SetTexture(_scatterPouringKernel, "Height", HeightMap);
        SandBoardComputeShader.SetTexture(_erosionKernel, "Height", HeightMap);


        // params
        SandBoardComputeShader.SetFloat("MaxHeight", MaxHeight);
        SandBoardComputeShader.SetFloat("InitHeight", InitHeight);
        SandBoardComputeShader.SetFloats("InitColor", new float[] { InitColor.r, InitColor.g, InitColor.b });
        SandBoardComputeShader.SetInts("CollisionMapSize", new int[] { CollisionMap.width, CollisionMap.height });
        SandBoardComputeShader.SetInts("HeightMapSize", new int[] { HeightMap.width, HeightMap.height });
    }
}