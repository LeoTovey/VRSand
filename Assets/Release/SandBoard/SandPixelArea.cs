using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

public class SandPixelArea
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

    public SandPixelArea(int minX, int minY, int maxX, int maxY)
    {
        SetBounds(minX, minY, maxX, maxY);
        _startId = new int[] { minX, minY};
        _endId = new int[] { maxX, maxY };
        _center = new int[2];
    }

    public void SetBounds(int minX, int minY, int maxX, int maxY)
    {
        MinX = minX;
        MinY = minY;
        MaxX = maxX;
        MaxY = maxY;
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


    public void Merge(SandPixelArea aabb)
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
    
}

