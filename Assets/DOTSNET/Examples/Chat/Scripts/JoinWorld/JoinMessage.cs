using Unity.Collections;

namespace DOTSNET.Examples.Chat
{
    public struct JoinMessage : NetworkMessage
    {
        public FixedString32 name;

        public JoinMessage(FixedString32 name)
        {
            this.name = name;
        }

        public bool Serialize(ref NetworkWriter writer) =>
            writer.WriteFixedString32(name);

        public bool Deserialize(ref NetworkReader reader) =>
            reader.ReadFixedString32(out name);
    }
}