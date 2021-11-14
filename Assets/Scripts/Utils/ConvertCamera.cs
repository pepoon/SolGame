using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class ConvertCamera : MonoBehaviour, IConvertGameObjectToEntity
{
    public EntityManager entityManager;
    public float moveSpeed;
    public Entity entity;
    private void Awake()
    {
        entityManager = World.DefaultGameObjectInjectionWorld.EntityManager;
    }
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<CopyTransformToGameObject>(entity);

        dstManager.AddComponentData(entity, new MoveData { MoveSpeed = moveSpeed });
        dstManager.AddComponentData(entity, new TargetData { Entity = entity});
    }

}
