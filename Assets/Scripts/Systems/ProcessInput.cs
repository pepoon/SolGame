using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ProcessInput : SystemBase
{
    protected override void OnUpdate()
    {
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");
        Debug.Log(inputH);
        Entities.ForEach((ref RawInputData input, ref MoveData moveData) => {
            input.InputH = inputH;
            input.InputV = inputV;
            moveData.MoveDir = new float3(input.InputH, input.InputV, 0);
        }).Schedule();
    }
}
