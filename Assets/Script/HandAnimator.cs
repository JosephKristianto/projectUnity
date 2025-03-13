using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class HandAnimator : MonoBehaviour
{
    // Start is called before the first frame update

    HandAnimationController controller;
    ActionBasedController xrController;
    public bool isGripping;
    public void  Setup()
    {
        controller = GetComponentInChildren<HandAnimationController>();
        xrController = GetComponent<ActionBasedController>();
    }

    private bool previousGripState = false;

    private void Update()
    {
        if (xrController != null)
        {
            float gripValue = xrController.selectAction.action.ReadValue<float>();
            bool currentGripState = gripValue > 0;

            // Only update and call the Grip function if the grip state has changed
            if (currentGripState != previousGripState)
            {
                isGripping = currentGripState;
                controller.Grip(isGripping);
                previousGripState = isGripping;
            }
        }
    }


}
