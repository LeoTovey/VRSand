using UnityEngine;
using Unity.Mathematics;


public interface ICollision
{
    Vector3 CenterVelocity { get; }
    Bounds CollisionBound { get; }
    abstract public bool DetectCollision();
}
