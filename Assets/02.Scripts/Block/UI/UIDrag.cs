using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Valve.VR.Extras;

namespace UIDRAG
{
    public class UIDrag : MonoBehaviour, IDragHandler, IEndDragHandler, IPointerDownHandler
    {
        Vector3 startPosition;
        Vector3 diffPosition;
        GameObject canvas_;
        Transform ParentPos;
        Vector3 StartPos;
        FunctionMove FunctionMove;
        FunctionRotate functionRotate;
        int Mnum, Rnum;
        GameObject CloneBlock;

        SteamVR_LaserPointer rayPosition;

        void Start()
        {
            FunctionMove = FindObjectOfType<FunctionMove>();
            functionRotate = FindObjectOfType<FunctionRotate>();
            ParentPos = transform.parent; ///부모 트랜스폼 저장
            StartPos = transform.position; ///처음위치
            canvas_ = GameObject.Find("Panel");

            rayPosition = FindObjectOfType<SteamVR_LaserPointer>();
        }

        public void OnDrag(PointerEventData eventData)
        {
            //Debug.Log(gameObject);

            // 마우스 좌표 인식해서 블록 이동
            Vector3 obj = Camera.main.WorldToScreenPoint(transform.position);
            Vector3 mouseposition = new Vector3(rayPosition.hitTransform.position.x, rayPosition.hitTransform.position.y, obj.z);
            Vector3 objPosition = Camera.main.ScreenToWorldPoint(mouseposition);
            transform.position = objPosition;
            //transform.position = rayPosition.hitTransform.position;
            transform.position = rayPosition.hitTransform.position - diffPosition; //기존 블록 조작인데 혹시 모르니 남겨두자

        }

        public void OnEndDrag(PointerEventData eventData)
        {
            Debug.Log("end drag");

            if (CloneBlock == null)
            {
                CloneBlock = Instantiate(gameObject, StartPos, transform.rotation) as GameObject; ///블록생성
                CloneBlock.transform.SetParent(ParentPos); ///다시 원래 부모 오브젝트로 돌아감
                CloneBlock.transform.localScale = new Vector3(1f, 1f, 1f); ///scale값 변경
            }
        }

        public void OnPointerDown(PointerEventData eventData)
        {
            startPosition = transform.position;
            diffPosition = rayPosition.hitTransform.position - startPosition;
            EventSystem.current.SetSelectedGameObject(gameObject);
            EventSystem.current.currentSelectedGameObject.transform.SetParent(canvas_.transform);
            EventSystem.current.currentSelectedGameObject.transform.SetSiblingIndex(2);
        }
    }
}