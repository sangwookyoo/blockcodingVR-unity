using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DestoryPlayer : MonoBehaviour
{
    public GameObject Player;

    public void OnButtonClick()
    {
        Destroy(Player);
    }
}
