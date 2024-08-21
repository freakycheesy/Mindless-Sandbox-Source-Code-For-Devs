using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ViewBobbing : MonoBehaviour
{
    public Animator animator;

    private void Start() {
        animator = GetComponent<Animator>();
    }

    void LateUpdate()
    {
        if (animator.GetBool("Reload"))
            return;
    }

}
