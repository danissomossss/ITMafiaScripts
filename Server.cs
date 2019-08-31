using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TopDownShooter.Game;
using TopDownShooter.UI;

namespace TopDownShooter.Net {
    public class Server : MonoBehaviour {
        public GameObject[] k;
        public void Create()
        {
            
            RegisterHandlers();
            UDPManager.Instance.StartServer();
            GameManager.Instance.AddServerPlayer();
        }

        private void RegisterHandlers()
        {
            NetworkServer.RegisterHandler(MsgType.Connect, OnClientConnected);
            NetworkServer.RegisterHandler(MessageType.INFO, OnClientInfo);
            NetworkServer.RegisterHandler(MessageType.TRANSFORM, OnClientTransform);
            NetworkServer.RegisterHandler(MsgType.Disconnect, OnClientDisconnect);
            NetworkServer.RegisterHandler(MessageType.VOTINGCHOOSE, OnClientVotingChoose);
            NetworkServer.RegisterHandler(MessageType.CHOOSE, OnClientChoose);
        }

        //При подключении клиента на сервер
        private void OnClientConnected(NetworkMessage nmsg)
        {

            
            UserInfo info = new UserInfo() { id = nmsg.conn.connectionId, IsBusyPosition = GameManager.Instance.IsBusyPosition, IsGame = GameManager.Instance.IsGame };
            NetworkServer.SendToClient(nmsg.conn.connectionId, MessageType.INFO, info);
            
        }
        //Создание персонажа клиента по данным
        private void OnClientInfo(NetworkMessage nmsg)
        {
            var user = nmsg.ReadMessage<UserInfo>();
            GameManager.Instance.IsBusyPosition = user.IsBusyPosition;
            //GameManager.Instance.AddPlayer(user.id,  user.name);
            var spawn = new UserInfo()
            {
                id = user.id,
				IsBusyPosition = user.IsBusyPosition,
                name = user.name
                
            };
            NetworkServer.SendToClient(user.id, MessageType.SPAWN, spawn);
        }
        //При получении данных по позиции и повороту от клиента
        private void OnClientTransform(NetworkMessage nmsg)
        {
            var info = nmsg.ReadMessage<UserInfo>();
            GameManager gm = GameManager.Instance;
            gm.IsBusyPosition = info.IsBusyPosition;
            if (!gm.players.ContainsKey(info.id))
                gm.AddPlayer(info.id, info.name);
            

           
            
            NetworkServer.SendToAll(MessageType.TRANSFORM, info);
        }

        private void OnClientVotingChoose(NetworkMessage nmsg)
        {
            var info = nmsg.ReadMessage<UserInfo>();
            GameManager gm = GameManager.Instance;
            
            

            if (info.choose != 100 && gm.time[0])
            {
                gm.votes[info.choose] += 1;
            }
            GameManager.Instance.PlayersGB[info.id].transform.GetChild(4).GetComponent<Text>().text = GameManager.Instance.PlayersGB[info.choose].GetComponent<Player>().name;
            NetworkServer.SendToAll(MessageType.VOTINGCHOOSE, info);
        }

        private void OnClientChoose(NetworkMessage nmsg)
        
        {
            var info = nmsg.ReadMessage<UserInfo>();
            GameManager gm = GameManager.Instance;


            if (info.choose != 100 && !gm.time[0])
                gm.choose = info.choose;

           
                NetworkServer.SendToAll(MessageType.CHOOSE, info);
            }

        //При дисконнекте клиента
        private void OnClientDisconnect(NetworkMessage nmsg)
        {
            var _id = nmsg.conn.connectionId;
            GameManager.Instance.RemovePlayer(_id);

            NetworkServer.SendToAll(MessageType.DISCONNECT, new UserInfo() { id = _id});
        }

        public void SendTransform(int _id, string Name, bool[] IsBusy, bool[] _time  )
        {
            UserInfo info = new UserInfo()
            {
                id = _id,

                IsBusyPosition = IsBusy,
                name = Name,
                ActivePlayersRoles = GameManager.Instance.ActivePlayersRoles,
                IsGame = GameManager.Instance.IsGame,
                time = _time,
                roleCount = GameManager.Instance.roleCount
           

            };
            NetworkServer.SendToAll(MessageType.TRANSFORM, info);
        }

        public void SendVotes0(int _id, int _choose)
        {

            UserInfo info = new UserInfo()
            {
                id = _id,
                choose = _choose,
                ActivePlayersRoles = GameManager.Instance.ActivePlayersRoles,
               
                

            };

            NetworkServer.SendToAll(MessageType.VOTINGCHOOSE, info);

        }

        public void SendNames(string[] _names)
        {
            UserInfo info = new UserInfo()
            {
                
                names = _names


            };

            NetworkServer.SendToAll(MessageType.NAMES, info);
        }

        public void SendVotesNot0()
        {

            UserInfo info = new UserInfo()
            {

                choose = GameManager.Instance.choose,
                ActivePlayersRoles = GameManager.Instance.ActivePlayersRoles,
                

            };

            NetworkServer.SendToAll(MessageType.CHOOSE, info);

        }

        public void Disconnect()
        {
            UDPManager.Instance.connected = false;
            foreach(Player player in GameManager.Instance.players.Values)
            {
                player.gameObject.SetActive(false);
            }
            for (int i = 0; i < GameManager.Instance.PlayersGB.Length; i++)
            {
                if (GameManager.Instance.PlayersGB[i].activeSelf)
                    GameManager.Instance.PlayersGB[i].transform.GetChild(2).GetComponent<Image>().sprite = UIManager.Instance.Anonim;
            }
            GameManager.Instance.IsBusyPosition = new bool[8];
            GameManager.Instance.ActivePlayersGB = new GameObject[8];
            GameManager.Instance.ActivePlayersRoles = new int[13];
            GameManager.Instance.IsGame = false;
            GameManager.Instance.players.Clear();
            NetManager.Instance.ChangeNetType();
            NetworkServer.DisconnectAll();
            NetworkServer.Shutdown();
            UIManager.Instance.gameScreen.SetActive(false);
            UIManager.Instance.netScreen.SetActive(true);
            UDPManager.Instance.Start(); 
            UIManager.Instance.ClearServerList();
            GameManager.Instance.ResetData();
             
        }

        public void OnApplicationQuit()
        {
            if (NetManager.Instance.netType == NetType.CLIENT)
                return;
            if (NetworkServer.active)
            {
                NetworkServer.DisconnectAll();
                NetworkServer.Shutdown();
            }
            
        }
    }
}
