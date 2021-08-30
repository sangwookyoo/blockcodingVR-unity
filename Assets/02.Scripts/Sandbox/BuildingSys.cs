using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

using Photon.Pun.Demo.Procedural;



// 블록생성 스크립트
public class BuildingSys : MonoBehaviourPunCallbacks
{
    public SteamVR_Input_Sources leftHand = SteamVR_Input_Sources.LeftHand;
    public SteamVR_Input_Sources rightHand = SteamVR_Input_Sources.RightHand;
    public SteamVR_Input_Sources any = SteamVR_Input_Sources.Any;
    public SteamVR_Action_Boolean trigger = SteamVR_Actions.default_InteractUI;
    public SteamVR_Action_Boolean Grip = SteamVR_Actions.default_GrabGrip;
    public SteamVR_Action_Boolean Trackpad = SteamVR_Actions.default_Teleport;

    private BlockType[] allBlockTypes;






    [SerializeField]
    private Camera playerCamera;

    private bool buildModeOn = false;
    private bool canBuild = false;
    private bool triggerEnter = false;

    private BlockSys bSys;



    [SerializeField]
    private LayerMask buildableSurfacesLayer;

    private Vector3 buildPos;

    private GameObject currentTemplateBlock;

    [SerializeField]
    private GameObject blockTemplatePrefab;
    [SerializeField]
    private GameObject blockPrefab;



    [SerializeField]
    private Material templateMaterial;

    private int blockSelectCounter = 0;




    private void Start()
    {
        bSys = GetComponent<BlockSys>();



    }

    private void Update()
    {

        if (buildModeOn)
        {
            RaycastHit buildPosHit;
            if (Physics.Raycast(playerCamera.ScreenPointToRay(new Vector3(Screen.width /2, Screen.height , 0)), out buildPosHit, 500, buildableSurfacesLayer))
            {
                Vector3 point = buildPosHit.point;
                buildPos = new Vector3(Mathf.Round(point.x), Mathf.Round(point.y) , Mathf.Round(point.z));
                canBuild = true;
            }

        }

        if (!buildModeOn && currentTemplateBlock != null)
        {
            Destroy(currentTemplateBlock.gameObject);
            canBuild = false;
        }

        if (canBuild && currentTemplateBlock == null)
        {
            currentTemplateBlock = Instantiate(blockTemplatePrefab, buildPos, Quaternion.identity);
            currentTemplateBlock.GetComponent<MeshRenderer>().material = templateMaterial;
        }

        if (canBuild && currentTemplateBlock != null)
        {
            currentTemplateBlock.transform.position = buildPos;


            if (trigger.GetStateDown(leftHand))     // 블록 생성 (왼손 트리거)
            {
                PlaceBlock();
                Debug.Log("박스 생성");

            }
            
            if (Trackpad.GetStateDown(leftHand)) // 블록 삭제 (왼쪽 터치 패드)
            {
                
               DestroyBlock();

            }



        }

    }

    public void OnButtonClick() // 박스 변경
    {

        blockSelectCounter++;

        if (blockSelectCounter >= bSys.allBlocks.Count)

            blockSelectCounter = 0;
        Debug.Log("박스 변경");
    }


    public void OnButtonClick1()  // 모드 변경
    {

        buildModeOn = !buildModeOn;
        Debug.Log("모드 변경");

    }



    
    public void PlaceBlock()
    {

        GameObject newBlock = Instantiate(blockPrefab, buildPos, Quaternion.identity);
        BlockS tempBlock = bSys.allBlocks[blockSelectCounter];
        newBlock.name = tempBlock.blockName + "-Block";
        newBlock.GetComponent<MeshRenderer>().material = tempBlock.blockMaterial;

        

    }
    
    public void DestroyBlock()

     {
         if (buildModeOn)
         {

                GameObject BlockPrefap = GameObject.FindGameObjectWithTag("Destroy");
                Destroy(BlockPrefap.gameObject);
                Debug.Log("박스 제거");
            
         }

      }


}