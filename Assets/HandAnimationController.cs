using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HandAnimationController : MonoBehaviour
{

    Animator animator;
    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        GetComponentInParent<HandAnimator>().Setup();
    }


    public void Grip(bool flag)
    {
        animator.SetBool("IsGrabbing", flag);

    }
}
