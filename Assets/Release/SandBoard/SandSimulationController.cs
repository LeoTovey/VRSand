using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

public class SandSimulationController : MonoBehaviour
{

    public float InitHeight = 0.5f;
    public float MaxHeight = 1.0f;
    public Color InitColor;


    public SandSimulationKernel Kernel;

    private void Start()
    {
        
    }

}