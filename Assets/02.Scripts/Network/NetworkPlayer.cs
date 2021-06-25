using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkPlayer : MonoBehaviourPunCallbacks
{
    public PhotonView PV;

    void Update()
    {
        /*if (PV.IsMine)
        {
            float Move = Input.GetAxis("Vertical") * Time.deltaTime;
            float Rotate = Input.GetAxis("Horizontal") * Time.deltaTime;

            transform.Translate(0, 0, Move);
            transform.Rotate(0, Rotate, 0);

            PV.RPC("moveRPC", RpcTarget.All, Move);
            PV.RPC("rotateRPC", RpcTarget.All, Rotate);
        }
    }
    [PunRPC]
    void moveRPC(float MoveRPC)
    {
        float Move = Input.GetAxis("Vertical");
    }
    [PunRPC]
    void rotateRPC(float RotateRPC)
    {
        float Rotate = Input.GetAxis("Horizontal");
    }*/
    }
}