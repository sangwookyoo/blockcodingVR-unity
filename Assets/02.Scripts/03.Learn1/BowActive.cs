using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowActive : MonoBehaviour
{
    public GameObject VivePlayer;
    public GameObject SteamVRPlayer;
    public GameObject Bow;
    public GameObject Monster;

    void OnTriggerEnter(Collider c)
    {
        if (c.name == "Player")
        {
            Debug.Log("충돌감지");
            VivePlayer.SetActive(false);
            SteamVRPlayer.SetActive(true);
            Bow.SetActive(true);
            Monster.SetActive(true);
        }
    }
}