using Unity.Burst;
using Unity.Collections;
using Unity.Entities;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Transforms;
using UnityEngine;

public class CameraFollowPlayer : MonoBehaviour
{
    public Transform Player;
    public void LateUpdate()
    {
        transform.position = Player.position;
    }
}
