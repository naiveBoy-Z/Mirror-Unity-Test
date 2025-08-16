using Mirror.BouncyCastle.Asn1.Mozilla;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrackerPoseIK : MonoBehaviour
{
    public bool isWaistTracker;
    [Header("IK Source Object")]
    public GameObject target;
    public GameObject hint;
    [Header("IK Object Reference")]
    public GameObject targetReference;
    public GameObject hintReference;

    void Update()
    {
        target.transform.position = targetReference.transform.position;
        if (!isWaistTracker)
        {
            hint.transform.position = hintReference.transform.position;
        }

        target.transform.rotation = targetReference.transform.rotation;
    }
}
