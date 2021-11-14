using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;

public class RotateTowardsSystem : SystemBase
{
    protected override void OnUpdate()
    {
        var deltaTime = Time.DeltaTime;

        Entities.ForEach((ref Rotation rotation, in RotationData rotationData, in MoveData moveData) => {

            //if (!moveData.MoveDir.Equals(float3.zero))
            //{
            //    quaternion targetRotation = quaternion.LookRotationSafe(moveData.MoveDir, math.back());
            //    rotation.Value = math.slerp(rotation.Value, targetRotation, rotationData.RotationSpeed);
            //}
        }).Schedule();
    }
}
