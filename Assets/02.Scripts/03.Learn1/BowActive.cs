using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowActive : MonoBehaviour
{
    public GameObject Player;
    public GameObject Bow;
    public GameObject Monster;

    void OnTriggerEnter(Collider c)
    {
        if (c.name == "Player")
        {
            Debug.Log("충돌감지");
            Player.SetActive(false);
            Bow.SetActive(true);
            Monster.SetActive(true);
        }
    }
}