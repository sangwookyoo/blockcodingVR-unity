using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropImage : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image containerImage;
    public Image receivingImage;
    private Color normalColor;
    public Color highlightColor = Color.yellow;

    public GameObject DropBox;
    public GameObject CreateDrop;
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
                gameObject.tag = "MoveZ";
            }
            if (data.pointerDrag.CompareTag("Rotate_R"))
            {
                gameObject.AddComponent<FunctionRotate>().RightRotate();
                gameObject.tag = "Rotate_R";
            }
            if (data.pointerDrag.CompareTag("Rotate_L"))
            {
                gameObject.AddComponent<FunctionRotate>().LeftRotate();
                gameObject.tag = "Rotate_L";
            }
            if (data.pointerDrag.CompareTag("Rotate_B"))
            {
                gameObject.AddComponent<FunctionRotate>().BackRotate();
                gameObject.tag = "Rotate_B";
            }
            if (data.pointerDrag.CompareTag("Jump"))
            {
                gameObject.AddComponent<FunctionJump>().Jump();
                gameObject.tag = "Jump";
            }
            CreateDrop = (Instantiate(Drop, DropBox.transform.position, DropBox.transform.rotation));
            CreateDrop.transform.SetParent(DropBox.transform);
            CreateDrop.transform.localScale = new Vector3(1, 1, 1);
            CreateDrop.GetComponent<Image>().color = new Color(255, 255, 255, 0);
            CreateDrop.GetComponent<RectTransform>().sizeDelta = new Vector2(300,100);
            Debug.Log("1");
        }
        else
        {
            Sprite dropSprite = GetDropSprite(data);
            if (dropSprite != null)
                receivingImage.overrideSprite = dropSprite;

            if (data.pointerDrag.CompareTag("MoveZ"))
            {
                CreateDrop.AddComponent<FunctionMove>();
                CreateDrop.tag = "MoveZ";
                CreateDrop.GetComponent<Image>().color = new Color(255, 255, 255, 1);
            }
            if (data.pointerDrag.CompareTag("Rotate_R"))
            {
                CreateDrop.AddComponent<FunctionRotate>().RightRotate();
                CreateDrop.tag = "Rotate_R";
                CreateDrop.GetComponent<Image>().color = new Color(255, 255, 255, 1);
            }
            if (data.pointerDrag.CompareTag("Rotate_L"))
            {
                CreateDrop.AddComponent<FunctionRotate>().LeftRotate();
                CreateDrop.tag = "Rotate_L";
                CreateDrop.GetComponent<Image>().color = new Color(255, 255, 255, 1);
            }
            if (data.pointerDrag.CompareTag("Rotate_B"))
            {
                CreateDrop.AddComponent<FunctionRotate>().BackRotate();
                CreateDrop.tag = "Rotate_B";
                CreateDrop.GetComponent<Image>().color = new Color(255, 255, 255, 1);
            }
            if (data.pointerDrag.CompareTag("Jump"))
            {
                CreateDrop.AddComponent<FunctionJump>().Jump();
                CreateDrop.tag = "Jump";
                CreateDrop.GetComponent<Image>().color = new Color(255, 255, 255, 1);
            }
            if (data.pointerDrag.CompareTag("For"))
            {
                CreateDrop.AddComponent<FunctionFor>();
                CreateDrop.tag = "For";
            }
            CreateDrop.GetComponent<Image>().sprite = data.pointerDrag.GetComponent<Image>().sprite;
            CreateDrop = Instantiate(Drop, DropBox.transform.position, DropBox.transform.rotation);
            CreateDrop.transform.SetParent(DropBox.transform);
            CreateDrop.transform.localScale = new Vector3(1, 1, 1);
            CreateDrop.GetComponent<Image>().color = new Color(255, 255, 255, 0);
            CreateDrop.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 100);
            Debug.Log("2");
            Debug.Log(CreateDrop.transform.parent.name);
        }
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
