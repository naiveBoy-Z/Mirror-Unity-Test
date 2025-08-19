using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;


public class ViveController : MonoBehaviour
{
    [Header("IK Source Object")]
    public GameObject target;
    public GameObject hint;
    [Header("IK Object Reference")]
    public GameObject targetReference;
    public GameObject hintReference;


    void Update()
    {
        target.transform.SetPositionAndRotation(
            targetReference.transform.position,
            targetReference.transform.rotation
        );
    }
}
