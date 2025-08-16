using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ViveHMD : MonoBehaviour
{
    public GameObject target;
    public GameObject targetReference;

    void Update()
    {
        target.transform.SetPositionAndRotation(targetReference.transform.position, targetReference.transform.rotation);
    }
}
