using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class SetActive2 : MonoBehaviour
{
    public SteamVR_Input_Sources leftHand = SteamVR_Input_Sources.LeftHand;
    public SteamVR_Input_Sources rightHand = SteamVR_Input_Sources.RightHand;
    public SteamVR_Input_Sources any = SteamVR_Input_Sources.Any;
    public SteamVR_Action_Boolean trigger = SteamVR_Actions.default_InteractUI;
    public SteamVR_Action_Boolean Grip = SteamVR_Actions.default_GrabGrip;
    public SteamVR_Action_Boolean Trackpad = SteamVR_Actions.default_Teleport;

    private bool state;


    public GameObject Player;
   


    // Start is called before the first frame update
    void Start()
    {
        state = false;
    }

    // Update is called once per frame
    void Update()
    {
        if (Trackpad.GetStateDown(rightHand))
        {
            if (state == false)
            {
                Player.gameObject.SetActive(true);
                state = true;
                Debug.Log("활성화");


               
            }
            else
            {
                Player.gameObject.SetActive(false);
                state = false;
                Debug.Log("비활성화");

            }

        }

    }
}
