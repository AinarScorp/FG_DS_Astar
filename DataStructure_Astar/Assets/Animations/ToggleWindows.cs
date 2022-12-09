using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ToggleWindows : MonoBehaviour
{
    [SerializeField] Animator animator;
    [SerializeField] bool windowIsOpen = true;

    public void ToggleWindow()
    {
        string trigger = windowIsOpen ? "Close" : "Open";
        windowIsOpen = !windowIsOpen;
        animator.SetTrigger(trigger);
    }
}
