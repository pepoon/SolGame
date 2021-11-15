// A message that sends the full state of a NetworkIdentity to the client in order
// to spawn it.
using Unity.Collections;
using Unity.Mathematics;
using UnityEngine;

namespace DOTSNET
{
    public unsafe struct SpawnMessage : NetworkMessage
    {
        // client needs to know which prefab to spawn
        public Bytes16 prefabId;

        // client needs to know which netId was assigned to this entity
        public ulong netId;

        // flag to indicate if the connection that we send it to owns the entity
        public bool owned;

        // the spawn position
        // unlike StateMessage, we include the position once when spawning so
        // that even without a NetworkTransform system, it's still positioned
        // correctly when spawning.
        public float3 position;

        // the spawn rotation
        // unlike StateMessage, we include the rotation once when spawning so
        // that even without a NetworkTransform system, it's still rotated
        // correctly when spawning.
        public quaternion rotation;

        // payload contains the serialized component data.
        // we also store the exact bit size.
        // -> 'fixed byte[]' is inlined. better than allocating a 'new byte[]'!
        // -> also avoids allocation attacks since the size is always fixed!
        public int payloadSize;
        // 128 bytes per entity should be way enough
        // => has to be the size of the NetworkComponentsSerialization NetworkWriter!
        public const int PayloadFixedSize = 128;
        public fixed byte payload[PayloadFixedSize];

        public SpawnMessage(Bytes16 prefabId, ulong netId, bool owned, float3 position, quaternion rotation, NetworkWriter128 serialization)
        {
            this.prefabId = prefabId;
            this.netId = netId;
            this.owned = owned;
            this.position = position;
            this.rotation = rotation;

            // were any NetworkComponents serialized?
            payloadSize = serialization.Position;
            if (serialization.Position > 0)
            {
                // copy writer into our payload
                fixed (byte* buffer = payload)
                {
                    if (serialization.CopyTo(buffer, PayloadFixedSize) == 0)
                        Debug.LogError($"Failed to copy writer at Position={serialization.Position} to StateUpdateMessage payload");
                }
            }
        }

        public bool Serialize(ref NetworkWriter writer)
        {
            fixed (byte* buffer = payload)
            {
                // rotation is compressed from 16 bytes quaternion into 4 bytes
                //   100,000 messages * 16 byte = 1562 KB
                //   100,000 messages *  4 byte =  391 KB
                // => DOTSNET is bandwidth limited, so this is a great idea.
                return writer.WriteBytes16(prefabId) &&
                       writer.WriteULong(netId) &&
                       writer.WriteBool(owned) &&
                       writer.WriteFloat3(position) &&
                       writer.WriteQuaternionSmallestThree(rotation) &&
                       // write payload
                       writer.WriteInt(payloadSize) &&
                       writer.WriteBytes(buffer, PayloadFixedSize, 0, payloadSize);
            }
        }

        public bool Deserialize(ref NetworkReader reader)
        {
            if (reader.ReadBytes16(out prefabId) &&
                reader.ReadULong(out netId) &&
                reader.ReadBool(out owned) &&
                reader.ReadFloat3(out position) &&
                reader.ReadQuaternionSmallestThree(out rotation) &&
                // read payload size
                reader.ReadInt(out payloadSize) &&
                // verify size
                payloadSize <= PayloadFixedSize * 8)
            {
                fixed (byte* buffer = payload)
                {
                    return reader.ReadBytes(buffer, PayloadFixedSize, payloadSize);
                }
            }
            return false;
        }
    }
}
