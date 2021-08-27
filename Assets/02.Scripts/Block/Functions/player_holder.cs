using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class player_holder : MonoBehaviour
{
    Vector3 startPos; //초기위치
    Quaternion StartRot; //초기회전
    GameObject Player; //플레이어
    GameObject Finish; //도착지점
    GameObject FailButton;
    Animator animator;
    #region 외부 함수 선언
    GameObject Blocks;

    #endregion
    public Image StartButton = null;
    public bool FinishCheck;
    private void Start()
    {
        FinishCheck = true;
        Player = GameObject.FindGameObjectWithTag("Player");
        Finish = GameObject.FindGameObjectWithTag("Finish");
        animator = Player.GetComponent<Animator>();
        startPos = Player.transform.position;
        StartRot = Player.transform.rotation;
        FailButton = GameObject.FindGameObjectWithTag("FailButton");
    }
    //Panel Main Loop 안에 자식들 순차적 접근
    public IEnumerator Go() //스타트W버튼을 눌렀을때 실행되는 코루틴함수
    {
        yield return new WaitForSeconds(1f); //1초 딜레이
        //Fail fail = FailButton.GetComponent<Fail>();
        //메인패널에 접근
        Blocks = GameObject.FindGameObjectWithTag("MainDrop").gameObject;//Panel Main Loop에 접근
        Debug.Log(Blocks.name);
        int nSize = Blocks.transform.childCount; //Panel Main Loop의 자식들 갯수
        for (int i = 0; i < nSize; i++)
        {

            Debug.Log("i : " + i);
            Debug.Log("nSize :" + nSize);
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
                yield return StartCoroutine(FunctionFor.For()); 
            }
            else if (!FunctionMove && !FunctionJump &&  !FunctionRotate && !FunctionFor && FunctionClass)
            {
                yield return StartCoroutine(FunctionClass.Function()); 
            }

        //    if (i == (nSize - 1)) //마지막 블록까지 모두 실행됐을때
        //    {
        //        if (Mathf.Abs(transform.position.x - Finish.transform.position.x) <= 1F && Mathf.Abs(Player.transform.position.z - Finish.transform.position.z) <= 1F) //도착지점에 도착하면 
        //        {
        //            animator.SetTrigger("Jump"); //점프
        //        }
        //        else // 도착지점에 도착하지 못하면
        //            fail.FailCheck = true; //실패
        //    }
        }
    }
   

    
    public void OnClickButton() //스타트 버튼 눌렀을때 실행
    {
        StartCoroutine(Go());
    }
}