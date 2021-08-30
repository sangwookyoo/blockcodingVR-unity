using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowOff : MonoBehaviour
{
    //솔, 충돌 시 화살 오브젝트 비활성화
    void OnTriggerEnter(Collider c)
    {
        if (c.name == "Player")
        {
            Debug.Log("충돌감지");
            this.gameObject.SetActive(false);
        }
    }
}
