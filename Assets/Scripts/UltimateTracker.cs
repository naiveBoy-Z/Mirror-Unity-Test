using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Wave.OpenXR;
using Wave.Essence.Tracker;
using TMPro;


public enum UltimateTrackerRole
{
    Waist = (int)InputDeviceTracker.TrackerRole.Waist,
    Right_Ankle = (int)InputDeviceTracker.TrackerRole.Ankle_Right,
    Left_Ankle = (int)InputDeviceTracker.TrackerRole.Ankle_Left
}


public class UltimateTracker : MonoBehaviour
{
    public UltimateTrackerRole trackerRole = UltimateTrackerRole.Waist;
    [Header("IK Source Object")]
    public GameObject target;
    public GameObject hint;
    [Header("IK Object Reference")]
    public GameObject targetReference;
    public GameObject hintReference;

    InputDeviceTracker.TrackerId trackerId = InputDeviceTracker.TrackerId.Tracker1;
    bool completeCalibrating;
    Vector3 position;
    Quaternion rotation;

    void Start()
    {
        completeCalibrating = false;

        foreach (InputDeviceTracker.TrackerId id in System.Enum.GetValues(typeof(InputDeviceTracker.TrackerId)))
        {
            if ((int)InputDeviceTracker.GetRole(id) == (int)trackerRole &&
                InputDeviceTracker.IsAvailable(id) &&
                InputDeviceTracker.IsTracked(id))
            {
                trackerId = id;
                break;
            }
        }
    }


    void Update()
    {
        if (completeCalibrating)
        {
            if (InputDeviceTracker.GetPosition(trackerId, out position))
            {
                transform.localPosition = position;
                target.transform.position = targetReference.transform.position;
                if (trackerRole != UltimateTrackerRole.Waist)
                {
                    hint.transform.position = hintReference.transform.position;
                }
            }

            if (InputDeviceTracker.GetRotation(trackerId, out rotation))
            {
                transform.localRotation = rotation;
                target.transform.rotation = targetReference.transform.rotation;
            }
        }
    }


    public void StartGettingTrackingData()
    {
        completeCalibrating = true;
    }


    public Vector3 GetCurrentPosition()
    {
        if (InputDeviceTracker.GetPosition(trackerId, out position))
        {
            return position;
        }
        return Vector3.zero;
    }
}
