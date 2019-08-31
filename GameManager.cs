using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TopDownShooter.UI;
using TopDownShooter.Net;

namespace TopDownShooter.Game
{
    public class GameManager : MonoBehaviour
    {
        #region Singleton
        private static GameManager _instance;
        public static GameManager Instance { get { return _instance; } }

        //public int time { get => time; set => time = value; }

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

        [Header("Player Setings")]
        public PlayerSettings playerSettings = new PlayerSettings();
        public int selfId;
        public Player playerPrefab;
        public PlayerCamera playerCamera;
        public GameObject PlayerParent;
        public bool[] IsBusyPosition = new bool[8];
        public bool IsGame;
        public int[] roleCount = new int[4];
        public float timeChoose = 20f;
        public int choose = 100;
        public int[] votes = new int[12];
        public bool IsTimer;

        public int GMtime;
        public bool[] time = new bool[4];
        private int mafChoose = 100;// это id
        private int manChoose = 100;
        private int codChoose = 100;

        public string[] names;

        public bool IsVoted = false;


        public void SetPlayerSettings(PlayerSettings playerSettings)
        {
            this.playerSettings = playerSettings;
        }

        public Dictionary<int, Player> players = new Dictionary<int, Player>();

        public GameObject[] PlayersGB;
        public GameObject[] ActivePlayersGB = new GameObject[12];
        public int[] ActivePlayersRoles = new int[13];

        public void AddPlayer(int id, string name)
        {

            PlayersGB[id].GetComponent<Player>().Init(id, name);
            players.Add(id, PlayersGB[id].GetComponent<Player>());




        }



        public void AddServerPlayer()
        {
            PlayersGB[0].GetComponent<Player>().Init(0, UIManager.Instance.playerName.text);
            UIManager.Instance.StopSearchPlayersAndStartGameButton.SetActive(true);

            selfId = 0;

            IsBusyPosition[0] = true;
        }

        public void RemovePlayer(int id)
        {
            while (players.ContainsKey(id))
            {
                PlayersGB[id].SetActive(false);
                players.Remove(id);
            }
        }

        public void RemovePlayerOfVoting(int id)
        {
            PlayersGB[id].SetActive(false);

        }

        public void OnStopSearchPlayersAndStartGameButton()
        {

            int TestCount = 0;
            int KondCount = 0;
            int MeneCount = 0;
            int k = 0;
            int[] usedPlayrs = { 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12, 12 };

            IsGame = true;

            for (int i = 0; i < 12; i++)
            {
                if (PlayersGB[i].activeSelf)
                    k += 1;



            }

            ActivePlayersGB = new GameObject[k];

            for (int i = 0; i < 12; i++)
            {
                if (PlayersGB[i].activeSelf)
                    ActivePlayersGB[i] = PlayersGB[i];


            }

            switch (ActivePlayersGB.Length)
            {
                case 1:

                    TestCount = 0;
                    KondCount = 0;
                    MeneCount = 0;
                    break;

                case 2:

                    TestCount = 1;
                    KondCount = 0;
                    MeneCount = 0;
                    break;

                case 3:

                    TestCount = 1;
                    KondCount = 0;
                    MeneCount = 0;
                    break;

                case 4:

                    TestCount = 1;
                    KondCount = 0;
                    MeneCount = 0;
                    break;

                case 5:

                    TestCount = 1;
                    KondCount = 1;
                    MeneCount = 0;
                    break;

                case 6:

                    TestCount = 1;
                    KondCount = 1;
                    MeneCount = 1;
                    break;

                case 7:

                    TestCount = 1;
                    KondCount = 1;
                    MeneCount = 1;
                    break;

                case 8:

                    TestCount = 1;
                    KondCount = 1;
                    MeneCount = 1;
                    break;
            }

            roleCount[0] = ActivePlayersGB.Length - TestCount - KondCount - MeneCount;
            roleCount[1] = TestCount;
            roleCount[2] = KondCount;
            roleCount[3] = MeneCount;




            SetRole(1, TestCount, usedPlayrs);
            SetRole(2, KondCount, usedPlayrs);
            SetRole(3, MeneCount, usedPlayrs);

            ActivePlayersRoles = new int[ActivePlayersGB.Length];
            for (int i = 0; i < ActivePlayersGB.Length; i++)
            {
                ActivePlayersRoles[i] = ActivePlayersGB[i].GetComponent<Player>().role;
            }


            UIManager.Instance.SetPlayerImage(PlayersGB[selfId]);

            time[1] = true;

            Invoke("StepUI", 2f);
            IsTimer = true;
            UIManager.Instance.TimerText.SetActive(true);
            names = new string[ActivePlayersGB.Length];

            for(int i = 0; i < ActivePlayersGB.Length; i++)
            {
                names[i] = ActivePlayersGB[i].GetComponent<Player>().name;
            }
            NetManager.Instance.server.SendNames(names);
        }

        //0 - программист мир playbutton
        //1 - тестер мафия settingsbutton
        //2 - кодъвьер лекарь quitbutton
        //3 - менеджер комиссар pausebutton

        int SetRandom(int[] usedPlayrs)
        {
            int r = Mathf.RoundToInt(Random.Range(0f, ActivePlayersGB.Length - 1));//gg
            for (int i = 0; i < usedPlayrs.Length; i++)
            {
                if (usedPlayrs[i] == r) return SetRandom(usedPlayrs);
            }
            return r;
        }


