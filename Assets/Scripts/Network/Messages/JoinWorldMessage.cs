using DOTSNET;
using Unity.Collections;

namespace SolGame.Network.Messages
{
    public struct JoinWorldMessage : NetworkMessage
    {
        public Bytes16 playerPrefabId;

        public JoinWorldMessage(Bytes16 playerPrefabId)
        {
            this.playerPrefabId = playerPrefabId;
        }

        public bool Serialize(ref NetworkWriter writer) =>
            writer.WriteBytes16(playerPrefabId);

        public bool Deserialize(ref NetworkReader reader) =>
            reader.ReadBytes16(out playerPrefabId);
    }
}