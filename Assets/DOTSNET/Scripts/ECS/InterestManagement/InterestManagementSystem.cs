﻿// Interest Management is needed to broadcast an entity's updates only to the
// surrounding players in order to save bandwidth.
//
// This can be done in a lot of different ways:
// - brute force distance checking everyone to everyone else
// - physics sphere casts to find everyone in a radius
// - spatial hashing aka grid checking
// - etc.
//
// So we need a base class for all of them.
using Unity.Collections;
using Unity.Entities;

namespace DOTSNET
{
    // ComponentSystem for now. Jobs come later.
    [ServerWorld]
    // update AFTER everything else, but before Server broadcasts.
    // and only while server is active.
    [UpdateInGroup(typeof(ServerActiveLateSimulationSystemGroup))]
    // IMPORTANT: use [UpdateBefore(typeof(BroadcastSystem))] when inheriting
    // IMPORTANT: use [DisableAutoCreation] + SelectiveSystemAuthoring when
    //            inheriting
    public abstract class InterestManagementSystem : SystemBase
    {
        // dependencies
        [AutoAssign] protected NetworkServerSystem server;
        [AutoAssign] protected NetworkComponentSerializers serializers;

        // cache messages so we can ForEach with Burst and send them afterwards
        protected NativeMultiHashMap<int, SpawnMessage> spawnMessages;
        protected NativeMultiHashMap<int, UnspawnMessage> unspawnMessages;

        protected override void OnCreate()
        {
            // allocate
            spawnMessages = new NativeMultiHashMap<int, SpawnMessage>(1000, Allocator.Persistent);
            unspawnMessages = new NativeMultiHashMap<int, UnspawnMessage>(1000, Allocator.Persistent);
        }

        protected override void OnDestroy()
        {
            spawnMessages.Dispose();
            unspawnMessages.Dispose();
        }

        // rebuild all areas of interest for everyone once
        //
        // note:
        //   we DO NOT do any custom rebuilding after someone joined/spawned or
        //   disconnected/unspawned.
        //   this would require INSANE complexity.
        //   for example, OnTransportDisconnect would have to:
        //     1. remove the connection so the connectionId is invalid
        //     2. then call BroadcastAfterUnspawn(oldEntity) which broadcasts
        //        destroyed messages BEFORE rebuilding so we know the old
        //        observers that need to get the destroyed message
        //     3. RebuildAfterUnspawn to remove it
        //     4. then remove the Entity from connection's owned objects, which
        //        IS NOT POSSIBLE anymore because the connection was already
        //        removed. which means that the next rebuild would still see it
        //        etc.
        //        (it's just insanity)
        //   additionally, we would also need extra flags in Spawn to NOT
        //   rebuild when spawning 10k scene objects in start, etc.s
        //
        //   DOTS is fast, so it makes no sense to have that insane complexity.
        //
        // first principles:
        //   it wouldn't even make sense to have special cases because players
        //   might walk in and out of range from each other all the time anyway.
        //   we already need to handle that case. (dis)connect is no different.
        public abstract void OnRebuildAll();
        public void RebuildAll()
        {
            // serialize all networkcomponents once
            // (we need them for spawn messages)
            serializers.SerializeAll();

            // now call the OnRebuildAll implementation
            OnRebuildAll();
        }

        // flush all unspawn messages
        void FlushUnspawnMessages()
        {
            // send Unspawn message for each one in the removed buffer
            // => we send Unspawn before Spawn to have minimum amount of
            //    Entities on the client.
            //
            // see AddNewObservers() comment for the 3 different cases.
            // it's the same here.
            //server.Send(unspawnMessages);

            // for each connectionId:
            foreach (int connectionId in server.connections.Keys)
            {
                NativeMultiHashMapIterator<int>? it = default;
                while (unspawnMessages.TryIterate(connectionId, out UnspawnMessage message, ref it))
                {
                    server.Send(connectionId, message);
                }
            }

            // clean up
            unspawnMessages.Clear();
        }

        // flush all spawn messages
        void FlushSpawnMessages()
        {
            // send Spawn message for each one in the removed buffer
            //
            // there are three possible cases:
            //   if we have a monster and a player walks near it:
            //   -> we add player connectionId to monster observers
            //   -> we need to send SpawnMessage(Monster) to player
            //
            //   if we have playerA and playerB and they walk near:
            //   -> we add playerB connectionId to playerA observers
            //      when rebuilding playerA
            //   -> we need to send SpawnMessage(PlayerB) to playerA
            //      we do NOT need to send SpawnMessage(PlayerA) to
            //      playerB because rebuilding playerB will take care of
            //      it later.
            //
            //   if a player first spawns into an empty world:
            //   -> we will add his own connectionId to his observers
            //   -> and then send SpawnMessage(Player) to his connection
            //
            // => all three cases require the same call:
            //server.Send(spawnMessages);

            // for each connectionId:
            foreach (int connectionId in server.connections.Keys)
            {
                NativeMultiHashMapIterator<int>? it = default;
                while (spawnMessages.TryIterate(connectionId, out SpawnMessage message, ref it))
                {
                    server.Send(connectionId, message);
                }
            }

            // clean up
            spawnMessages.Clear();
        }

        // spawn/unspawn all observers that were added/removed from the
        // implementation's Job
        // make sure to call this from the implementation after rebuilding.
        protected void FlushMessages()
        {
            FlushUnspawnMessages();
            FlushSpawnMessages();
        }
    }
}
