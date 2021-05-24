using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;

public class NetworkPlayer : MonoBehaviour
{
    public PhotonView photonView;

    void Update()
    {
        /*if (photonView.IsMine)
        {
            float Move = Input.GetAxis("Vertical") * Time.deltaTime;
            float Rotate = Input.GetAxis("Horizontal") * Time.deltaTime;

            transform.Translate(0, 0, Move);
            transform.Rotate(0, Rotate, 0);

            photonView.RPC("moveRPC", RpcTarget.AllBuffered, Move);
            photonView.RPC("rotateRPC", RpcTarget.AllBuffered, Rotate);
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