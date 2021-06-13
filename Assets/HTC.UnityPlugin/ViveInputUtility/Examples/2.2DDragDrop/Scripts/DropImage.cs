using UnityEngine;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using System.Collections;
public class DropImage : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image containerImage;
    public Image receivingImage;
    private Color normalColor;
    public Color highlightColor = Color.yellow;

    [SerializeField]
    GameObject CreateDrop;
    public GameObject DropBox;
    public GameObject Drop;
    
    
    public void OnEnable()
    {
        if (containerImage != null)
            normalColor = containerImage.color;
    }

    public void OnDrop(PointerEventData data)
    {
        containerImage.color = normalColor;

        if (receivingImage == null)
            return;

        int Count = DropBox.transform.childCount;
        Debug.Log("Count : " + Count);
        if (Count == 1)
        {
            Sprite dropSprite = GetDropSprite(data);
            if (dropSprite != null)
                receivingImage.overrideSprite = dropSprite;

            if (data.pointerDrag.CompareTag("MoveZ"))
            {
                gameObject.AddComponent<FunctionMove>();
            }
            if (data.pointerDrag.CompareTag("Rotate_R"))
            {
                gameObject.AddComponent<FunctionRotate>().RightRotate();
            }
            if (data.pointerDrag.CompareTag("Rotate_L"))
            {
                gameObject.AddComponent<FunctionRotate>().LeftRotate();
            }
            if (data.pointerDrag.CompareTag("Rotate_B"))
            {
                gameObject.AddComponent<FunctionRotate>().BackRotate();
            }
            if (data.pointerDrag.CompareTag("Jump"))
            {
                gameObject.AddComponent<FunctionJump>().Jump();
            }
            CreateDrop = (Instantiate(Drop, DropBox.transform.position, Quaternion.identity));
            CreateDrop.transform.SetParent(DropBox.transform);
        }
        else
        //{
        //    Sprite dropSprite = GetDropSprite(data);
        //    if (dropSprite != null)
        //        receivingImage.overrideSprite = dropSprite;

        //    if (data.pointerDrag.CompareTag("MoveZ"))
        //    {
        //        CreateDrop.AddComponent<FunctionMove>();
        //    }
        //    if (data.pointerDrag.CompareTag("Rotate_R"))
        //    {
        //        CreateDrop.AddComponent<FunctionRotate>().RightRotate();
        //    }
        //    if (data.pointerDrag.CompareTag("Rotate_L"))
        //    {
        //        CreateDrop.AddComponent<FunctionRotate>().LeftRotate();
        //    }
        //    if (data.pointerDrag.CompareTag("Rotate_B"))
        //    {
        //        CreateDrop.AddComponent<FunctionRotate>().BackRotate();
        //    }
        //    if (data.pointerDrag.CompareTag("Jump"))
        //    {
        //        CreateDrop.AddComponent<FunctionJump>().Jump();
        //    }
        //    CreateDrop = Instantiate(Drop, DropBox.transform.position, Quaternion.identity);
        //    CreateDrop.transform.SetParent(DropBox.transform);
        //    Debug.Log(CreateDrop.transform.parent.name);
        //}


    }

    public void OnPointerEnter(PointerEventData data)
    {
        if (containerImage == null)
            return;

        Sprite dropSprite = GetDropSprite(data);
        if (dropSprite != null)
            containerImage.color = highlightColor;
    }

    public void OnPointerExit(PointerEventData data)
    {
        if (containerImage == null)
            return;

        containerImage.color = normalColor;
    }

    private Sprite GetDropSprite(PointerEventData data)
    {
        var originalObj = data.pointerDrag;
        if (originalObj == null)
            return null;

        var dragMe = originalObj.GetComponent<DragImage>();
        if (dragMe == null)
            return null;

        var srcImage = originalObj.GetComponent<Image>();
        if (srcImage == null)
            return null;

        return srcImage.sprite;
    }
}
