using UnityEngine;
using Wave.OpenXR;


public enum UltimateTrackerRole
{
    Waist = (int)InputDeviceTracker.TrackerRole.Waist,
    Right_Ankle = (int)InputDeviceTracker.TrackerRole.Ankle_Right,
    Left_Ankle = (int)InputDeviceTracker.TrackerRole.Ankle_Left
}


public class UltimateTracker : MonoBehaviour
{
    public UltimateTrackerRole trackerRole = UltimateTrackerRole.Waist;

    InputDeviceTracker.TrackerId trackerId = InputDeviceTracker.TrackerId.Tracker1;
    Vector3 position;
    Quaternion rotation;

    void Start()
    {
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
        if (InputDeviceTracker.GetPosition(trackerId, out position))
        {
            transform.localPosition = position;
        }

        if (InputDeviceTracker.GetRotation(trackerId, out rotation))
        {
            transform.localRotation = rotation;
        }
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
