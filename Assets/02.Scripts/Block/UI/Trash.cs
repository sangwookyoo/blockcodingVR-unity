using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Trash : MonoBehaviour
{
    // Update is called once per frame
    void Update()
    {
        if(transform.childCount > 1)
        {
            Transform Trash = transform.GetChild(1);
            Destroy(Trash.gameObject);
            transform.GetChild(0).GetComponent<Image>().sprite = gameObject.GetComponent<Image>().sprite;
        }
    }
}
