using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionClass : MonoBehaviour
{
    public GameObject Fpanel; //Fpanel
    public GameObject Block3; //Block3
    public GameObject Block4; //Block4
    public GameObject Class; //Class Panel
    public GameObject PML; //Panel Main Loop
    public GameObject ClassBlock; //Class Block
    public GameObject SaveFx; //SaveFx
    GameObject Child;

    IEnumerator FxBox() // 함수에 들어가는 블록들을 저장해두는 곳
    {
        int nSize = transform.childCount;
        for (int i = 0; i < nSize; i++)
        {
            Child = transform.GetChild(i).gameObject; // Panel Main Loop의 자식오브젝트 저장
            FunctionMove FunctionMove = Child.GetComponent<FunctionMove>();
            FunctionRotate FunctionRotate = Child.GetComponent<FunctionRotate>();

            if (FunctionMove)   //FunctionMove 즉, 이동 블록일때 실행
            {
                yield return StartCoroutine(FunctionMove.MoveZ()); //z축 1 이동

            }
            else if (!FunctionMove) // 회전 블록 일때 실행
            {
                if (Child.CompareTag("Rotate_R")) //오른쪽 회전
                    yield return StartCoroutine(FunctionRotate.RightRotate());
                else if (Child.tag == "Rotate_L") //왼쪽 회전
                    yield return StartCoroutine(FunctionRotate.LeftRotate());
                else if (Child.tag == "Rotate_B") //오른쪽 회전
                    yield return StartCoroutine(FunctionRotate.BackRotate());
            }
        }

    }
    public void MakeFunction() //함수 만들기
    {
        Fpanel.SetActive(true);
        Class.SetActive(true);
        PML.SetActive(false);
        SaveFx.SetActive(true);
    }
    public void SaveFunction() //함수 저장하기
    {
        Fpanel.SetActive(false);
        Block4.SetActive(true);
        Class.SetActive(false);
        PML.SetActive(true);
        GameObject CloneBlock = Instantiate(ClassBlock, Block4.transform.position, transform.rotation) as GameObject; ///블록생성
        CloneBlock.transform.localScale = new Vector3(2.77f, 3, 1); ///scale값 변경
        CloneBlock.transform.SetParent(Block4.transform); ///다시 원래 부모 오브젝트로 돌아감
        Block3.SetActive(false);
        Debug.Log("1");

    }

    public IEnumerator Function() //FBox()에 저장된 블록들을 실행. 
    { 
        yield return StartCoroutine(FxBox());
       
    }
}
