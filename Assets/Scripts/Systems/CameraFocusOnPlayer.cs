using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
public class CameraFocusOnTarget : SystemBase
{
    Translation player;
    Translation camera;
    protected override void OnStartRunning()
    {
       
    }
    protected override void OnUpdate()
    {
        //var playerEntity = GetEntityQuery(typeof(PlayerData)).get();
        //player = EntityManager.GetComponentData<Translation>(playerEntity);

        //var cameraEntity = GetEntityQuery(typeof(CameraData)).GetSingletonEntity();
        //camera = EntityManager.GetComponentData<Translation>(cameraEntity);
        //camera.Value = player.Value;
    }
}
