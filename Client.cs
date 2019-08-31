using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using TopDownShooter.Game;
using TopDownShooter.UI;

namespace TopDownShooter.Net {
    public class Client : MonoBehaviour {

        NetworkClient client;
        bool doThis = true;

        public void StartClient(string ip, int port)
        {
            if(UIManager.Instance.playerName.text == "")
            {
                UIManager.Instance.NotName();
                return;
            }
            var config = new ConnectionConfig();
            config.AddChannel(QosType.ReliableFragmented);
            config.AddChannel(QosType.UnreliableFragmented);
            client = new NetworkClient();
            client.Configure(config, 1);
            RegisterHandlers();
            client.Connect(ip, port);
        }

        private void RegisterHandlers()
        {
            client.RegisterHandler(MsgType.Connect, OnConnected);
            client.RegisterHandler(MessageType.INFO, OnInfo);
            client.RegisterHandler(MessageType.SPAWN, OnSpawn);
            client.RegisterHandler(MessageType.TRANSFORM, OnTransform);
            client.RegisterHandler(MessageType.DISCONNECT, OnAnyDisconnect);
            client.RegisterHandler(MessageType.VOTINGCHOOSE, OnGetClientVotingChoose);
            client.RegisterHandler(MessageType.CHOOSE, OnGetChoose);
            client.RegisterHandler(MessageType.NAMES, OnGetNames);
            client.RegisterHandler(MsgType.Disconnect, OnDisconnect);
        }

        private void OnConnected(NetworkMessage nmsg)
        {
            UDPManager.Instance.connected = true;
        }
        private void OnInfo(NetworkMessage nmsg)
        {
            var user = nmsg.ReadMessage<UserInfo>();
            if(user.IsGame)
            {
                UIManager.Instance.NotConnectedText.SetActive(true);
            Disconnect();
            
            }
            else
            {
                
            UIManager.Instance.gameScreen.SetActive(true);
            UIManager.Instance.netScreen.SetActive(false);
			
            GameManager.Instance.selfId = user.id;
			GameManager.Instance.IsBusyPosition = user.IsBusyPosition;
            
           
			for(int i = 0; i < 8; i++)
			{
				if(GameManager.Instance.IsBusyPosition[i] == false)
				{
					GameManager.Instance.IsBusyPosition[i] = true;
					//GameManager.Instance.AddPlayer(user.id, user.name);
					
					i = 90;
				}
			}
            UserInfo info = new UserInfo()
            {
                id = user.id,
                name = UIManager.Instance.playerName.text,
                IsBusyPosition = GameManager.Instance.IsBusyPosition
			   	
                
            };
            client.Send(MessageType.INFO, info);
            }
        }
        private void OnSpawn(NetworkMessage nmsg)
        {
            var user = nmsg.ReadMessage<UserInfo>();
            GameManager gm = GameManager.Instance;
            GameManager.Instance.IsBusyPosition = user.IsBusyPosition;
            gm.AddPlayer(user.id, user.name);
        }
        private void OnTransform(NetworkMessage nmsg)
        {
            var info = nmsg.ReadMessage<UserInfo>();
            GameManager gm = GameManager.Instance;
            gm.IsBusyPosition = info.IsBusyPosition;

            for (int i = 0; i<info.roleCount.Length; i++){
                gm.roleCount[i] = info.roleCount[i];

            }

            if(info.IsGame)
            {
            gm.ActivePlayersRoles = info.ActivePlayersRoles;
            gm.IsGame = info.IsGame;

            for(int i = 0; i < gm.ActivePlayersGB.Length - 1; i++)
            {
                if(gm.PlayersGB[i].activeSelf)
                gm.PlayersGB[i].GetComponent<Player>().role = gm.ActivePlayersRoles[i];
            }
            
            UIManager.Instance.SetPlayerImage(gm.PlayersGB[gm.selfId]);
            }

            if (!gm.players.ContainsKey(info.id))
                gm.AddPlayer(info.id, info.name);
            

            
            
            if(doThis && info.IsGame)
            {
                doThis = false;
                int k=0;
                 for(int i = 0; i < info.time.Length; i++)
                 {
                     gm.time[i] = info.time[i];
                 }
                UIManager.Instance.Step(gm.time);
                gm.IsTimer = true;
                
                for (int i = 0; i < 12; i++)
                {
                    if (gm.PlayersGB[i].activeSelf)
                        k += 1;



                }

                UIManager.Instance.TimerText.SetActive(true);
                gm.ActivePlayersGB = new GameObject[k];

                for (int i = 0; i < 12; i++)
                {
                    if (gm.PlayersGB[i].activeSelf)
                        gm.ActivePlayersGB[i] = gm.PlayersGB[i];
                }
            }

           

        }

