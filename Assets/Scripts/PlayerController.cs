using UnityEngine;
using Mirror;
using UnityEngine.InputSystem;
using System.Collections.Generic;

public class PlayerController : NetworkBehaviour
{
    [Header("XR Device Scripts")]
    public ViveHMD hmdScripts;
    public ViveController leftController, rightController;
    public TrackerPoseIK waistPose, leftLegPose, rightLegPose;
    

    public void ReferIkObjectToReference(List<GameObject> ikObjects)
    {
        waistPose.target = ikObjects[0];
        hmdScripts.target = ikObjects[1];
        leftController.target = ikObjects[2];
        leftController.hint = ikObjects[3];
        rightController.target = ikObjects[4];
        rightController.hint = ikObjects[5];
        leftLegPose.target = ikObjects[6];
        leftLegPose.hint = ikObjects[7];
        rightLegPose.target = ikObjects[8];
        rightLegPose.hint = ikObjects[9];
    }
}
