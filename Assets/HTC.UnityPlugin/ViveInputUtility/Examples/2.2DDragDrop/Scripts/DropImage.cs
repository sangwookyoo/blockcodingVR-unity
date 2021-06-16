using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropImage : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
    public Image containerImage;
    public Image receivingImage;
    private Color normalColor;
    public Color highlightColor;

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
        Drop = Resources.Load("Drop") as GameObject;
        containerImage.color = normalColor;

        if (receivingImage == null)
            return;

        int Count = DropBox.transform.childCount;
        
        //Debug.Log(data.pointerDrag.name);
        if (Count == 1)
        {
            Sprite dropSprite = GetDropSprite(data);
           // Debug.Log(dropSprite.name);
            if (dropSprite != null)
                receivingImage.overrideSprite = dropSprite;

            //Debug.Log(receivingImage.name);
            if (data.pointerDrag.CompareTag("MoveZ"))
            {
                gameObject.AddComponent<FunctionMove>();
                
                gameObject.tag = "MoveZ";
                gameObject.GetComponent<DropImage>().enabled = false;

            }
            if (data.pointerDrag.CompareTag("Rotate_R"))
            {
                gameObject.AddComponent<FunctionRotate>().RightRotate();
                gameObject.tag = "Rotate_R";
                gameObject.GetComponent<DropImage>().enabled = false;
            }
            if (data.pointerDrag.CompareTag("Rotate_L"))
            {
                gameObject.AddComponent<FunctionRotate>().LeftRotate();
                gameObject.tag = "Rotate_L";
                gameObject.GetComponent<DropImage>().enabled = false;
            }
            if (data.pointerDrag.CompareTag("Rotate_B"))
            {
                gameObject.AddComponent<FunctionRotate>().BackRotate();
                gameObject.tag = "Rotate_B";
                gameObject.GetComponent<DropImage>().enabled = false;
            }
            if (data.pointerDrag.CompareTag("Jump"))
            {
                gameObject.AddComponent<FunctionJump>().Jump();
                gameObject.tag = "Jump";
                gameObject.GetComponent<DropImage>().enabled = false;
            }
            //if (data.pointerDrag.CompareTag("For"))
            //{
            //    CreateDrop = (Instantiate(Drop, gameObject.transform.position, gameObject.transform.rotation));
            //    CreateDrop.transform.SetParent(gameObject.transform);
            //    CreateDrop.transform.localScale = new Vector3(1, 1, 1);
            //    CreateDrop.GetComponent<Image>().color = new Color(255, 255, 255, 1);       //////////////////
            //    CreateDrop.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 100);
            //    gameObject.AddComponent<FunctionFor>();
            //    gameObject.tag = "For";
            //    gameObject.AddComponent<VerticalLayoutGroup>();
            //    gameObject.AddComponent<ContentSizeFitter>();
            //    CreateDrop.GetComponent<DropImage>().Drop = Drop;

            //}
            CreateDrop = (Instantiate(Drop, DropBox.transform.position, DropBox.transform.rotation));
            CreateDrop.transform.SetParent(DropBox.transform);
            CreateDrop.transform.localScale = new Vector3(1, 1, 1);
            CreateDrop.GetComponent<Image>().color = new Color(255, 255, 255, 1);        /////////////////////
            CreateDrop.GetComponent<RectTransform>().sizeDelta = new Vector2(300,100);
            CreateDrop.GetComponent<DropImage>().containerImage = containerImage;
            CreateDrop.GetComponent<DropImage>().receivingImage = CreateDrop.GetComponent<Image>();
            CreateDrop.GetComponent<DropImage>().DropBox = DropBox;
            CreateDrop.GetComponent<DropImage>().Drop = Drop;
            //Debug.Log("1");
            //Debug.Log(CreateDrop.name);
        }
        else if (Count > 1)
        {
            Sprite dropSprite = GetDropSprite(data);
            //Debug.Log(dropSprite.name);
            if (dropSprite != null)
                receivingImage.overrideSprite = dropSprite;

            //Debug.Log(receivingImage.name);

            if (data.pointerDrag.CompareTag("MoveZ"))
            {
                gameObject.AddComponent<FunctionMove>();
                gameObject.tag = "MoveZ";
                gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);

            }
            if (data.pointerDrag.CompareTag("Rotate_R"))
            {
                gameObject.AddComponent<FunctionRotate>().RightRotate();
                gameObject.tag = "Rotate_R";
                gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);
            }
            if (data.pointerDrag.CompareTag("Rotate_L"))
            {
                gameObject.AddComponent<FunctionRotate>().LeftRotate();
                gameObject.tag = "Rotate_L";
                gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);
            }
            if (data.pointerDrag.CompareTag("Rotate_B"))
            {
                gameObject.AddComponent<FunctionRotate>().BackRotate();
                gameObject.tag = "Rotate_B";
                gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);
            }
            if (data.pointerDrag.CompareTag("Jump"))
            {
                gameObject.AddComponent<FunctionJump>().Jump();
                gameObject.tag = "Jump";
                gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);
            }
            //if (data.pointerDrag.CompareTag("For"))
            //{
            //    //CreateDrop.AddComponent<DragImage>();
            //    CreateDrop = (Instantiate(Drop, gameObject.transform.position, gameObject.transform.rotation));
            //    CreateDrop.transform.SetParent(gameObject.transform);
            //    CreateDrop.transform.localScale = new Vector3(1, 1, 1);
            //    CreateDrop.GetComponent<Image>().color = new Color(255, 255, 255, 1);            ////////////////
            //    CreateDrop.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 100);
            //    gameObject.AddComponent<FunctionFor>();
            //    gameObject.tag = "For";
            //    gameObject.AddComponent<VerticalLayoutGroup>();
            //    gameObject.AddComponent<ContentSizeFitter>();
            //    gameObject.GetComponent<DropImage>().containerImage = containerImage;
            //    gameObject.GetComponent<DropImage>().receivingImage = CreateDrop.GetComponent<Image>();
            //    gameObject.GetComponent<DropImage>().DropBox = DropBox;
            //    CreateDrop.GetComponent<DropImage>().Drop = Drop;

            //}
            //gameObject.GetComponent<Image>().sprite = data.pointerDrag.GetComponent<Image>().sprite;
            
            Debug.Log(gameObject.transform.parent);
            CreateDrop = Instantiate(Drop, DropBox.transform.position, DropBox.transform.rotation);
            CreateDrop.transform.SetParent(DropBox.transform);
            CreateDrop.transform.localScale = new Vector3(1, 1, 1);
            CreateDrop.GetComponent<Image>().color = new Color(255, 255, 255, 1);               /////////////////////
            CreateDrop.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 100);
            CreateDrop.GetComponent<DropImage>().containerImage = containerImage;
            CreateDrop.GetComponent<DropImage>().receivingImage = CreateDrop.GetComponent<Image>();
            CreateDrop.GetComponent<DropImage>().DropBox = DropBox;
            CreateDrop.GetComponent<DropImage>().Drop = Drop;
            Debug.Log("2");
            Debug.Log(CreateDrop.name);
        }
        if (data.pointerDrag.CompareTag("For"))
        {
            gameObject.tag = "For";
            gameObject.GetComponent<DropImage>().enabled = false;
            Sprite dropSprite = GetDropSprite(data);
            Debug.Log(dropSprite.name);
            if (dropSprite != null)
                receivingImage.overrideSprite = dropSprite;

            Debug.Log(receivingImage);

            CreateDrop = (Instantiate(Drop, gameObject.transform.position, gameObject.transform.rotation));
            CreateDrop.transform.SetParent(gameObject.transform);
            Debug.Log(gameObject.transform);
            CreateDrop.transform.localScale = new Vector3(1, 1, 1);
            CreateDrop.GetComponent<Image>().color = new Color(255, 255, 255, 1);       //////////////////
            CreateDrop.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 100);
            CreateDrop.name = "ForDrop";
            CreateDrop.GetComponent<DropImage>().containerImage = containerImage;
            CreateDrop.GetComponent<DropImage>().receivingImage = CreateDrop.GetComponent<Image>();
            CreateDrop.GetComponent<DropImage>().DropBox = gameObject;
            CreateDrop.GetComponent<DropImage>().Drop = Drop;

            gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 172);
            gameObject.AddComponent<FunctionFor>();
            gameObject.tag = "For";
            gameObject.transform.localScale = new Vector3(1f, 1f, 1f);
            gameObject.AddComponent<VerticalLayoutGroup>().padding.left = 20;
            gameObject.GetComponent<VerticalLayoutGroup>().padding.top = 35;
            gameObject.AddComponent<ContentSizeFitter>();
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
