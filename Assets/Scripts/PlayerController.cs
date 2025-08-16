using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerController : NetworkBehaviour
{
    [Header("XR Device Scripts")]
    public ViveHMD hmdScripts;
    public ViveController leftController, rightController;
    public UltimateTracker waistTracker, leftLegTracker, rightLegTracker;
    

    public void ReferIkObjectToReference(List<GameObject> ikObjects)
    {
        waistTracker.target = ikObjects[0];
        hmdScripts.target = ikObjects[1];
        leftController.target = ikObjects[2];
        leftController.hint = ikObjects[3];
        rightController.target = ikObjects[4];
        rightController.hint = ikObjects[5];
        leftLegTracker.target = ikObjects[6];
        leftLegTracker.hint = ikObjects[7];
        rightLegTracker.target = ikObjects[8];
        rightLegTracker.hint = ikObjects[9];
    }
}
