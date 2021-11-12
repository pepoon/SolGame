using System;
using Unity.Collections;
using Unity.Entities;
using Unity.Mathematics;
using UnityEngine;

[GenerateAuthoringComponent]
public struct MoveData : IComponentData
{
    public float MoveSpeed;
    [HideInInspector]public float3 MoveDir;
}
