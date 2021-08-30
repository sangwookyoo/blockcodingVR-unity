using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine;
using UnityEngine.UI;

// 블록 색 변경 시스템 스크립트

public class BlockSys : MonoBehaviour
{

    public GameObject[] spriteArray;



    [SerializeField]
    private BlockType[] allBlockTypes;



    [HideInInspector]
    public Dictionary<int, BlockS> allBlocks = new Dictionary<int, BlockS>();


    private int spriteArrayIndex;
    int count = 1;
    int count1 = 0;

    public void Start()
    {
        spriteArrayIndex = 0;
        spriteArray[0].SetActive(true);


    }

    public void Awake()
    {
        for (int i = 0; i < allBlockTypes.Length; i++)
        {
            BlockType newBlockType = allBlockTypes[i];
            BlockS newBlock = new BlockS(i, newBlockType.blockName, newBlockType.blockMat);
            allBlocks[i] = newBlock;
            UnityEngine.Debug.Log("Block added to dictionary " + allBlocks[i].blockName);

        }

    }


    public void ScreenTapUp()
    {

        if (count < spriteArray.Length)
        {
            spriteArray[count].SetActive(true);
        }
        count++;

        if (count >= spriteArray.Length)

            count = 0;




    }

    public void ScreenTapDown()
    {
        if (count1 < spriteArray.Length)
        {
            spriteArray[count1].SetActive(false);

        }
        count1++;

        if (count1 >= spriteArray.Length)

            count1 = 0;
    }
}




public class BlockS
{

    public int blockID;

    public string blockName;
    public Material blockMaterial;

    public BlockS(int id, string name, Material mat)
    {
        blockID = id;
        blockName = name;
        blockMaterial = mat;

    }
}

[Serializable]
public struct BlockType
{

    public string blockName;
    public Material blockMat;
}
