// Applies the Snapshot message to the Entity.
// There is no interpolation yet, only the bare minimum.
using Unity.Collections;
using Unity.Entities;
using Unity.Transforms;

namespace DOTSNET
{
    // the system is always created because Snapshot is always needed
    public class SnapshotClientMessageSystem : NetworkClientMessageSystem<SnapshotMessage>
    {
        // dependencies
        [AutoAssign] protected NetworkComponentSerializers serialization;

        // cache new messages <netId, message> to apply all at once in OnUpdate.
        // finding the Entity with netId and calling SetComponent for one Entity
        // in OnMessage 10k times would be very slow.
        // a ForEach query is faster, it can use Burst(!) and it could be a Job.
        NativeHashMap<ulong, SnapshotMessage> messages;

        protected override void OnCreate()
        {
            // call base because it might be implemented.
            base.OnCreate();

            // create messages HashMap
            messages = new NativeHashMap<ulong, SnapshotMessage>(1000, Allocator.Persistent);
        }

        protected override void OnDestroy()
        {
            // dispose with Dependency in case it's used in a Job
            messages.Dispose(Dependency);

            // call base because it might be implemented.
            base.OnDestroy();
        }

        protected override void OnMessage(SnapshotMessage message)
        {
            // store in messages
            // note: we might overwrite the previous Snapshot, but
            //       that's fine since we don't send deltas and we only care
            //       about the latest position/rotation.
            //       so this can even avoid some computations.
            messages[message.netId] = message;
            //Debug.LogWarning($"Client received SnapshotMessage for netId={message.netId} with payload={message.payloadBitSize} bits");
        }

        // TODO this should be called from Client/Server after processing all
        // incoming messages later. at the perfect time, effective immediately.
        protected override void OnUpdate()
        {
            // don't need to Entities.ForEach every update.
            // only if we actually received anything.
            if (messages.IsEmpty)
                return;

            // we assume large amounts of entities, so we go through all of them
            // and apply their Snapshot message (if any).
            NativeHashMap<ulong, SnapshotMessage> _messages = messages;
            Entities.ForEach((ref Translation translation,
                              ref Rotation rotation,
                              ref NetworkComponentsDeserialization deserialization,
                              in NetworkIdentity identity) =>
            {
                // do we have a message for this netId?
                if (_messages.ContainsKey(identity.netId))
                {
                    SnapshotMessage message = _messages[identity.netId];
                    //Debug.LogWarning($"Apply Snapshot netId={message.netId} pos={message.position} rot={message.rotation}");

                    // server always syncs transform.
                    // apply only if not local player && CLIENT_TO_SERVER.
                    bool hasTransformAuthority = identity.owned &&
                                                 identity.transformDirection == SyncDirection.CLIENT_TO_SERVER;
                    if (!hasTransformAuthority)
                    {
                        translation.Value = message.position;
                        rotation.Value = message.rotation;
                        //Debug.LogWarning($"Apply Snapshot Transform for netId={message.netId} pos={message.position} rot={message.rotation}");
                    }

                    // copy payload to Deserialization component
                    // deserialization will ignore CLIENT_TO_SERVER if authority.
                    unsafe
                    {
                        deserialization.reader = new NetworkReader128(message.payload, message.payloadSize);
                    }
                }
            })
            // DO NOT Schedule()!
            // The time it takes to start the job is way too noticeable for
            // position updates on clients. Try to run the Pong example with
            // server as build and client as build. It's way too noticeable.
            .Run();

            // tell serializers to deserialize all NetworkComponents once
            serialization.DeserializeAll();

            // clear messages after everything is done
            // (in case we ever need their memory when deserializing in the future)
            messages.Clear();
        }
    }
}