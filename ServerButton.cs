using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TopDownShooter.Net;

namespace TopDownShooter.UI
{
    public class ServerButton : MonoBehaviour, IPointerClickHandler
    {

        public Text serverName;
        private ServerInfo serverInfo;

        public void Init(ServerInfo serverInfo)
        {
            this.serverInfo = serverInfo;
            serverName.text = serverInfo.name;
        }

        public void OnPointerClick(PointerEventData eventData)
        {
            
            NetManager.Instance.client.StartClient(serverInfo.ip, serverInfo.port);
            //UIManager.Instance.gameScreen.SetActive(true);
            //UIManager.Instance.netScreen.SetActive(false);
        }
    }
}
