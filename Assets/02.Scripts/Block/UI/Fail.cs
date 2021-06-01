using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fail : MonoBehaviour
{
    public GameObject Player;
    Animator animator;
    public bool FailCheck;
    public GameObject FailPanel;
    CanvasGroup canvasGroup;
    Vector3 startPos; //초기위치
    Quaternion StartRot; //초기회전 
    // Start is called before the first frame update
    void Start()
    {
        FailCheck = false;
        canvasGroup = FailPanel.GetComponent<CanvasGroup>();
        startPos = Player.transform.position;
        StartRot = Player.transform.rotation;
        animator = Player.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if(FailCheck)
        {
            animator.SetTrigger("Death");
            FailCheck = false;
            canvasGroup.alpha = 1;
            canvasGroup.blocksRaycasts = true;
        }
    }

    public void onClickFailButton()
    {
        Player.transform.position = startPos;
        Player.transform.rotation = StartRot;
        animator.Rebind(); //애니메이션 초기화
        FailCheck = false;
        canvasGroup.alpha = 0;
        canvasGroup.blocksRaycasts = false;
    }
}
