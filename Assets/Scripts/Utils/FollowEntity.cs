using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class FollowEntity : MonoBehaviour
{
    public Entity Target;
    public float3 Offset;
    private EntityManager _manager;

    public void Awake()
    {
        _manager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }

    public void LateUpdate()
    {
        if (Target == null) return;
        var translation = _manager.GetComponentData<Translation>(Target);
        transform.position = translation.Value + Offset;
    }
}
