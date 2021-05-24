using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FunctionDeath : MonoBehaviour
{
    Animator animator;
    public bool DeathCheck;
    // Start is called before the first frame update
    void Start()
    {
        DeathCheck = false;
        animator = transform.GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Object"))
        {
            DeathCheck = true;
            transform.position = transform.position;
            animator.Play("Death");
        }
    }
}
