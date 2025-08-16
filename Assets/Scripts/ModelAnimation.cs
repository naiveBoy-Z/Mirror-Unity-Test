using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class ModelAnimation : MonoBehaviour
{
    public BoxCollider leftIndexFingerCollider, rightIndexFingerCollider;

    InputAction pressLeftGripAction, pressRightGripAction;
    Animator animator;

    private void Start()
    {
        pressLeftGripAction = new InputAction("pressLeftGrip", InputActionType.Button, "<ViveWaveController>{LeftHand}/gripPressed");
        pressLeftGripAction.Enable();
        pressRightGripAction = new InputAction("pressRightGrip", InputActionType.Button, "<ViveWaveController>{RightHand}/gripPressed");
        pressRightGripAction.Enable();

        animator = GetComponent<Animator>();
    }

    private void Update()
    {
        if (pressLeftGripAction.WasPressedThisFrame())
        {
            animator.SetBool("pressLeftGrip", true);
            leftIndexFingerCollider.enabled = true;
        }
        else if (pressLeftGripAction.WasReleasedThisFrame())
        {
            animator.SetBool("pressLeftGrip", false);
            leftIndexFingerCollider.enabled = false;
        }

        if (pressRightGripAction.WasPressedThisFrame())
        {
            animator.SetBool("pressRightGrip", true);
            rightIndexFingerCollider.enabled = true;
        }
        else if (pressRightGripAction.WasReleasedThisFrame())
        {
            animator.SetBool("pressRightGrip", false);
            rightIndexFingerCollider.enabled = false;
        }
    }
}
