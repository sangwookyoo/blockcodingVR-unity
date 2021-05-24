using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using UnityEngine.UI;

public class UI_Time : MonoBehaviour
{
    public Text text_time;

    void Start()
    {
        Init_Time();
    }

    void Init_Time()
    {
        if (IsInvoking("Update_Time"))
            CancelInvoke("Update_Time");
        InvokeRepeating("Update_Time", 0, 0.2f);
    }

    void Update_Time()
    {
        string time = DateTime.Now.ToString("hh : mm  tt");
        text_time.text = time;
    }
}