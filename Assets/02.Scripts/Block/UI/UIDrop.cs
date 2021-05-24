using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIDrop : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData Data)
    {
        EventSystem.current.currentSelectedGameObject.transform.parent = transform;
        EventSystem.current.currentSelectedGameObject.transform.SetAsLastSibling();
        //LayoutRebuilder.ForceRebuildLayoutImmediate(GetComponent<RectTransform>());
    }
}