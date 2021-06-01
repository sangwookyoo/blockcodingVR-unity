using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trash : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(transform.childCount > 0)
        {
            Transform Trash = transform.GetChild(0);
            Destroy(Trash.gameObject);
        }
    }
}
