using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoorActive : MonoBehaviour
{
    void OnTriggerEnter(Collider c)
    {
        if (c.name == "PlayerFBX")
        {
            Debug.Log("충돌감지");
            transform.GetComponent<Animator>().SetBool("isOpen", true);
        }
    }
}