using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class UIDrag : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler, IBeginDragHandler
{
    Vector3 startPosition;
    Vector3 diffPosition;
    GameObject canvas_;
    Transform ParentPos;
    Vector3 StartPos;
    GameObject CloneBlock;
    void Start()
    {
        FunctionMove FunctionMove = FindObjectOfType<FunctionMove>();
        FunctionRotate functionRotate = FindObjectOfType<FunctionRotate>();
        ParentPos = transform.parent; //부모 트랜스폼 저장
        canvas_ = GameObject.Find("Panel");
    }
    void Update()
    {
        
    }
    public void OnBeginDrag(PointerEventData eventData)
    {
        StartPos = transform.position; 
    }

   
    public void OnDrag(PointerEventData eventData)
    {
        // 마우스 좌표 인식해서 블록 이동
        Vector3 obj = Camera.main.WorldToScreenPoint(transform.position);
        Vector3 mouseposition = new Vector3(Input.mousePosition.x, Input.mousePosition.y, obj.z);
        Vector3 objPosition = Camera.main.ScreenToWorldPoint(mouseposition);
        transform.position = objPosition;
        //transform.position = Input.mousePosition - diffPosition; //기존 블록 조작인데 혹시 모르니 남겨두자
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        if (CloneBlock == null)
        { 
            CloneBlock = Instantiate(gameObject, StartPos, transform.rotation) as GameObject; ///블록생성
            CloneBlock.transform.SetParent(ParentPos); ///다시 원래 부모 오브젝트로 돌아감
            CloneBlock.transform.localScale = new Vector3(1f, 1f, 1f); ///scale값 변경
            if(CloneBlock.CompareTag("For"))
            {
                CloneBlock.transform.localScale = new Vector3(1f, 3f, 1f); ///scale값 변경 for문은 어쩔수 없이 이걸로 해야함
            }
            GetComponent<Image>().raycastTarget = true;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        startPosition = transform.position;
        diffPosition = Input.mousePosition - startPosition;
        EventSystem.current.SetSelectedGameObject(gameObject);
        EventSystem.current.currentSelectedGameObject.transform.SetParent(canvas_.transform);
        EventSystem.current.currentSelectedGameObject.transform.SetSiblingIndex(2);
        //Debug.Log("start drag " + gameObject.name);
    }
}
