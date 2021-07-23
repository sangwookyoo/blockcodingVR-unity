using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionClass : MonoBehaviour
{
    
    public GameObject Fpanel; //Fpanel
    public GameObject Block3; //Block3
    public GameObject Block4; //Block4
    public GameObject Fx; //Class Panel
    public GameObject PML; //Panel Main Loop
    public GameObject ClassBlock; //Class Block
    public GameObject SaveFx; //SaveFx
    public GameObject MakeFx; //MakeFx
    public GameObject Block; //Block
    GameObject Child;

    IEnumerator FxBox() // 함수에 들어가는 블록들을 저장해두는 곳
    {
        int nSize = transform.childCount;
        for (int i = 0; i < nSize; i++)
        {
            Child = transform.GetChild(i).gameObject; 
            FunctionMove FunctionMove = Child.GetComponent<FunctionMove>(); 
            FunctionJump FunctionJump = Child.GetComponent<FunctionJump>();
            FunctionRotate FunctionRotate = Child.GetComponent<FunctionRotate>();  
            FunctionFor FunctionFor = Child.GetComponent<FunctionFor>(); 
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
            else if (!FunctionMove && !FunctionJump && !FunctionRotate && FunctionFor)// FOR문
            {
                yield return StartCoroutine(FunctionFor.For());
            }
            else if (!FunctionMove && !FunctionJump && !FunctionRotate && !FunctionFor && FunctionClass) //함수
            {
                yield return StartCoroutine(FunctionClass.Function());
            }
        }

    }
    public void MakeFunction() //함수 만들기
    {
        Fpanel.SetActive(true);
        //Fx.SetActive(true);
        CanvasGroup canvasGroup = Fx.GetComponent<CanvasGroup>();
        canvasGroup.alpha = 1;
        canvasGroup.blocksRaycasts = true;
        PML.SetActive(false);
        SaveFx.SetActive(true);
        Block.SetActive(true);
    }
    public void SaveFunction() //함수 저장하기
    {
        CanvasGroup canvasGroup = Fx.GetComponent<CanvasGroup>();
        Fpanel.SetActive(false);
        Block4.SetActive(true);
        PML.SetActive(true);
        GameObject CloneBlock = Instantiate(Fx, Block4.transform.position, transform.rotation) as GameObject; //Fx를 클론생성
        GameObject CloneBlock2 = Instantiate(ClassBlock, Block4.transform.position, transform.rotation) as GameObject; //ClassBlock를 블록생성
        //Fx.SetActive(false);
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
        CloneBlock2.transform.SetParent(Block4.transform);
        CloneBlock.transform.SetParent(CloneBlock2.transform); //Fx를 ClassBlock 자식 오브젝트로 넣음으로써 ClassBlock에 Fx 기능들을 전이 시킴
        CanvasGroup canvasGroup2 = CloneBlock.GetComponent<CanvasGroup>();
        canvasGroup2.alpha = 0; //CloneBlock.SetActive(false); 을 대신함
        canvasGroup2.blocksRaycasts = false; // 혹시 터치못하게끔 하기 위해
        //CloneBlock.SetActive(false); //Panel 상에서 보이지 않게끔 비활성화 시킴 > ClassBlock만 보여줌
        CloneBlock2.transform.localScale = new Vector3(1, 1, 1);
        Block3.SetActive(false);
        Block.SetActive(false);
    }

    public IEnumerator Function() //FBox()에 저장된 블록들을 실행. 
    { 
        yield return StartCoroutine(FxBox());
       
    }
}
