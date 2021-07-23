using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkPlayer : MonoBehaviourPun
{
    public Camera _camera;

    void Start()
    {

        if (photonView.IsMine)
        {
            _camera.tag = "MainCamera";
            AudioListener.volume = 1;
        }
        else
        {
            _camera.enabled = false;
        }
    }

    void Update()
    {
        if (!photonView.IsMine)
        {
            return;
        }
    }
}