using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;
using Unity.Mathematics;
public class FollowTarget : SystemBase
{
    protected override void OnUpdate()
    {
        //Entities.ForEach((ref MoveData moveData, in TargetData target, in Translation transation) => {

        //    var trasnslationArray = GetComponentDataFromEntity<Translation>(true);
        //    if (!trasnslationArray.HasComponent(target.Entity)) { return; }

        //    var targetPosition = trasnslationArray[target.Entity];
        //    moveData.MoveDir = math.normalize(targetPosition.Value - transation.Value);
        //    moveData.MoveDir.z = 0;
        //}).Schedule();
    }
}
