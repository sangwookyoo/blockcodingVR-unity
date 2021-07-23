using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.UI;

public class CountPlayer : MonoBehaviourPunCallbacks
{
    public string person;
    public string MaxPerson;
    public Text SendText;

    void Start()
    {
        SendText = gameObject.GetComponent<Text>();
    }

    void Update()
    {
        person = "( " + (PhotonNetwork.CountOfPlayersInRooms) + "/" + MaxPerson +  " )";
        SendText.text = person;
    }
}
