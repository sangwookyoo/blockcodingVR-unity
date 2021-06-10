using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PanelBtn : MonoBehaviour
{
    public GameObject PanelAlpha;

    CanvasGroup OpenCanvasGroup;
    CanvasGroup CloseCanvasGroup;


    public void OnBtnClick()
    {
        OpenCanvasGroup = PanelAlpha.GetComponent<CanvasGroup>();
        OpenCanvasGroup.alpha = 1;
        OpenCanvasGroup.blocksRaycasts = true;
    }

    public void OffBtnClick()
    {
        CloseCanvasGroup = PanelAlpha.GetComponent<CanvasGroup>();
        CloseCanvasGroup.alpha = 0;
        CloseCanvasGroup.blocksRaycasts = false;
    }
}
