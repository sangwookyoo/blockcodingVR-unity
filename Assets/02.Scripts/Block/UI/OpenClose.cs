using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenClose : MonoBehaviour
{
    public GameObject gameObjectOpen;
    public GameObject gameObjectClose;
    public Image StartButton = null ;
    CanvasGroup OpenCanvasGroup;
    CanvasGroup CloseCanvasGroup;

   
    public void OnOffButtonClick()
    {
        OpenCanvasGroup = gameObjectOpen.GetComponent<CanvasGroup>();
        CloseCanvasGroup = gameObjectClose.GetComponent<CanvasGroup>();
        OpenCanvasGroup.alpha = 1;
        OpenCanvasGroup.blocksRaycasts = true;
        CloseCanvasGroup.alpha = 0;
        CloseCanvasGroup.blocksRaycasts = false;


        if (StartButton != null && StartButton.enabled == false)
            StartButton.enabled = false;

    }
}