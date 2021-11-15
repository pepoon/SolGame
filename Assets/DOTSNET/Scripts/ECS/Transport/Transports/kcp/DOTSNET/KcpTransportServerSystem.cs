using System;
using System.Linq;
using Unity.Entities;
using UnityEngine;
using kcp2k;
using Unity.Collections;

namespace DOTSNET.kcp2k
{
    [ServerWorld]
    [AlwaysUpdateSystem]
    // use SelectiveSystemAuthoring to create it selectively
    [DisableAutoCreation]
    public class KcpTransportServerSystem : TransportServerSystem
    {
        // configuration
        public ushort Port = 7777;
        public bool DualMode = true;
        public bool NoDelay = true;
        public uint Interval = 10; // in milliseconds

        // advanced configuration
        public int FastResend = 0;
        public bool CongestionWindow = true; // KCP 'NoCongestionWindow' is false by default. here we negate it for ease of use.
        public uint SendWindowSize = 4096; // Kcp.WND_SND; 32 by default. DOTSNET sends a lot, so we need a lot more.
        public uint ReceiveWindowSize = 4096; // Kcp.WND_RCV; 128 by default. DOTSNET sends a lot, so we need a lot more.
        public int Timeout = 10000; // in milliseconds
        public bool NonAlloc = true;

        // debugging
        public bool debugLog;

        // kcp server
        internal KcpServer server;

        // all except WebGL
        public override bool Available() =>
            Application.platform != RuntimePlatform.WebGLPlayer;

        // MTU
        public override int GetMaxPacketSize(Channel channel)
        {
            switch (channel)
            {
                case Channel.Reliable:   return KcpConnection.ReliableMaxMessageSize;
                case Channel.Unreliable: return KcpConnection.UnreliableMaxMessageSize;
            }
            return 0;
        }

        public override bool IsActive() => server != null && server.IsActive();

        // IMPORTANT: use Start() to set everything up.
        //            - authoring configuration is only applied after OnCreate()
        //            - Early/LateUpdate will need to be in ServerActiveGroup,
        //              so OnStartRunning() would be called way after
        //              Server.Start() calls Transport.Start()
        public override void Start()
        {
            // only once
            if (IsActive()) return;

            // logging
            //   Log.Info should use Debug.Log if enabled, or nothing otherwise
            //   (don't want to spam the console on headless servers)
            if (debugLog)
                Log.Info = Debug.Log;
            else
                Log.Info = _ => {};
            Log.Warning = Debug.LogWarning;
            Log.Error = Debug.LogError;

            // server
            server = NonAlloc
                ? new KcpServerNonAlloc(
                    (connectionId) => OnConnected.Invoke(connectionId),
                    (connectionId, message) => OnData.Invoke(connectionId, ArraySegmentToNativeSlice(message, receiveConversionBuffer)),
                    (connectionId) => OnDisconnected.Invoke(connectionId),
                    DualMode,
                    NoDelay,
                    Interval,
                    FastResend,
                    CongestionWindow,
                    SendWindowSize,
                    ReceiveWindowSize,
                    Timeout)
                : new KcpServer(
                    (connectionId) => OnConnected.Invoke(connectionId),
                    (connectionId, message) => OnData.Invoke(connectionId, ArraySegmentToNativeSlice(message, receiveConversionBuffer)),
                    (connectionId) => OnDisconnected.Invoke(connectionId),
                    DualMode,
                    NoDelay,
                    Interval,
                    FastResend,
                    CongestionWindow,
                    SendWindowSize,
                    ReceiveWindowSize,
                    Timeout);

            Debug.Log("KCP server created");

            server.Start(Port);
        }

        // note: DOTSNET already packs messages. Transports don't need to.
        public override bool Send(int connectionId, NativeSlice<byte> slice, Channel channel)
        {
            // convert to NativeSlice while kcp still works with ArraySegment
            // TODO make kcp work with NativeSlice
            ArraySegment<byte> segment = NativeSliceToArraySegment(slice, sendConversionBuffer);

            // switch to kcp channel.
            // unreliable or reliable.
            // default to reliable just to be sure.
            switch (channel)
            {
                case Channel.Unreliable:
                    server.Send(connectionId, segment, KcpChannel.Unreliable);
                    break;
                default:
                    server.Send(connectionId, segment, KcpChannel.Reliable);
                    break;
            }
            OnSend?.Invoke(connectionId, slice); // statistics etc.
            return true;
        }

        public override void Disconnect(int connectionId) =>
            server?.Disconnect(connectionId);

        public override string GetAddress(int connectionId) =>
            server?.GetClientAddress(connectionId);

        public override void Stop()
        {
            server?.Stop();
            server = null;
        }

        // statistics
        public int GetAverageMaxSendRate() =>
            server.connections.Count > 0
                ? server.connections.Values.Sum(conn => (int)conn.MaxSendRate) / server.connections.Count
                : 0;
        public int GetAverageMaxReceiveRate() =>
            server.connections.Count > 0
                ? server.connections.Values.Sum(conn => (int)conn.MaxReceiveRate) / server.connections.Count
                : 0;
        public int GetTotalSendQueue() =>
            server.connections.Values.Sum(conn => conn.kcp.snd_queue.Count);
        public int GetTotalReceiveQueue() =>
            server.connections.Values.Sum(conn => conn.kcp.rcv_queue.Count);
        public int GetTotalSendBuffer() =>
            server.connections.Values.Sum(conn => conn.kcp.snd_buf.Count);
        public int GetTotalReceiveBuffer() =>
            server.connections.Values.Sum(conn => conn.kcp.rcv_buf.Count);

        // ECS /////////////////////////////////////////////////////////////////
        // process received in EarlyUpdate
        public override void EarlyUpdate() => server?.TickIncoming();

        // process outgoing in LateUpdate
        public override void LateUpdate() => server?.TickOutgoing();

        protected override void OnDestroy()
        {
            server = null;
            base.OnDestroy();
        }
    }
}