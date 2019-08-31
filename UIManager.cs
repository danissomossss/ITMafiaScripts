using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TopDownShooter.Game;
using TopDownShooter.Net;

namespace TopDownShooter.UI
{
    public class UIManager : MonoBehaviour
    {
        #region Singleton
        private static UIManager _instance;
        public static UIManager Instance { get { return _instance; } }

        private void Awake()
        {
            if (_instance == null)
                _instance = this;
        }
        #endregion

        [Header("PlayerSettings")]
        public InputField playerName;
        public Image headColorImage;
        public Image bodyColorImage;
        [Header("UI Sound Settings")]
        public AudioClip buttonSound;
        public AudioSource audioSource;
        [Header("Server Settings")]
        public InputField serverName;
        [Header("Server List")]
        public ServerButton serverButtonPrefab;
        public RectTransform contentRectTransform;
        private float contentLength;
        public float borderSize = -15;
        public float contentSize = -45;
        [Header("Joysticks")]
        public Joystick moveJoystick;
        public Joystick rotationJoystick;
        [Header("Screens")]
        public GameObject netScreen;
        public GameObject gameScreen;
        public GameObject StopSearchPlayersAndStartGameButton;
        public GameObject NotConnectedText;
        private bool Technic;

        public GameObject DayOnText;
        public GameObject TestersTurnText;
        public GameObject KodviverTurnText;
        public GameObject MenedjerTurnText;
        public GameObject VotingText;
        public GameObject DayOffText;

        public Sprite Programer;
        public Sprite Tester;
        public Sprite Kodvirer;
        public Sprite Menedjer;
        public Sprite Anonim;

        public GameObject gameoverScreen;

        public GameObject[] WhoWasWho;
        public GameObject WhoWinText;

        
        public GameObject textArea;
        public GameObject textArea2;



        public GameObject TimerText;

        public GameObject NotNameText;
        

        public void SetColorImage(Image image, Color color)
        {
            image.color = color;
        }

        public void OpenURL(string url)
        {
            Application.OpenURL(url);
        }

        public void Quit()
        {
            
            Application.Quit();
        }
        
        public void LoadServerName()
        {
            if (!PlayerPrefs.HasKey("serverName"))
                return;
            serverName.text = PlayerPrefs.GetString("serverName");
        }
        public void SaveServerName()
        {
            PlayerPrefs.SetString("serverName", serverName.text);
        }

        public void LoadPlayerSettings()
        {
            if (!PlayerPrefs.HasKey("playerSettings"))
                return;
            var playerSettings = JsonUtility.FromJson<PlayerSettings>(PlayerPrefs.GetString("playerSettings"));
            playerName.text = playerSettings.name;
            
        }
        public void SavePlayerSettings()
        {
            PlayerSettings playerSettings = new PlayerSettings
            {
                name = string.IsNullOrEmpty(playerName.text) ? "Player" : playerName.text,
                
            };
            PlayerPrefs.SetString("playerSettings", JsonUtility.ToJson(playerSettings));
        }

        public void SetPlayerSettings()
        {
            GameManager.Instance.SetPlayerSettings(new PlayerSettings
            {
                name = string.IsNullOrEmpty(playerName.text) ? "Player" : playerName.text
                
            });
        }

        public void AddServerToList(ServerInfo info)
        {
            contentLength += borderSize;
            ServerButton server = Instantiate(serverButtonPrefab, contentRectTransform.transform);
            server.Init(info);
            server.GetComponent<RectTransform>().anchoredPosition = new Vector2(0, contentLength);
            contentLength += contentSize;
            contentRectTransform.sizeDelta = new Vector2(contentRectTransform.sizeDelta.x, Mathf.Abs(contentLength + borderSize));
        }

        public void ClearServerList()
        {
            contentLength = 0;
            foreach (Transform child in contentRectTransform.transform)
            {
                Destroy(child.gameObject);
            }
        }


        private void Start()
        {
            LoadPlayerSettings();
            LoadServerName();
        }

        private void Update()
        {
            if (Input.GetKeyDown(KeyCode.Y))
                PlayerPrefs.DeleteAll();

                if(NotConnectedText.activeSelf && !Technic)
                {
                    Technic = true;
                    Invoke("SetOffNotConnectedText",2f);
                }
        }

        public void OnDisconnectButton()
        {
            if (NetManager.Instance.netType == NetType.SERVER)
            NetManager.Instance.server.Disconnect();
            else
            NetManager.Instance.client.Disconnect();
        }

        private void SetOffNotConnectedText ()
        {
            NotConnectedText.SetActive(false);
            Technic = false;
        }

