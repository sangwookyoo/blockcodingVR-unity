using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FunctionFor : MonoBehaviour
{
    GameObject Blocks;
    public int count;
    // Start is called before the first frame update
    void Start()
    {
        
    }


    
    // public IEnumerator For() 이거는 캔버스를 온오프하는게 아니라 그냥 위치를 옮길때 이방법으로 하면 가능
    IEnumerator ForBox() // For()에서 돌아갈 블록들을 여기에 한번 저장을 한다.
    {
        int nSize = transform.childCount;
        for (int i = 0; i < nSize; i++)
        {
            GameObject Child = transform.GetChild(i).gameObject; // Panel Main Loop의 자식오브젝트 저장
            FunctionMove FunctionMove = Child.GetComponent<FunctionMove>();
            FunctionRotate FunctionRotate = Child.GetComponent<FunctionRotate>();
            
            if (FunctionMove)   //FunctionMove 즉, 이동 블록일때 실행
            {
                yield return StartCoroutine(FunctionMove.MoveZ()); //z축 1 이동

            }
            else if (!FunctionMove) // 회전 블록 일때 실행
            {
                if (Child.tag == "Rotate_R") //오른쪽 회전
                    yield return StartCoroutine(FunctionRotate.RightRotate());
                else if (Child.tag == "Rotate_L") //왼쪽 회전
                    yield return StartCoroutine(FunctionRotate.LeftRotate());
                else if (Child.tag == "Rotate_B") //오른쪽 회전
                    yield return StartCoroutine(FunctionRotate.BackRotate());
            }
        }
    }
    public IEnumerator For() //ForBox()에 저장된 블록들을 실행. 최종목표는 특정 스테이지가 끝날때까지 돌아가도록 하고 싶지만
    {                        //현재는 스테이지 없는 관계로 임의로 3번만 반복하도록 count를 추가함. 추후에 수정하면 됨
        for(count = 0; count < 3; count++)
        {
            yield return StartCoroutine(ForBox());
        }
    }

}
