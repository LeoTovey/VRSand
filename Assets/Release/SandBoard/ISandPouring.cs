using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;


public interface ISandPouring
{
    Vector3 PouringCenter { get; }

    float PouringVelocity { get; }

    ParticleSystem SandEffect { get; }

    float Strenth { get; }

    bool Enabled { get; }

    float[] SandAmount { get; }

}
