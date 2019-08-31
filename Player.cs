using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using TopDownShooter.Game;
using TopDownShooter.Net;
using TopDownShooter.UI;
using System;
using UnityEngine.UI;

public class Player : MonoBehaviour, IPointerClickHandler {

    public int id;
    private int selfId;
    private Rigidbody rb;
    private Action sender;
    private bool initialized;
    public int role = 0;
    public int choose;
    public int votes;
    public string name;

    public void Init(int id, string name)
    {
        this.id = id;
        this.name = name;
        
   GameManager.Instance.PlayersGB[id].SetActive(true);
        
        
        selfId = GameManager.Instance.selfId;
        gameObject.transform.GetChild(3).GetComponent<Text>().text = name;
        sender = NetManager.Instance.netType == NetType.SERVER ? (Action)SendServerTransform : (Action)SendClientTransform;
        if (selfId != id)
        {
            Destroy(GetComponent<PlayerJoystickController>());
            Destroy(GetComponent<PlayerMovement>());
        }
        else
        {
           // GameManager.Instance.playerCamera.player = transform;
        }
        initialized = true;
        Debug.Log("Init");
    }

    

    private void SendClientTransform()
    {
        NetManager.Instance.client.SendTransform(selfId, UIManager.Instance.playerName.text, GameManager.Instance.IsBusyPosition);
    }
    private void SendServerTransform()
    {
        NetManager.Instance.server.SendTransform(selfId,UIManager.Instance.playerName.text, GameManager.Instance.IsBusyPosition, GameManager.Instance.time);
    }

    private void FixedUpdate()
    {
        if (id != selfId || !initialized)
            return;
        sender.Invoke();
    }

    public void  OnPointerClick(PointerEventData eventData)
    {
        if (GameManager.Instance.time[1] && !GameManager.Instance.IsVoted)
        {
            if (id != GameManager.Instance.selfId && GameManager.Instance.PlayersGB[GameManager.Instance.selfId].GetComponent<Player>().role == 1 && NetManager.Instance.netType == NetType.SERVER)
            {
                GameManager.Instance.choose = id;
                GameManager.Instance.IsVoted = true;
                NetManager.Instance.server.SendVotesNot0();
                UIManager.Instance.PlayerLightOff(id);
            }

            if (id != GameManager.Instance.selfId && GameManager.Instance.PlayersGB[GameManager.Instance.selfId].GetComponent<Player>().role == 1 && NetManager.Instance.netType == NetType.CLIENT)
            {
                GameManager.Instance.choose = id;
                GameManager.Instance.IsVoted = true;
                NetManager.Instance.client.SendVotesNot0();
                UIManager.Instance.PlayerLightOff(id);
            }
        }

        if (GameManager.Instance.time[2] && !GameManager.Instance.IsVoted)
        {
            if (id != GameManager.Instance.selfId && GameManager.Instance.PlayersGB[GameManager.Instance.selfId].GetComponent<Player>().role == 2 && NetManager.Instance.netType == NetType.SERVER)
            {
                GameManager.Instance.choose = id;
                GameManager.Instance.IsVoted = true;
                NetManager.Instance.server.SendVotesNot0();
                UIManager.Instance.PlayerLightOff(id);
            }

            if (id != GameManager.Instance.selfId && GameManager.Instance.PlayersGB[GameManager.Instance.selfId].GetComponent<Player>().role == 1 && NetManager.Instance.netType == NetType.CLIENT)
            {
                GameManager.Instance.choose = id;
                GameManager.Instance.IsVoted = true;
                NetManager.Instance.client.SendVotesNot0();
                UIManager.Instance.PlayerLightOff(id);
            }
        }

        if (GameManager.Instance.time[3] && !GameManager.Instance.IsVoted)
                {
                    if (id != GameManager.Instance.selfId && GameManager.Instance.PlayersGB[GameManager.Instance.selfId].GetComponent<Player>().role == 3 && NetManager.Instance.netType == NetType.SERVER)
                    {
                        GameManager.Instance.choose = id;
                        GameManager.Instance.IsVoted = true;
                NetManager.Instance.server.SendVotesNot0();
                UIManager.Instance.PlayerLightOff(id);
            }

                    if (id != GameManager.Instance.selfId && GameManager.Instance.PlayersGB[GameManager.Instance.selfId].GetComponent<Player>().role == 1 && NetManager.Instance.netType == NetType.CLIENT)
                    {
                        GameManager.Instance.choose = id;
                        GameManager.Instance.IsVoted = true;
                NetManager.Instance.client.SendVotesNot0();
                UIManager.Instance.PlayerLightOff(id);
            }
                }

        if (GameManager.Instance.time[0] && !GameManager.Instance.IsVoted)
        {
            if (id != GameManager.Instance.selfId && NetManager.Instance.netType == NetType.SERVER)
            {
                GameManager.Instance.votes[id] += 1;
                GameManager.Instance.IsVoted = true;
                NetManager.Instance.server.SendVotes0(GameManager.Instance.selfId, id);
                GameManager.Instance.PlayersGB[GameManager.Instance.selfId].transform.GetChild(4).GetComponent<Text>().text = name;
                UIManager.Instance.PlayerLightOff(id);
            }

            if (id != GameManager.Instance.selfId && NetManager.Instance.netType == NetType.CLIENT)
            {
                GameManager.Instance.choose = id;
                GameManager.Instance.IsVoted = true;
                NetManager.Instance.client.SendVotes0(GameManager.Instance.selfId, id);
                GameManager.Instance.PlayersGB[GameManager.Instance.selfId].transform.GetChild(4).GetComponent<Text>().text = name;
                UIManager.Instance.PlayerLightOff(id);
            }
        }

    }

}
