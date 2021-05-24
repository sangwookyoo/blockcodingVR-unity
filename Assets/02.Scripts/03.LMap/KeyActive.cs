using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class KeyActive : MonoBehaviour
{
    public GameObject Player;
    public GameObject NPlayer;
    public GameObject key;
    public GameObject key_particle;

    private float num;

    void Update()
    {
        if (key.activeSelf == true)
        {
            key_particle.SetActive(true);
            Destroy(key, 5);
            Destroy(key_particle, 5);
            num += Time.deltaTime;

            if (num > 5.0f)
            {
                num = 0.0f;
                Player.SetActive(false);
                NPlayer.SetActive(true);
            }
        }
    }
}