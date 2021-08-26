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

    GameObject createFx;
    GameObject DropFor;



    public void OnEnable()
    {
        //if (containerImage != null)
           // normalColor = containerImage.color;
    }

    public void OnDrop(PointerEventData data)
    {
        Drop = Resources.Load("Drop") as GameObject;
        DropFor = Resources.Load("DropFor") as GameObject;
        //containerImage.color = normalColor;

        if (receivingImage == null)
            return;

        int Count = DropBox.transform.childCount;
        Debug.Log("data.pointerDrag.tag : " + data.pointerDrag.tag);
        Debug.Log("data.pointerDrag : " + data.pointerDrag);
        //Debug.Log(data.pointerDrag.name);
        if (Count == 1)
        {
            Sprite dropSprite = GetDropSprite(data);
            if (dropSprite != null)
                receivingImage.overrideSprite = dropSprite;

           
            gameObject.GetComponent<Image>().sprite = receivingImage.overrideSprite;
            if (data.pointerDrag.CompareTag("MoveZ") && data.pointerDrag.transform.parent.CompareTag("Drag"))
            {
                gameObject.tag = "MoveZ";
                gameObject.AddComponent<FunctionMove>();
                gameObject.GetComponent<DropImage>().enabled = false;
                gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);
                gameObject.AddComponent<DragImage>();
                CreateDrops();

            }
            if (data.pointerDrag.CompareTag("Rotate_R") && data.pointerDrag.transform.parent.CompareTag("Drag"))
            {
                gameObject.tag = "Rotate_R";
                gameObject.AddComponent<FunctionRotate>().RightRotate();
                gameObject.GetComponent<DropImage>().enabled = false;
                gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);
                gameObject.AddComponent<DragImage>();
                CreateDrops();
            }
            if (data.pointerDrag.CompareTag("Rotate_L") && data.pointerDrag.transform.parent.CompareTag("Drag"))
            {
                gameObject.tag = "Rotate_L";
                gameObject.AddComponent<FunctionRotate>().LeftRotate();
                gameObject.GetComponent<DropImage>().enabled = false;
                gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);
                gameObject.AddComponent<DragImage>();
                CreateDrops();
            }
            if (data.pointerDrag.CompareTag("Rotate_B") && data.pointerDrag.transform.parent.CompareTag("Drag"))
            {
                gameObject.tag = "Rotate_B";
                gameObject.AddComponent<FunctionRotate>().BackRotate();
                gameObject.GetComponent<DropImage>().enabled = false;
                gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);
                gameObject.AddComponent<DragImage>();
                CreateDrops();
            }
            if (data.pointerDrag.CompareTag("Jump") && data.pointerDrag.transform.parent.CompareTag("Drag"))
            {
                gameObject.tag = "Jump";
                gameObject.AddComponent<FunctionJump>().Jump();
                gameObject.GetComponent<DropImage>().enabled = false;
                gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);
                gameObject.AddComponent<DragImage>();
                CreateDrops();
            }
            if (data.pointerDrag.CompareTag("Class") && data.pointerDrag.transform.parent.CompareTag("Drag"))
            {
                gameObject.tag = "Class";
                gameObject.AddComponent<FunctionClass>().Function();
                gameObject.GetComponent<DropImage>().enabled = false;
                gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);
                gameObject.AddComponent<DragImage>();
                
                createFx = (Instantiate(data.pointerDrag.transform.GetChild(0).gameObject, gameObject.transform.position, gameObject.transform.rotation));
                createFx.transform.SetParent(gameObject.transform);
                Debug.Log(createFx.name);
                
                CreateDrops();
            }
            if (data.pointerDrag.CompareTag("For") && data.pointerDrag.transform.parent.CompareTag("Drag"))
            {
                gameObject.tag = "For";
                gameObject.GetComponent<DropImage>().enabled = false;
                gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);
                gameObject.AddComponent<DragImage>();
                CreateDropsFor();
                CreateDropsfors();
            }
        }
        else if (Count > 1)
        {
            Sprite dropSprite = GetDropSprite(data);
            if (dropSprite != null && data.pointerDrag.transform.parent.CompareTag("Drag"))
                receivingImage.overrideSprite = dropSprite;

            gameObject.GetComponent<Image>().sprite = receivingImage.overrideSprite;
            if (data.pointerDrag.CompareTag("MoveZ") && data.pointerDrag.transform.parent.CompareTag("Drag"))
            {
                gameObject.AddComponent<FunctionMove>();
                gameObject.tag = "MoveZ";
                gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);
                gameObject.GetComponent<Image>().sprite = data.pointerDrag.GetComponent<Image>().sprite;
                gameObject.AddComponent<DragImage>();
                CreateDrops();
            }
            if (data.pointerDrag.CompareTag("Rotate_R") && data.pointerDrag.transform.parent.CompareTag("Drag"))
            {
                gameObject.AddComponent<FunctionRotate>().RightRotate();
                gameObject.tag = "Rotate_R";
                gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);
                gameObject.AddComponent<DragImage>();
                CreateDrops();
            }
            if (data.pointerDrag.CompareTag("Rotate_L") && data.pointerDrag.transform.parent.CompareTag("Drag"))
            {
                gameObject.AddComponent<FunctionRotate>().LeftRotate();
                gameObject.tag = "Rotate_L";
                gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);
                gameObject.AddComponent<DragImage>();
                CreateDrops();
            }
            if (data.pointerDrag.CompareTag("Rotate_B") && data.pointerDrag.transform.parent.CompareTag("Drag"))
            {
                gameObject.AddComponent<FunctionRotate>().BackRotate();
                gameObject.tag = "Rotate_B";
                gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);
                gameObject.AddComponent<DragImage>();
                CreateDrops();
            }
            if (data.pointerDrag.CompareTag("Jump") && data.pointerDrag.transform.parent.CompareTag("Drag"))
            {
                gameObject.AddComponent<FunctionJump>().Jump();
                gameObject.tag = "Jump";
                gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);
                gameObject.AddComponent<DragImage>();
                CreateDrops();
            }
            if (data.pointerDrag.CompareTag("For") && data.pointerDrag.transform.parent.CompareTag("Drag"))
            {
                gameObject.tag = "For";
                gameObject.GetComponent<DropImage>().enabled = false;
                gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);
                gameObject.AddComponent<DragImage>();
                CreateDropsFor();
                CreateDropsfors();
            }
            if (data.pointerDrag.CompareTag("Class") && data.pointerDrag.transform.parent.CompareTag("Drag"))
            {
                gameObject.tag = "Class";
                gameObject.AddComponent<FunctionClass>().Function();
                gameObject.GetComponent<DropImage>().enabled = false;
                gameObject.GetComponent<Image>().color = new Color(255, 255, 255, 1);
                gameObject.AddComponent<DragImage>();

                createFx = (Instantiate(data.pointerDrag.transform.GetChild(0).gameObject, gameObject.transform.position, gameObject.transform.rotation));
                createFx.transform.SetParent(gameObject.transform);
                Debug.Log(createFx.name);

                CreateDrops();
            }
        }
        
    }

    public void OnPointerEnter(PointerEventData data)
    {
        if (containerImage == null)
            return;

        Sprite dropSprite = GetDropSprite(data);
       // if (dropSprite != null)
           // containerImage.color = highlightColor;
    }
   
    public void OnPointerExit(PointerEventData data)
    {
        if (containerImage == null)
            return;

        //containerImage.color = normalColor;
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

    void CreateDrops()
    {
        CreateDrop = (Instantiate(Drop, DropBox.transform.position, DropBox.transform.rotation));
        CreateDrop.transform.SetParent(DropBox.transform);
        CreateDrop.transform.localScale = new Vector3(1, 1, 1);
        CreateDrop.GetComponent<Image>().color = new Color(255, 255, 255, 1);        /////////////////////노란 박스 보고싶으면 255,255,255,1
        CreateDrop.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 100);
        CreateDrop.GetComponent<DropImage>().containerImage = containerImage;
        CreateDrop.GetComponent<DropImage>().receivingImage = CreateDrop.GetComponent<Image>();
        CreateDrop.GetComponent<DropImage>().DropBox = DropBox;
        CreateDrop.GetComponent<DropImage>().Drop = Drop;
    }
    void CreateDropsfors()
    {
        CreateDrop = (Instantiate(DropFor, DropBox.transform.position, DropBox.transform.rotation));
        CreateDrop.transform.SetParent(DropBox.transform);
        CreateDrop.transform.localScale = new Vector3(1, 1, 1);
        CreateDrop.GetComponent<Image>().color = new Color(255, 255, 255, 1);        /////////////////////노란 박스 보고싶으면 255,255,255,1
        CreateDrop.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 100);
        CreateDrop.GetComponent<DropImage>().containerImage = containerImage;
        CreateDrop.GetComponent<DropImage>().receivingImage = CreateDrop.GetComponent<Image>();
        CreateDrop.GetComponent<DropImage>().DropBox = DropBox;
        CreateDrop.GetComponent<DropImage>().Drop = DropFor;
    }
    void CreateDropsFor()
    {
        gameObject.GetComponent<Image>().type = Image.Type.Sliced;
        gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(320, 172);
        gameObject.AddComponent<FunctionFor>();
        gameObject.tag = "For";
        gameObject.transform.localScale = new Vector3(1f, 0.88f, 1f);
        gameObject.AddComponent<VerticalLayoutGroup>().padding.left = 25;
        gameObject.GetComponent<VerticalLayoutGroup>().padding.top = 55;
        gameObject.GetComponent<VerticalLayoutGroup>().spacing = -55;
        gameObject.GetComponent<VerticalLayoutGroup>().childControlHeight = false;
        gameObject.GetComponent<VerticalLayoutGroup>().childControlWidth = false;
        gameObject.GetComponent<VerticalLayoutGroup>().childForceExpandHeight = false;
        gameObject.GetComponent<VerticalLayoutGroup>().childForceExpandWidth = false;

        CreateDrop = (Instantiate(DropFor, gameObject.transform.position, gameObject.transform.rotation));
        CreateDrop.transform.SetParent(gameObject.transform);
        CreateDrop.transform.localScale = new Vector3(1, 1, 1);
        CreateDrop.GetComponent<Image>().color = new Color(255, 255, 255, 1);       /////////////////////노란 박스 보고싶으면 255,255,255,1
        CreateDrop.GetComponent<RectTransform>().sizeDelta = new Vector2(300, 100);
        CreateDrop.name = "ForDrop";
        CreateDrop.GetComponent<DropImage>().containerImage = containerImage;
        CreateDrop.GetComponent<DropImage>().receivingImage = CreateDrop.GetComponent<Image>();
        CreateDrop.GetComponent<DropImage>().DropBox = gameObject;
        CreateDrop.GetComponent<DropImage>().Drop = DropFor;



        //gameObject.AddComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
    }

    void CreateDropsClass()
    {
        
    }
}
