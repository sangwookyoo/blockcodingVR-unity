using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropImageTrash : MonoBehaviour, IDropHandler //IPointerEnterHandler, IPointerExitHandler
{
    bool isOnDropTrash = false;
        

    public void OnDrop(PointerEventData data)
    {
        if (gameObject.transform.parent.CompareTag("Trash") && data.pointerDrag.transform.parent.CompareTag("Drop"))
        {
            Debug.Log("TrashDrop");
            isOnDropTrash = true;
            Destroy(data.pointerDrag.transform.gameObject);
            Destroy(GameObject.Find("icon").transform.gameObject);
        }
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
