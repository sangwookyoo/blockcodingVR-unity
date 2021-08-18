using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowActive : MonoBehaviour
{
    public GameObject VivePlayer;
    public GameObject SteamVRPlayer;
    public GameObject Bow;
    public GameObject Monster;

    //솔, 카메라 시점 함수 추가
    public GameObject Camera_1;
    Camera cam1;
    public GameObject Camera_3;
    Camera cam3;

    void OnTriggerEnter(Collider c)
    {
        if (c.name == "Player")
        {
            Debug.Log("충돌감지");
            VivePlayer.SetActive(false);
            cam1 = Camera_1.GetComponent<Camera>();
            cam1.enabled = false;

            SteamVRPlayer.SetActive(true);
            cam3 = Camera_1.GetComponent<Camera>();
            cam3.enabled = true;

            Bow.SetActive(true);
            Monster.SetActive(true);
        }
    }
}