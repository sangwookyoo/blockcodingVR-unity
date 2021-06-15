using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionDeath : MonoBehaviour
{
    Animator animator;
    public bool PlayerDeath;//죽는 애니메이션 실행 bool
    public bool DeathCheck; //죽었는지 체크
    public bool FailCheck; //한번에 빨간장판까지 갔는지 못갔는지 체크
    Vector3 startPos; //초기위치
    Quaternion StartRot; //초기회전 
    void Start()
    {
        FailCheck = false;
        
        startPos = transform.position;
        StartRot = transform.rotation;
        PlayerDeath = false;
        //DeathCheck = false;
        animator = transform.GetComponent<Animator>();
        
    }

    // Update is called once per frame
    void Update()
    {
        if (DeathCheck && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >=0.9f )  //죽는 애니메이션이 끝나면
        {
            transform.position = startPos;
            transform.rotation = StartRot;
            animator.Rebind(); //애니메이션 초기화
            PlayerDeath = false;
            DeathCheck = false;
        }
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Object") || other.gameObject.CompareTag("Wall")) //장애물이나 벽에 닿으면
        {
            PlayerDeath = true; //true가 되면 앞으로 가는 애니메이션 종료됨
            animator.SetTrigger("Death"); //죽는 애니메이션 실행
            DeathCheck = true; //죽었음
        }
        
    }
}
