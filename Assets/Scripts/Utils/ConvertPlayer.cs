using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.Entities;
using Unity.Transforms;

public class ConvertPlayer : MonoBehaviour, IConvertGameObjectToEntity
{
    public GameObject camera;

    public void Convert(Entity entity, EntityManager dstManager, GameObjectConversionSystem conversionSystem)
    {
        dstManager.AddComponent<CopyTransformToGameObject>(entity);

        var convertCamera = camera.GetComponent<ConvertCamera>();
        if (convertCamera == null)
        {
            convertCamera = camera.AddComponent<ConvertCamera>();
        }
        convertCamera.entity = entity;
    }

}
