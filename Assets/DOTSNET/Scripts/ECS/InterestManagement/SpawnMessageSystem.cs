using Unity.Collections;
using Unity.Entities;

namespace DOTSNET
{
    // use SelectiveAuthoring to create/inherit it selectively
    [DisableAutoCreation]
    public class SpawnMessageSystem : NetworkClientMessageSystem<SpawnMessage>
    {
        // dependencies
        [AutoAssign] protected NetworkComponentSerializers serialization;

        // cache new messages <netId, message> to apply all at once in OnUpdate,
        // so that we only have to call DeserializeAll() ONCE, instead of after
        // every message!
        NativeList<SpawnMessage> messages;

        protected override void OnCreate()
        {
            // call base because it might be implemented.
            base.OnCreate();

            // create messages HashMap
            messages = new NativeList<SpawnMessage>(1000, Allocator.Persistent);
        }

        protected override void OnDestroy()
        {
            // dispose with Dependency in case it's used in a Job
            messages.Dispose(Dependency);

            // call base because it might be implemented.
            base.OnDestroy();
        }

        protected override void OnMessage(SpawnMessage message)
        {
            // store in messages
            messages.Add(message);
        }

        protected override void OnUpdate()
        {
            // now spawn them all
            for (int i = 0; i < messages.Length; ++i)
            {
                // get message
                SpawnMessage message = messages[i];

                // copy payload to reader
                NetworkReader128 deserialization;
                unsafe
                {
                    deserialization = new NetworkReader128(message.payload, message.payloadSize);
                }

                // call spawn
                client.Spawn(message.prefabId,
                             message.netId,
                             message.owned,
                             message.position,
                             message.rotation,
                             deserialization);
            }

            // and call DeserializeAll only once
            serialization.DeserializeAll();

            // clear messages after everything is done
            // (in case we ever need their memory when deserializing in the future)
            messages.Clear();
        }
    }
}
