using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenClose : MonoBehaviour
{
    public GameObject gameObjectOpen = null;
    public GameObject gameObjectClose = null;
    public Image StartButton = null;

    public void OnButtonClick()
    {
        gameObjectOpen.SetActive(true);
        gameObjectClose.SetActive(false);

        if (StartButton != null && StartButton.enabled == false)
            StartButton.enabled = true;
    }
}