        private void OnGetClientVotingChoose(NetworkMessage nmsg)
        {
            var info = nmsg.ReadMessage<UserInfo>();
            GameManager gm = GameManager.Instance;



            if (info.choose != 100 && gm.time[0])
            {
                gm.votes[info.choose] += 1;
            }
           
            
            GameManager.Instance.ActivePlayersGB[info.id].transform.GetChild(4).GetComponent<Text>().text = GameManager.Instance.ActivePlayersGB[info.choose].GetComponent<Player>().name;
        }

        private void OnGetChoose(NetworkMessage nmsg)
        {
            var info = nmsg.ReadMessage<UserInfo>();
            GameManager gm = GameManager.Instance;


            if (info.choose != 100 && !gm.time[0])
                gm.choose = info.choose;

           

        }

        private void OnAnyDisconnect(NetworkMessage nmsg)
        {
            UserInfo info = nmsg.ReadMessage<UserInfo>();
            GameManager.Instance.RemovePlayer(info.id);
        }
        private void OnDisconnect(NetworkMessage nmsg)
        {
            Disconnect();
        }

        private void OnGetNames(NetworkMessage nmsg)
        {
            var info = nmsg.ReadMessage<UserInfo>();
            GameManager gm = GameManager.Instance;

            gm.names = new string[info.names.Length];
            for (int i = 0; i < info.names.Length; i++)
            {
                gm.names[i] = info.names[i];
            }
        }

        public void SendTransform(int _id,string Name, bool[] IsBusy)
        {
            GameManager.Instance.IsBusyPosition = IsBusy;
            UserInfo info = new UserInfo()
            {
                id = _id,
                name = Name,
                IsBusyPosition = IsBusy,
                choose = GameManager.Instance.choose
                
            };
            
            client.Send(MessageType.TRANSFORM, info);

        }

        public void SendVotes0(int _id, int _choose)
        {
            
            UserInfo info = new UserInfo()
            {
                id = _id,
                choose = _choose

            };

            client.Send(MessageType.VOTINGCHOOSE, info);

        }

        public void SendVotesNot0()
        {

            UserInfo info = new UserInfo()
            {
                id = GameManager.Instance.selfId,
                choose = GameManager.Instance.choose

            };

            client.Send(MessageType.CHOOSE, info);

        }
        public void StopConnection()
        {
            UserInfo info = new UserInfo()
            {
                id = GameManager.Instance.selfId
            };
            client.Send(MessageType.DISCONNECT, info);
            client.Disconnect();
            GameManager.Instance.players.Clear();
            UIManager.Instance.gameScreen.SetActive(false);
            UIManager.Instance.netScreen.SetActive(true);  
            UDPManager.Instance.Start();  
        }

        public void Disconnect()
        {
            UDPManager.Instance.connected = false;
            foreach (Player player in GameManager.Instance.players.Values)
            {
                player.gameObject.SetActive(false);
            }

            for(int i = 0; i < GameManager.Instance.PlayersGB.Length; i++)
            {
                if (GameManager.Instance.PlayersGB[i].activeSelf)
                    GameManager.Instance.PlayersGB[i].transform.GetChild(2).GetComponent<Image>().sprite = UIManager.Instance.Anonim;
            }
            GameManager.Instance.IsBusyPosition = new bool[8];
            GameManager.Instance.ActivePlayersGB = new GameObject[8];
            GameManager.Instance.ActivePlayersRoles = new int[13];
            GameManager.Instance.IsGame = false;
            GameManager.Instance.players.Clear();
            if (UDPManager.Instance.connected) Disconnect();
            UIManager.Instance.gameScreen.SetActive(false);
            UIManager.Instance.netScreen.SetActive(true);
            UDPManager.Instance.Start();
            UIManager.Instance.ClearServerList();
            GameManager.Instance.ResetData();

        }

        public void OnApplicationQuit()
        {
            if (NetManager.Instance.netType == NetType.SERVER)
                return;
           
                if(UDPManager.Instance.connected)Disconnect();
        }

        


    }
}
