using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class ProcessInputSystem : SystemBase
{
    protected override void OnUpdate()
    {
        float inputH = Input.GetAxis("Horizontal");
        float inputV = Input.GetAxis("Vertical");
        inputH = inputH > 0 ? 1 : inputH < 0 ? -1 : 0;
        inputV = inputV > 0 ? 1 : inputV < 0 ? -1 : 0;
        Entities.ForEach((ref RawInputData input, ref MoveData moveData) => {
            input.InputH = inputH;
            input.InputV = inputV;
            moveData.MoveDir = new float3(input.InputH, input.InputV, 0);
        }).Schedule();
    }
}
