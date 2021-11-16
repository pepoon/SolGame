using Unity.Entities;

namespace SolGame
{
    [GenerateAuthoringComponent]
    public struct PlayerMovementData : IComponentData
    {
        // movement speed in m/s
        public float speed;
    }
}
