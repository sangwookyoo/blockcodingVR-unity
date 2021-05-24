using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class player_holder : MonoBehaviour
{
    #region 외부 함수 선언
    GameObject Blocks;

    #endregion
    public Image StartButton = null;

    //Panel Main Loop 안에 자식들 순차적 접근
    public IEnumerator Go() //스타트W버튼을 눌렀을때 실행되는 코루틴함수
    {
        yield return new WaitForSeconds(1f); //1초 딜레이
        ///메인패널에 접근

        //GameObject Blocks = GameObject.Find("Main/Canvas/Panel Main Loop").gameObject;
        Blocks = GameObject.Find("Canvas").transform.Find("Panel").transform.Find("Panel Main Loop").gameObject; //Panel Main Loop에 접근
        int nSize = Blocks.transform.childCount; //Panel Main Loop의 자식들 갯수
        for (int i = 0; i < nSize; i++)
        {
            GameObject Child = Blocks.transform.GetChild(i).gameObject; // Panel Main Loop의 자식오브젝트 저장
            ///블록의 함수들 다 불러오기
            FunctionMove FunctionMove = Child.GetComponent<FunctionMove>();  //자식오브젝트의 FunctionMove 함수 불러오기
            FunctionJump FunctionJump = Child.GetComponent<FunctionJump>();
            FunctionRotate FunctionRotate = Child.GetComponent<FunctionRotate>();  //자식오브젝트의 FunctionRotate 함수 불러오기
            FunctionFor FunctionFor = Child.GetComponent<FunctionFor> (); //자식오브젝트의 FunctionFor 함수 불러오기
            FunctionClass FunctionClass = Child.GetComponent<FunctionClass>();

            ///차례로 함수들이 있는지 체크하기
            if (FunctionMove)   //FunctionMove 즉, 이동 블록일때 실행
            {
                yield return StartCoroutine(FunctionMove.MoveZ()); //z축 1 이동
            }
            else if (!FunctionMove && FunctionJump) // 회전 블록 일때 실행
            {
                if (Child.tag == "Jump") //점프
                    yield return StartCoroutine(FunctionJump.Jump());
            }
            else if (!FunctionMove && !FunctionJump &&  FunctionRotate) // 회전 블록 일때 실행
            {
                if (Child.tag == "Rotate_R") //오른쪽 회전
                    yield return StartCoroutine(FunctionRotate.RightRotate());
                else if (Child.tag == "Rotate_L") //왼쪽 회전
                    yield return StartCoroutine(FunctionRotate.LeftRotate());
                else if (Child.tag == "Rotate_B") //오른쪽 회전
                    yield return StartCoroutine(FunctionRotate.BackRotate());
            }
            else if (!FunctionMove && !FunctionJump &&  !FunctionRotate && FunctionFor)
            {
                yield return StartCoroutine(FunctionFor.For()); //z축 1 이동
            }

            else if (!FunctionMove && !FunctionJump &&  !FunctionRotate && FunctionFor)
            {
                yield return StartCoroutine(FunctionFor.For()); //z축 1 이동
            }
            else if (!FunctionMove && !FunctionJump &&  !FunctionRotate && !FunctionFor && FunctionClass)
            {
                yield return StartCoroutine(FunctionClass.Function()); //z축 1 이동
            }

        }
    }

   
    public void OnClickButton() //스타트 버튼 눌렀을때 실행
    {
        StartCoroutine(Go());
        StartButton.enabled = false;
    }
}