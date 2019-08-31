using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using TopDownShooter.UI;

namespace TopDownShooter.Net
{
    public class UDPManager : NetworkDiscovery
    {
        private static UDPManager _instance;
        public static UDPManager Instance { get { return _instance; } }
        private void Awake()
        {
            if (_instance == null)
                _instance = this;
        }

        private NetManager nm;
        public bool connected = false;
        string FromAddress;
        string Data;

        #region CLIENT
        [HideInInspector]
        public Dictionary<string, ServerInfo> serverDict = new Dictionary<string, ServerInfo>();

        public void Start()
        {
            nm = NetManager.Instance;
            Initialize();
            StartAsClient();
        }

        private string GetIP(string fromAddress)
        {
            int startIndex = fromAddress.LastIndexOf(":") + 1;
            string ip = fromAddress.Substring(startIndex, fromAddress.Length - startIndex);
            return ip;
        }

        private ServerInfo GetInfo(string fromAddress, string data)
        {
            ServerInfo serverInfo = new ServerInfo();
            serverInfo = JsonUtility.FromJson<ServerInfo>(data);
            serverInfo.ip = GetIP(fromAddress);
            return serverInfo;
        }

        public void RefreshServers()
        {
            UIManager.Instance.ClearServerList();
            serverDict.Clear();
            Start();
            
           
        }

        public override void OnReceivedBroadcast(string fromAddress, string data)
        {
            FromAddress = fromAddress;
            Data = data;
            if (connected)
                return;
            try
            {
                ServerInfo serverInfo = GetInfo(fromAddress, data);
                if (serverDict == null)
                {
                    serverDict.Add(serverInfo.ip + ":" + serverInfo.port, serverInfo);
                    UIManager.Instance.AddServerToList(serverInfo);
                }
                if (!serverDict.ContainsKey(serverInfo.ip + "+" + serverInfo.port))
                {
                    serverDict.Add(serverInfo.ip + ":" + serverInfo.port, serverInfo);
                    UIManager.Instance.AddServerToList(serverInfo);
                }
            }
            catch
            {

            }

        }

        public void StartClient()
        {
            StopBroadcast();
            Initialize();
            StartAsClient();
        }
        #endregion

        #region SERVER
        public void StartServer()
        {
            
            StopBroadcast();
            Application.runInBackground = true;

            var config = new ConnectionConfig();
            config.AddChannel(QosType.ReliableFragmented);
            config.AddChannel(QosType.UnreliableFragmented);
            var ht = new HostTopology(config, 30);

            if (!NetworkServer.Configure(ht))
                return;

            if (EmptyPort())
            {
                Initialize();
                ServerInfo info = new ServerInfo();
                info.name = nm.serverName;
                info.port = port;
                info.ip = "";
                broadcastData = JsonUtility.ToJson(info);
                StartAsServer();
            }
        }

        private int minPort = 10000;
        private int maxPort = 10100;
        private int port;

        private bool EmptyPort()
        {
            for (int _port = minPort; _port <= maxPort; _port++)
            {
                if (NetworkServer.Listen(_port))
                {
                    port = _port;
                    return true;
                }
            }
            return false;
        }
        #endregion

    }
}
