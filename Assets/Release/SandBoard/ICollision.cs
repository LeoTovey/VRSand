using UnityEngine;
using Unity.Mathematics;

public enum CollisionType
{
    Hand,
    Tool
}

public enum CollisionStatus
{ 
    Invalid,
    Valid
}
public interface ICollision
{
    Vector3 CenterVelocity { get; }
    Bounds CollisionBound { get; }
    CollisionType CollisionType { get; }
    abstract public bool DetectCollision();
}
