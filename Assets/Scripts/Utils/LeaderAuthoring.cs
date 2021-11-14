using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;

[AddComponentMenu("Custom Authoring/LeaderAuthoring")]
public class LeaderAuthoring : MonoBehaviour, IConvertGameObjectToEntity
{
    [SerializeField]
    
    private GameObject _followerObject;
    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        var followEntity = _followerObject.GetComponent<FollowEntity>();
        if (followEntity == null) followEntity = _followerObject.AddComponent<FollowEntity>();
        followEntity.Target = entity;
    }
}
