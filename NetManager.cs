using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using TopDownShooter.UI;
using UnityEngine.Networking;

namespace TopDownShooter.Net
{
    public class NetManager : MonoBehaviour
    {
        #region Singleton
        private static NetManager _instance;
        public static NetManager Instance { get { return _instance; } }

        private void Awake()
        {
            if (_instance != this && _instance != null)
            {
                Destroy(gameObject);
                return;
            }
            _instance = this;
            DontDestroyOnLoad(this);
        }
        #endregion

        public NetType netType = NetType.CLIENT;

        [Header("SERVER")]
        public string serverName;
        public Client client;
        public Server server;
        
        public void ChangeNetType()
        {
            netType = netType == NetType.CLIENT ? NetType.SERVER : NetType.CLIENT;
        }
        public void SetServerName()
        {
            serverName = string.IsNullOrEmpty(UIManager.Instance.serverName.text)
                ? "Default Server" : UIManager.Instance.serverName.text;
        }
        public void StartDemoServer()
        {
            if (UIManager.Instance.playerName.text == "")
            {
                UIManager.Instance.NotName();
                
                ChangeNetType();
                serverName = "";
                Invoke("ToNetScreen", 0.05f);
                return;
            }
            server.Create();
        }

        void ToNetScreen() {
            UIManager.Instance.netScreen.SetActive(true);
            UIManager.Instance.gameScreen.SetActive(false);
        }

        public void Disconnect()
        {
            if (netType == NetType.SERVER)
            {
                server.Disconnect();
                UDPManager.Instance.StartAsClient();
            }
            else
                client.Disconnect();
        }

    }

    public enum NetType
    {
        CLIENT,
        SERVER
    }
}
