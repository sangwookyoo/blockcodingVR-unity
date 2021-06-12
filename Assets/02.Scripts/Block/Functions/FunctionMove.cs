using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionMove : MonoBehaviour
{
    public bool moveCheck = false;
    public int num;
    public Vector3 StartPos;
    public Animator Move;

    public void Start()
    {
        
        num = 1;
    }

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
            if (FunctionDeath.PlayerDeath == true) // 죽었으면 그만 앞으로 가라
            {
                break;
            }
            if ((Mathf.Abs(StartPos.z - player.transform.position.z) >= 5) || (Mathf.Abs(StartPos.x - player.transform.position.x) >= 5) || (Mathf.Abs(StartPos.y - player.transform.position.y)>=5)) // 5 이동했으면 그만 멈춰라
            {
                animator.SetBool("Walk", false);
                break;
            }
            yield return null;
        }
        
        //yield return new WaitForSeconds(0.3f);
        //Debug.Log("Move");
    }

}