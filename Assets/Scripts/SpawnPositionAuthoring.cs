// Copies own position to JoinWorldMessageSystem.spawnPosition.
// Spawn position is game dependent.
// DOTSNET doesn't know anything about spawn positions.
using DOTSNET;
using UnityEngine;

namespace SolGame
{
    public class SpawnPositionAuthoring : MonoBehaviour
    {
        void Awake()
        {
            Bootstrap.ServerWorld.GetExistingSystem<JoinWorldMessageSystem>().spawnPosition = transform.position;
        }
    }
}
