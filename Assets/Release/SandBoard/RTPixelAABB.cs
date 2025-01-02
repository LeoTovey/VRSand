using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class RTPixelAABB
{
    public int MinX { get; private set; }
    public int MinY { get; private set; }
    public int MaxX { get; private set; }
    public int MaxY { get; private set; }

    public int Width => MaxX - MinX + 1;
    public int Height => MaxY - MinY + 1;

    public int Area => Width * Height;
    public bool IsEmpty() => MinX >= MaxX || MinY >= MaxY;
    
    
    private int[] _startId;
    private int[] _endId;
    private int[] _center;
    private int _RTWidth;
    private int _RTHeight;

    public int[] StartId
    {
        get
        {
            _startId[0] = MinX;
            _startId[1] = MinY;
            return _startId;
        }
    }

    public int[] EndId
    {
        get
        {
            _endId[0] = MaxX;
            _endId[1] = MaxY;
            return _endId;
        }
    }

    public int[] Center
    {
        get
        {
            _center[0] = (int)(0.5f * (MinX + MaxX));
            _center[1] = (int)(0.5f * (MinY + MaxY));
            return _center;
        }
    }

    public RTPixelAABB(int RTWidth, int RTHeight)
    {
        _RTWidth = RTWidth;
        _RTHeight = RTHeight;
        SetBounds(_RTWidth - 1, _RTHeight - 1, 0, 0);
        _startId = new int[] { _RTWidth - 1, _RTHeight - 1};
        _endId = new int[] { 0, 0 };
        _center = new int[2];
    }

    public void Reset()
    {
        SetBounds(_RTWidth - 1, _RTHeight - 1, 0, 0);
    }

    public void SetBounds(int minX, int minY, int maxX, int maxY)
    {
        MinX = minX;
        MinY = minY;
        MaxX = maxX;
        MaxY = maxY;
    }
    
    public void MergeBounds(Bounds bounds, Transform RTTransform)
    {
        Vector3 localMin = bounds.min - RTTransform.position;
        Vector3 localMax = bounds.max - RTTransform.position;
        Vector3 collisionPlaneScale = RTTransform.lossyScale;
        int minX = Mathf.Clamp(Mathf.CeilToInt((localMin.x / collisionPlaneScale.x + 0.5f) * (_RTWidth - 1)) - 1, 0, _RTWidth - 1);
        int maxX = Mathf.Clamp(Mathf.CeilToInt((localMax.x / collisionPlaneScale.x + 0.5f) * (_RTWidth - 1)) + 1, 0, _RTWidth - 1);
        int minY = Mathf.Clamp(Mathf.CeilToInt((localMin.z / collisionPlaneScale.y + 0.5f) * (_RTHeight - 1)) - 1, 0, _RTHeight - 1);
        int maxY = Mathf.Clamp(Mathf.CeilToInt((localMax.z / collisionPlaneScale.y + 0.5f) * (_RTHeight - 1)) + 1, 0, _RTHeight - 1);
        Merge(minX, minY, maxX, maxY);
    }

    public void ExtendX(int x)
    {
        MaxX = Math.Max(MaxX, x);
        MinX = Math.Min(MinX, x);
    }

    public void ExtendY(int y)
    {
        MinY = Math.Min(MinY, y);
        MaxY = Math.Max(MaxY, y);
    }


    public void Merge(RTPixelAABB aabb)
    {
        MinX = Math.Min(MinX, aabb.MinX);
        MinY = Math.Min(MinY, aabb.MinY);
        MaxX = Math.Max(MaxX, aabb.MaxX);
        MaxY = Math.Max(MaxY, aabb.MaxY);
    }

    public void Merge(int minX, int minY, int maxX, int maxY)
    {
        MinX = Math.Min(MinX, minX);
        MinY = Math.Min(MinY, minY);
        MaxX = Math.Max(MaxX, maxX);
        MaxY = Math.Max(MaxY, maxY);
    }
    
    // public static Vector2 World2Pixel(Vector3 position, Transform planeTransform, Vector2 textureSize)
    // {
    //     Vector3 localCenter = position - planeTransform.position;
    //     Vector3 collisionPlaneScale = planeTransform.lossyScale;
    //     Vector2 pixelPos = Vector2.zero;
    //     pixelPos[0] = Mathf.Clamp(Mathf.CeilToInt((localCenter.x / collisionPlaneScale.x + 0.5f) * (textureSize.x - 1)) - 1, 0, textureSize.x - 1);
    //     pixelPos[1] = Mathf.Clamp(Mathf.CeilToInt((localCenter.z / collisionPlaneScale.y + 0.5f) * (textureSize.y - 1)) - 1, 0, textureSize.y - 1);
    //     return pixelPos;
    // }
    
    

}



