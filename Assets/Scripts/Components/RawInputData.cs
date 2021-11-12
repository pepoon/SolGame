using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct RawInputData : IComponentData
{
    [HideInInspector]
    public float InputH;
    [HideInInspector]
    public float InputV;
}
