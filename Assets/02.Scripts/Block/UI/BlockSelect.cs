using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class BlockSelect : MonoBehaviour
{
    List<GameObject> BlockList = new List<GameObject>();
    Transform a;

    public void Start()
    {
        int i;

        for (i = 0; i < 4; i++)
        {
            a = GameObject.Find("Block").transform.GetChild(i);
            BlockList.Add(a.gameObject);
        }

    }
    
    public void select1()
    {
        for (int i = 0; i < 4; i++)
            if (i == 0)
                BlockList[i].SetActive(true);
            else
                BlockList[i].SetActive(false);
    }

    public void select2()
    {

        for (int i = 0; i < 4; i++)
            if (i == 1)
                BlockList[i].SetActive(true);
            else
                BlockList[i].SetActive(false);
    }
    public void select3()
    {
        for (int i = 0; i < 4; i++)
            if (i == 2)
                BlockList[i].SetActive(true);
            else
                BlockList[i].SetActive(false);
    }

    public void select4()
    {
        for (int i = 0; i < 4; i++)
            if (i == 3)
                BlockList[i].SetActive(true);
            else
                BlockList[i].SetActive(false);
    }
}