        public void SetPlayerImage(GameObject player)
        {
            switch (player.GetComponent<Player>().role)
            {
                
                case 0:
                player.transform.GetChild(2).GetComponent<Image>().sprite = Programer;
                   // player.transform.GetChild(2).GetComponent<RectTransform>().localScale = new Vector3(1.6f, 1.8f, 1);
                    break;

                case 1:
                player.transform.GetChild(2).GetComponent<Image>().sprite = Tester;
                 //   player.transform.GetChild(2).GetComponent<RectTransform>().localScale = new Vector3(0.6f, 1, 1);
                break;

                case 2:
                player.transform.GetChild(2).GetComponent<Image>().sprite = Kodvirer;
                 //   player.transform.GetChild(2).GetComponent<RectTransform>().localScale = new Vector3(1.6f, 1.8f, 1);
                    break;

                case 3:
                player.transform.GetChild(2).GetComponent<Image>().sprite = Menedjer;
                  //  player.transform.GetChild(2).GetComponent<RectTransform>().localScale = new Vector3(1.6f, 1.8f, 1);
                    break;
            }
        }

        public void OpenCards(int role)
        {
            Sprite spritesprite = Anonim;
            switch(role)
            {
                case 0:
                spritesprite = Programer;
                break;
                case 1:
                spritesprite = Tester;
                break;
                case 2:
                spritesprite = Kodvirer;
                break;
                case 3:
                spritesprite = Menedjer;
                break;

            }


            for(int i = 0; i < GameManager.Instance.ActivePlayersGB.Length; i++)
            {
            if(GameManager.Instance.ActivePlayersGB[i].GetComponent<Player>().role == role) 
            {GameManager.Instance.ActivePlayersGB[i].transform.GetChild(2).GetComponent<Image>().sprite = spritesprite;}
            // GameManager.Instance.ActivePlayersGB[i].transform.GetChild(2).GetComponent<RectTransform>().localScale = new Vector3(1.6f, 1.8f, 1);
          //  if(spritesprite == Tester) {GameManager.Instance.ActivePlayersGB[i].transform.GetChild(2).GetComponent<RectTransform>().localScale  = new Vector3(0.6f, 1, 1);}


               
            }
        }

        public void SetAllCardsAnonim()
        {
            for(int i = 0; i < GameManager.Instance.ActivePlayersGB.Length; i++)
            {
                if (i != GameManager.Instance.selfId)
                {
                    GameManager.Instance.ActivePlayersGB[i].transform.GetChild(2).GetComponent<Image>().sprite = Anonim;
                   // GameManager.Instance.ActivePlayersGB[i].transform.GetChild(2).GetComponent<RectTransform>().localScale = new Vector3(1.6f, 1.8f, 1);
                }
                }
            
        }

        public void SetNight()
        {
            GameObject.Find("BG").GetComponent<Animation>().Play("ToNight");
            DayOffText.SetActive(true);
            Invoke("DayOffTextOff", 2f);
        }

        public void SetDay()
        {
            GameObject.Find("BG").GetComponent<Animation>().Play("ToDay");
            DayOnText.SetActive(true);
            Invoke("DayOnTextOff", 2f);

        }

        void DayOffTextOff()
        {
            DayOffText.SetActive(false);
        }

        void DayOnTextOff()
        {
            DayOnText.SetActive(false);
        }

        

        public void Step(bool[] time)
        {
                if(time[0])
                {
                SetDay();
                Invoke("VotingTextOn",2f);
                }
                

                if(time[1])
                {
                SetNight();
                Invoke("TestersTurnTextOn",2f);
                }
              

                if(time[2])
                {
                KodviverTurnText.SetActive(true);
                Invoke("KodviverTurnextOff",2f);
                }
               

               if(time[3])
               {
                MenedjerTurnText.SetActive(true);
                Invoke("MenedjerTurnOffText",2f);
               }
              
            
    


        }

        void VotingTextOn()
        {
            VotingText.SetActive(true);
            Invoke("VotingTextOff",2f);
        }

        void TestersTurnTextOn()
        {
            TestersTurnText.SetActive(true);
            Invoke("TestersTurnTextOff",2f);
        }

        void VotingTextOff()
        {
            VotingText.SetActive(false);
        }

        void TestersTurnTextOff()
        {
            TestersTurnText.SetActive(false);
        }

        void KodviverTurnextOff()
        {
            KodviverTurnText.SetActive(false);
        }

        void MenedjerTurnOffText()
        {
            MenedjerTurnText.SetActive(false);
        }

        public void NotName()
        {
            NotNameText.SetActive(true);
            Invoke("NotNameOff",1f);
        }

        void NotNameOff()
        {
            NotNameText.SetActive(false);
        }

        public void progText()
        {
            textArea.GetComponent<Text>().text = "Программист - это специалист, создающий исходный код" +
            " для программы. Такой программой может быть операционная система компьютера," +
            " видеоигра, web или мобильное приложение и даже алгоритм работы микроволновки." +
            " Программный код пишется на специальном языке программирования. Его выбирают" +
            " исходя из специфики продукта.";
        }

       

        public void testText()
        {
            textArea.GetComponent<Text>().text = "Тестировщик — это специалист, который занимается" +
            " тестированием программного обеспечения (ПО) с целью выявления ошибок в его работе и их" +
            " последующего исправления.";
        }

