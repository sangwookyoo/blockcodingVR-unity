using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionJump : MonoBehaviour
{
   
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public IEnumerator Jump()
    {
        GameObject Player = GameObject.FindGameObjectWithTag("Player"); //애니메이터 움직일 캐릭터
        Animator Animator;
        Animator = Player.GetComponent<Animator>();
        Animator.SetTrigger("Jump");
        float checkTime = 0;
        float directionTime = 1;    // 연출시간
        
       // float directionTime2 = 2;    // 연출시간
        BoxCollider Collider = Player.GetComponent<BoxCollider>();
        Collider.enabled = false;
        Debug.Log("1" + Collider.enabled);
       
        while (true)
        {
            //Player.transform.Translate(0 , 0 , 5f * Time.deltaTime * 2);
            
            //Player.transform.Translate(0, -2 * Time.deltaTime, 0);
            checkTime += Time.deltaTime;
            if (checkTime > directionTime)
            {
                break;
                
            }
            yield return null;
        }
        
    }
}
