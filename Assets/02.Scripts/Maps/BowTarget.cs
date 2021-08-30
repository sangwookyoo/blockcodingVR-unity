using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BowTarget : MonoBehaviour
{
    private int count = 0;
    public int counterIndex = 0;
    public GameObject key;

    void OnTriggerEnter(Collider c)
    {
        if (c.name == "Bow Arrow")
        {
            count++;
            if (count == counterIndex)
            {
                key.SetActive(true);
                Destroy(c.gameObject);
                Destroy(this.gameObject);
            }
        }
    }
}