        public void meneText()
        {
            textArea.GetComponent<Text>().text = "IT-менеджер компании – сотрудник, управляющий" +
            " информационными процессами. Он разбирается в технических аспектах IT-среды," +
            " в вопросах ее взаимодействия с другими сферами: финансовой, кадровой, рыночной." +
            "  Задача менеджера внутри фирмы: знать цели развития бизнеса компании," +
            " уметь представить бизнес-процессы для их автоматизации.";
        }

        public void codReText()
        {
            textArea.GetComponent<Text>().text = "Code review (код ревью) - инженерная практика"+
            " в терминах гибкой методологии разработки. Это анализ (инспекция) кода с целью"+
            " выявить ошибки, недочеты, расхождения в стиле написания кода, в соответствии"+
            " написанного кода и поставленной задачи.";
        }

        public void progText2()
        {
            textArea2.GetComponent<Text>().text = "Программист - это специалист, создающий исходный код" +
            " для программы. Такой программой может быть операционная система компьютера," +
            " видеоигра, web или мобильное приложение и даже алгоритм работы микроволновки." +
            " Программный код пишется на специальном языке программирования. Его выбирают" +
            " исходя из специфики продукта.";
        }

       

        public void testText2()
        {
            textArea2.GetComponent<Text>().text = "Тестировщик — это специалист, который занимается" +
            " тестированием программного обеспечения (ПО) с целью выявления ошибок в его работе и их" +
            " последующего исправления.";
        }

        public void meneText2()
        {
            textArea2.GetComponent<Text>().text = "IT-менеджер компании – сотрудник, управляющий" +
            " информационными процессами. Он разбирается в технических аспектах IT-среды," +
            " в вопросах ее взаимодействия с другими сферами: финансовой, кадровой, рыночной." +
            "  Задача менеджера внутри фирмы: знать цели развития бизнеса компании," +
            " уметь представить бизнес-процессы для их автоматизации.";
        }

        public void codReText2()
        {
            textArea2.GetComponent<Text>().text = "Code review (код ревью) - инженерная практика"+
            " в терминах гибкой методологии разработки. Это анализ (инспекция) кода с целью"+
            " выявить ошибки, недочеты, расхождения в стиле написания кода, в соответствии"+
            " написанного кода и поставленной задачи.";
        }

        public void GameOver(string[] names, int[] roles, string whowin)
        {
            string role = "";
            for(int i = 0; i < names.Length; i++)
            {
                if (roles[i] == 0) role = "Программист";
                if (roles[i] == 1) role = "Тестеровщик";
                if (roles[i] == 2) role = "Кодвьюрер";
                if (roles[i] == 3) role = "Менеджер";

                WhoWasWho[i].SetActive(true);
                WhoWasWho[i].GetComponent<Text>().text = "" + names[i] + " - " + role;
            }

            WhoWinText.GetComponent<Text>().text = "Победили" + whowin;
        }

        public void Light(bool[] time)
        {
            if (!GameManager.Instance.IsVoted)
            {

                for (int i = 0; i < time.Length; i++)
                {
                    if (time[i] && i != 0 && GameManager.Instance.ActivePlayersGB[GameManager.Instance.selfId].GetComponent<Player>().role == i)
                    {
                        for (int g = 0; g < GameManager.Instance.ActivePlayersGB.Length; g++)
                        {
                            if (g != GameManager.Instance.selfId && GameManager.Instance.ActivePlayersGB[GameManager.Instance.selfId].GetComponent<Player>().role != GameManager.Instance.ActivePlayersGB[g].GetComponent<Player>().role)
                                GameManager.Instance.ActivePlayersGB[g].transform.GetChild(0).gameObject.SetActive(true);
                            Debug.Log("jdjdjkvs");
                        }
                        break;

                    }

                    if (i == 0)
                    {
                        for (int g = 0; g < GameManager.Instance.ActivePlayersGB.Length; g++)
                        {
                            if (g != GameManager.Instance.selfId) GameManager.Instance.ActivePlayersGB[g].transform.GetChild(0).gameObject.SetActive(true);
                        }
                        break;
                    }

                    if (i != GameManager.Instance.ActivePlayersGB[GameManager.Instance.selfId].GetComponent<Player>().role)
                    {
                        for (int g = 0; g < GameManager.Instance.ActivePlayersGB.Length; g++)
                        {
                            if (g != GameManager.Instance.selfId) GameManager.Instance.ActivePlayersGB[g].transform.GetChild(0).gameObject.SetActive(false);
                        }
                    }


                }

            }
        }

        public void PlayerLightOff(int id)
        {
            GameManager.Instance.ActivePlayersGB[id].transform.GetChild(0).gameObject.SetActive(false);
        }

        public void AllPlayersLightOff()
        {
            for(int g = 0; g < GameManager.Instance.ActivePlayersGB.Length; g++)
                    {
                        if(g != GameManager.Instance.selfId) GameManager.Instance.ActivePlayersGB[g].transform.GetChild(0).gameObject.SetActive(false);
                    }
        }

    }
}
