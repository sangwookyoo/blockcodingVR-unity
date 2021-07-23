using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionRotate : MonoBehaviour
{
    public bool rotateCheck = false;
    public int num;
    public Animator rotate;
    public Transform sunrise;
    public Transform sunset;
    public float journeyTime = 1.0F;
    private float startTime;

    public bool moveCheck = false;
    public Vector3 StartPos;
    public Animator Move;
    public void Start()
    {
        num = 1;
        startTime = Time.time;
    }
    //public IEnumerator RightRotate()
    //{
    //    GameObject player;
    //    player = GameObject.FindGameObjectWithTag("Player");
    //    Vector3 startAngle = player.transform.eulerAngles;
    //    Debug.Log(startAngle);
    //    startAngle.y += 90;
    //    Vector3 endAngle = startAngle;
    //    Debug.Log(endAngle);
    //    float checkTime = 0;
    //    float directionTime = 1;    // 연출시간
    //    while (checkTime < directionTime)
    //    {
    //        yield return new WaitForFixedUpdate();
    //        checkTime += Time.fixedDeltaTime;
    //        transform.transform.eulerAngles = Vector3.Lerp(startAngle, endAngle, checkTime / directionTime);
    //    }
    //    yield return new WaitForSeconds(0.3f);
    //}


    public IEnumerator MoveZ()
    {
        GameObject player;
        player = GameObject.FindGameObjectWithTag("Player");
        StartPos = player.transform.position;
        Animator animator = player.GetComponent<Animator>();
        animator.SetBool("Walk", true);
        FunctionDeath FunctionDeath = FindObjectOfType<FunctionDeath>();
        while (true)
        {
            player.transform.Translate(0, 0, 5 * Time.deltaTime * 0.7f);
            if (FunctionDeath.PlayerDeath == true)
            {
                break;
            }
            if ((Mathf.Abs(StartPos.z - player.transform.position.z) >= 5)
                || (Mathf.Abs(StartPos.x - player.transform.position.x) >= 5)
                || (Mathf.Abs(StartPos.y - player.transform.position.y) >= 5))
            {
                animator.SetBool("Walk", false);
                break;
            }
            yield return null;
        }
    }

    public IEnumerator RightRotate() ///오른쪽으로 90도 회전
    {
        GameObject player;
        player = GameObject.FindGameObjectWithTag("Player");
        rotate = player.GetComponent<Animator>();
        rotate.SetBool("TurnRight", true);
        float checkTime = 0;
        float directionTime = 1;    // 연출시간
        while (true)
        {
            player.transform.Rotate(0, 90 * Time.deltaTime, 0);
            
            checkTime += Time.deltaTime;
            if(checkTime > directionTime)
            {
                rotate.SetBool("TurnRight", false);
                break;
            }
            yield return null;
        } 
    }

    public IEnumerator LeftRotate() ///왼쪽으로 90도 회전
    {
        GameObject player;
        player = GameObject.FindGameObjectWithTag("Player");
        rotate = player.GetComponent<Animator>();
        rotate.SetBool("TurnRight", true);
        float checkTime = 0;
        float directionTime = 1;    // 연출시간
        while (true)
        {
            player.transform.Rotate(0, -90 * Time.deltaTime, 0);

            checkTime += Time.deltaTime;
            if (checkTime > directionTime)
            {
                rotate.SetBool("TurnRight", false);
                break;
            }
            yield return null;
        }
        //yield return new WaitForSeconds(0.3f);
    }
    public IEnumerator BackRotate() ///오른쪽으로 180도 회전
    {
        GameObject player;
        player = GameObject.FindGameObjectWithTag("Player");
        rotate = player.GetComponent<Animator>();
        rotate.SetBool("TurnRight", true);
        float checkTime = 0;
        float directionTime = 1;    // 연출시간
        while (true)
        {
            player.transform.Rotate(0, 180 * Time.deltaTime, 0);

            checkTime += Time.deltaTime;
            if (checkTime > directionTime)
            {
                rotate.SetBool("TurnRight", false);
                break;
            }
            yield return null;
        }
        //yield return new WaitForSeconds(0.3f);
    }
}
