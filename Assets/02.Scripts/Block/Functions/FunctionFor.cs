using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FunctionFor : MonoBehaviour
{
    public int count;
    
    int Size = 0;
    VerticalLayoutGroup verticalLayoutGroup;
     void Awake()
    {
        verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
    }
     void Update()
    {
        int nSize = transform.childCount;
        //Debug.Log("nsize : " + nSize);
        int dSize = nSize - Size;
        //Debug.Log("dsize : " + dSize);
        if (dSize >= 2)
        {
            //verticalLayoutGroup.padding.top += 15;
           //verticalLayoutGroup.padding.bottom += 15;
            dSize = dSize - 1;
            //Debug.Log("dsize2 : " + dSize);
        }
        dSize = dSize - 1;
        //Debug.Log("dsize3 : " + dSize);
    }
    
    IEnumerator ForBox() // For()에서 돌아갈 블록들을 여기에 한번 저장을 한다.
    {
        int nSize = transform.childCount;
        for (int i = 0; i < nSize; i++)
        {
            GameObject Child = transform.GetChild(i).gameObject; // Panel Main Loop의 자식오브젝트 저장
            FunctionMove FunctionMove = Child.GetComponent<FunctionMove>();  //자식오브젝트의 FunctionMove 함수 불러오기
            FunctionJump FunctionJump = Child.GetComponent<FunctionJump>();
            FunctionRotate FunctionRotate = Child.GetComponent<FunctionRotate>();  //자식오브젝트의 FunctionRotate 함수 불러오기
            FunctionFor FunctionFor = Child.GetComponent<FunctionFor>(); //자식오브젝트의 FunctionFor 함수 불러오기
            FunctionClass FunctionClass = Child.GetComponent<FunctionClass>();

            if (FunctionMove)   //FunctionMove 즉, 이동 블록일때 실행
            {
                yield return StartCoroutine(FunctionMove.MoveZ()); //z축 1 이동
            }
            else if (!FunctionMove && FunctionJump) // 회전 블록 일때 실행
            {
                if (Child.tag == "Jump") //점프
                    yield return StartCoroutine(FunctionJump.Jump());
            }
            else if (!FunctionMove && !FunctionJump && FunctionRotate) // 회전 블록 일때 실행
            {
                if (Child.tag == "Rotate_R") //오른쪽 회전
                    yield return StartCoroutine(FunctionRotate.RightRotate());
                else if (Child.tag == "Rotate_L") //왼쪽 회전
                    yield return StartCoroutine(FunctionRotate.LeftRotate());
                else if (Child.tag == "Rotate_B") //오른쪽 회전
                    yield return StartCoroutine(FunctionRotate.BackRotate());
            }
            else if (!FunctionMove && !FunctionJump && !FunctionRotate && FunctionFor)
            {
                yield return StartCoroutine(FunctionFor.For());
            }
            else if (!FunctionMove && !FunctionJump && !FunctionRotate && !FunctionFor && FunctionClass)
            {
                yield return StartCoroutine(FunctionClass.Function());
            }
        }
    }
    public IEnumerator For() //ForBox()에 저장된 블록들을 실행. 최종목표는 특정 스테이지가 끝날때까지 돌아가도록 하고 싶지만
    {                        //현재는 스테이지 없는 관계로 임의로 3번만 반복하도록 count를 추가함. 추후에 수정하면 됨
        for(count = 0; count < 20; count++)
        {
            yield return StartCoroutine(ForBox());
        }
    }

}
