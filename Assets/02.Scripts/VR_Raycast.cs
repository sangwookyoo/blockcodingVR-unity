using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Valve.VR;
using Valve.VR.Extras;
using UnityEngine.EventSystems;

namespace Codezero
{
    public class VR_Raycast : MonoBehaviour
    {
        public AudioClip EffectSound;
        public AudioSource audioSource;

        private SteamVR_LaserPointer laserPointer;

        Vector3 startPosition;
        Vector3 diffPosition;
        GameObject canvas_;
        Transform ParentPos;
        Vector3 StartPos;
        FunctionMove FunctionMove;
        FunctionRotate functionRotate;
        int Mnum, Rnum;
        GameObject CloneBlock;

        private void OnEnable()
        {
            laserPointer = gameObject.GetComponent<SteamVR_LaserPointer>();

            // 이벤트 할당
            laserPointer.PointerIn += OnPointerEnter;
            laserPointer.PointerOut += OnPointerExit;
            laserPointer.PointerClick += OnPointerClick;
            laserPointer.PointerDown += OnPointerDown;
            laserPointer.Drag += OnDrag;
            laserPointer.EndDrag += OnEndDrag;
            laserPointer.Drop += OnDrop;
        }

        private void OnDisable()
        {
            // 이벤트 연결 해제
            laserPointer.PointerIn -= OnPointerEnter;
            laserPointer.PointerOut -= OnPointerExit;
            laserPointer.PointerClick -= OnPointerClick;
            laserPointer.PointerDown -= OnPointerDown;
            laserPointer.Drag -= OnDrag;
            laserPointer.EndDrag -= OnEndDrag;
            laserPointer.Drop -= OnDrop;
        }

        //레이저 포인터가 들어갔을 경우
        void OnPointerEnter(object sender, PointerEventArgs e)
        {
            IPointerEnterHandler enterHandler = e.target.GetComponent<IPointerEnterHandler>();
            if (enterHandler == null) return;

            enterHandler.OnPointerEnter(new PointerEventData(EventSystem.current));

            // UI 인터렉션 사운드
            if (GameObject.FindWithTag("UI")) {
                audioSource.PlayOneShot(EffectSound);
            }
        }

        // 레이저 포인터가 나갔을경우
        void OnPointerExit(object sender, PointerEventArgs e)
        {
            IPointerExitHandler exitHandler = e.target.GetComponent<IPointerExitHandler>();
            if (exitHandler == null) return;

            exitHandler.OnPointerExit(new PointerEventData(EventSystem.current));
        }

        //트리커 버튼을 클릭했을경우
        void OnPointerClick(object sender, PointerEventArgs e)
        {
            IPointerClickHandler clickHandler = e.target.GetComponent<IPointerClickHandler>();
            if (clickHandler == null) return;

            clickHandler.OnPointerClick(new PointerEventData(EventSystem.current));
        }

        // 포인터 다운했을 경우
        void OnPointerDown(object sender, PointerEventArgs e)
        {
            IPointerDownHandler downHandler = e.target.GetComponent<IPointerDownHandler>();
            if (downHandler == null) return;

            downHandler.OnPointerDown(new PointerEventData(EventSystem.current));
        }

        // 드래그했을 경우
        void OnDrag(object sender, PointerEventArgs e)
        {
            IDragHandler dragHandler = e.target.GetComponent<IDragHandler>();
            if (dragHandler == null) return;

            dragHandler.OnDrag(new PointerEventData(EventSystem.current));
        }

        // 드래그가 끝났을 경우
        void OnEndDrag(object sender, PointerEventArgs e)
        {
            IEndDragHandler endHandler = e.target.GetComponent<IEndDragHandler>();
            if (endHandler == null) return;

            endHandler.OnEndDrag(new PointerEventData(EventSystem.current));
        }

        // 드랍했을 경우
        void OnDrop(object sender, PointerEventArgs e)
        {
            IDropHandler dropHandler = e.target.GetComponent<IDropHandler>();
            if (dropHandler == null) return;

            dropHandler.OnDrop(new PointerEventData(EventSystem.current));
        }
    }
}