using System;
using UnityEngine;

namespace DOTSNET.kcp2k
{
    public class KcpTransportClientAuthoring : MonoBehaviour, SelectiveSystemAuthoring
    {
        // find NetworkClientSystem in ECS world
        KcpTransportClientSystem client =>
            Bootstrap.ClientWorld.GetExistingSystem<KcpTransportClientSystem>();

        // common
        [Header("Configuration")]
        public ushort Port = 7777;
        [Tooltip("NoDelay is recommended to reduce latency. This also scales better without buffers getting full.")]
        public bool NoDelay = true;
        [Tooltip("KCP internal update interval. 100ms is KCP default, but a lower interval is recommended to minimize latency and to scale to more networked entities.")]
        public uint Interval = 10;
        [Tooltip("KCP timeout in milliseconds. Note that KCP sends a ping automatically.")]
        public int Timeout = 10000;

        [Header("Advanced")]
        [Tooltip("KCP fastresend parameter. Faster resend for the cost of higher bandwidth. 0 in normal mode, 2 in turbo mode.")]
        public int FastResend = 2;
        [Tooltip("KCP congestion window. Enabled in normal mode, disabled in turbo mode. Disable this for high scale games if connections get chocked regularly.")]
        public bool CongestionWindow = false; // KCP 'NoCongestionWindow' is false by default. here we negate it for ease of use.
        [Tooltip("KCP window size can be modified to support higher loads.")]
        public uint SendWindowSize = 4096; // Kcp.WND_SND; 32 by default. DOTSNET sends a lot, so we need a lot more.
        [Tooltip("KCP window size can be modified to support higher loads.")]
        public uint ReceiveWindowSize = 4096; // Kcp.WND_RCV; 128 by default. DOTSNET sends a lot, so we need a lot more.
        [Tooltip("Enable to use where-allocation NonAlloc KcpServer/Client/Connection versions. Highly recommended on all Unity platforms.")]
        public bool NonAlloc = true;

        // debugging
        [Header("Debug")]
        public bool debugLog;
        // show statistics in OnGUI
        public bool statisticsGUI;
        // log statistics for headless servers that can't show them in GUI
        public bool statisticsLog;

        // add to selectively created systems before Bootstrap is called
        public Type GetSystemType() => typeof(KcpTransportClientSystem);

        // apply configuration in awake
        // IMPORTANT: MonoBehaviour.Awake() happens AFTER System.OnCreate().
        void Awake()
        {
            client.debugLog = debugLog;
            client.Port = Port;
            client.NoDelay = NoDelay;
            client.Interval = Interval;
            client.FastResend = FastResend;
            client.CongestionWindow = CongestionWindow;
            client.SendWindowSize = SendWindowSize;
            client.ReceiveWindowSize = ReceiveWindowSize;
            client.Timeout = Timeout;
            client.NonAlloc = NonAlloc;

            if (statisticsLog)
                InvokeRepeating(nameof(OnLogStatistics), 1, 1);
        }

        void OnGUI()
        {
            if (!statisticsGUI) return;

            GUILayout.BeginArea(new Rect(15, 460, 220, 300));
            if (client.IsConnected())
            {
                GUILayout.BeginVertical("Box");
                GUILayout.Label("<b>KCP Client</b>");
                GUILayout.Label($"  MaxSendRate: {Utils.PrettyBytes(client.GetMaxSendRate())}/s");
                GUILayout.Label($"  MaxRecvRate: {Utils.PrettyBytes(client.GetMaxReceiveRate())}/s");
                GUILayout.Label($"  SendQueue: {client.GetSendQueueCount()}");
                GUILayout.Label($"  ReceiveQueue: {client.GetReceiveQueueCount()}");
                GUILayout.Label($"  SendBuffer: {client.GetSendBufferCount()}");
                GUILayout.Label($"  ReceiveBuffer: {client.GetReceiveBufferCount()}");
                GUILayout.EndVertical();
            }
            GUILayout.EndArea();
        }

        void OnLogStatistics()
        {
            if (client.IsConnected())
            {
                string log = "kcp CLIENT\n";
                log += $"  MaxSendRate: {Utils.PrettyBytes(client.GetMaxSendRate())}/s\n";
                log += $"  MaxRecvRate: {Utils.PrettyBytes(client.GetMaxReceiveRate())}/s\n";
                log += $"  SendQueue: {client.GetSendQueueCount()}\n";
                log += $"  ReceiveQueue: {client.GetReceiveQueueCount()}\n";
                log += $"  SendBuffer: {client.GetSendBufferCount()}\n";
                log += $"  ReceiveBuffer: {client.GetReceiveBufferCount()}\n\n";
                Debug.Log(log);
            }
        }
    }
}