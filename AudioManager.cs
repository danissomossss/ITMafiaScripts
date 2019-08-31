using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public GameObject[] Audios;

    public void OnClick()
    {
        Audios[1].SetActive(true);
        Invoke("OnClickOff", 0.2f);
    }

    void OnClickOff()
    {
        Audios[1].SetActive(false);
    }
}