        public void SetRole(int role, int count, int[] usedPlayrs)
        {
            for (int i = 0; i < count; i++)
            {
                int r = SetRandom(usedPlayrs);
                Debug.Log(r);
                ActivePlayersGB[r].GetComponent<Player>().role = role;


            }
        }



        public int chesenId()
        {
            int id = 0;

            for (int i = 1; i < votes.Length; i++)
            {
                if (votes[i] > votes[id])
                    id = i;
            }

            return id;
        }

        private void Update()
        {
            if (IsGame)//проверка на начало игры
            {
                //Debug.Log(roleCount);
                if (((roleCount[0] != 0 || roleCount[2] != 0) || roleCount[3] != 0) && roleCount[1] != 0)
                {
                    if (timeChoose >= 0 && IsTimer)
                    {
                        timeChoose -= UnityEngine.Time.deltaTime;
                        UIManager.Instance.TimerText.GetComponent<Text>().text = "" + Mathf.RoundToInt(timeChoose);

                        if (time[1] && PlayersGB[selfId].GetComponent<Player>().role == 1 )
                        {
                            UIManager.Instance.OpenCards(1);
                            UIManager.Instance.Light(time);

                        }

                        if (time[2] && PlayersGB[selfId].GetComponent<Player>().role == 2)
                        {
                            UIManager.Instance.OpenCards(2);
                            UIManager.Instance.Light(time);

                        }
                        if (time[3] && PlayersGB[selfId].GetComponent<Player>().role == 3)
                        {
                            UIManager.Instance.OpenCards(3);
                            UIManager.Instance.Light(time);

                        }
                        if (time[0])
                        {

                            if (mafChoose != 100)
                            {
                                RemovePlayerOfVoting(mafChoose);
                                roleCount[PlayersGB[mafChoose].GetComponent<Player>().role]--;
                            }

                            if (manChoose != 100)
                            {
                                RemovePlayerOfVoting(manChoose);
                                roleCount[PlayersGB[manChoose].GetComponent<Player>().role]--;
                            }

                            //if (codChoose!=100){ надпись вылечели PlayersGB[codChoose].GetComponent<Player>().name}
                            UIManager.Instance.SetAllCardsAnonim();
                            UIManager.Instance.Light(time);

                        }

                        if (timeChoose <= 0)
                        {
                            if (time[1] && choose != 100)
                            {
                                mafChoose = choose;
                            }

                            if (time[2] && choose != 100)
                            {
                                manChoose = choose;
                                PlayersGB[choose].SetActive(true);
                            }

                            if (time[3] && choose != 100)
                            {
                                codChoose = choose;
                                if (codChoose == manChoose)
                                {
                                    manChoose = 100;
                                }
                                else if (codChoose == mafChoose)
                                {
                                    mafChoose = 100;
                                }
                                RemovePlayerOfVoting(choose);
                            }

                            if (time[0])
                            {
                               roleCount[PlayersGB[chesenId()].GetComponent<Player>().role]--;
                               RemovePlayerOfVoting(chesenId());
                            }

                            UIManager.Instance.SetAllCardsAnonim();
                            for (int i = 0; i < time.Length; i++)
                            {
                                if (time[i])
                                {
                                    time[i] = false;
                                    if (i + 1 == 4)
                                    {
                                        time[0] = true;
                                    }
                                    else if (i + 1 == 3 && roleCount[3] == 0)
                                    {
                                        time[0] = true;
                                    }
                                    else if (i + 1 == 2 && roleCount[2] == 0)
                                    {
                                        if (roleCount[3] == 0)
                                        {
                                            time[0] = true;
                                        }
                                        else
                                        {
                                            time[3] = true;
                                        }

                                    }
                                    else
                                    {
                                        time[i + 1] = true;
                                    }

                                    break;
                                }
                            }

                            UIManager.Instance.Step(time);
                            UIManager.Instance.AllPlayersLightOff();
                            IsTimer = true;
                            timeChoose = 20f;
                            choose = 100;
                            votes = new int[12];
                            IsVoted = false;
                            for(int i = 0; i < ActivePlayersGB.Length; i++)
                            {
                                PlayersGB[i].transform.GetChild(4).GetComponent<Text>().text = "";
                            }

                        }
                    }
                    else
                    {
                        timeChoose = 20f;
                    }
                }
                else
                {
                    string whowin;
                    if (roleCount[1] == 0) whowin = "  Программисты";
                    else whowin = "  Тестеровщики";
                    UIManager.Instance.gameoverScreen.SetActive(true);
                    UIManager.Instance.GameOver(names, ActivePlayersRoles, whowin);
                    
                    IsTimer = false;
                }

            }


        }


        void StepUI()
        {
            UIManager.Instance.Step(time);
        }

        public void ResetData()
        {
            IsTimer = false;
            timeChoose = 20f;
            choose = 100;
            mafChoose = 100;
        manChoose = 100;
        codChoose = 100;
            votes = new int[12];
            names = new string[12];
            IsVoted = false;
            time = new bool[4];
            //UIManager.Instance.SetAllCardsAnonim();
            IsBusyPosition = new bool[8];
             ActivePlayersGB = new GameObject[12];
         ActivePlayersRoles = new int[13];

    }
    }
}
