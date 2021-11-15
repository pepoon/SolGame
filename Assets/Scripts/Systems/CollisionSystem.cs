using Unity.Entities;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CollisionSystem : SystemBase
{
    EndSimulationEntityCommandBufferSystem m_EndSimulationEcbSystem;
    EntityCommandBufferSystem a;

    protected override void OnCreate()
    {
        base.OnCreate();
         a = World.GetExistingSystem<EntityCommandBufferSystem>();
        // Find the ECB system once and store it for later usage
        m_EndSimulationEcbSystem = World
            .GetOrCreateSystem<EndSimulationEntityCommandBufferSystem>();
    }

    protected override void OnUpdate()
    {
        EntityQuery query = GetEntityQuery(ComponentType.ReadOnly<Translation>(),
           ComponentType.ReadOnly<PlayerTag>());

        var player = query.GetSingletonEntity();
        var playerTranslation = query.GetSingleton<Translation>();
        var playerPos = playerTranslation.Value;

        //var ecb = m_EndSimulationEcbSystem.CreateCommandBuffer();
        //var cmdBuffer = a.CreateCommandBuffer();
        var d = m_EndSimulationEcbSystem.PostUpdateCommands;
        Entities.WithStructuralChanges().ForEach((ref Entity e, in Translation translation, in EnemyTag enemy) =>
        {
            var enemyPos = translation.Value;
            var collision = math.distancesq(enemyPos, playerPos) < 0.5f;
            if (collision)
            {
                //var g = new LinkedEntityGroup();
                //g.Value = e;
                //g.
                EntityManager.DestroyEntity(e);
                Debug.Log("HIT");
            }
        }).Run();
    }
}
