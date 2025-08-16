// "Wave SDK 
// © 2020 HTC Corporation. All Rights Reserved.
//
// Unless otherwise required by copyright law and practice,
// upon the execution of HTC SDK license agreement,
// HTC grants you access to and use of the Wave SDK(s).
// You shall fully comply with all of HTC’s SDK license agreement terms and
// conditions signed by you and all SDK and API requirements,
// specifications, and documentation provided by HTC to You."

using UnityEngine;
using Wave.Essence.Tracker;
using Wave.Native;

namespace Wave.Essence.BodyTracking.Demo
{
	public class DevicesTracking : MonoBehaviour
	{
		const string LOG_TAG = "Wave.Essence.BodyTracking.Demo.DevicesTracking";
		void DEBUG(string msg)
		{
			if (Log.EnableDebugLog)
				Log.d(LOG_TAG, msg, true);
		}

		public enum ShowingDevice
		{
			DeviceID = 1,
			DeviceRole = 2,
		}

		#region Inspector
		public ShowingDevice Showing;

		public GameObject[] IDDevices = new GameObject[7];

		Pose RHPose;
		public GameObject RightHand = null;
		Pose LHPose;
		public GameObject LeftHand = null;
		Pose WPose;
		public GameObject Waist = null;
		Pose RFPose;
		public GameObject RightFoot = null;
		Pose LFPose;
		public GameObject LeftFoot = null;
		#endregion

		void Update()
		{
			if (TrackerManager.Instance == null) { return; }
			switch (Showing)
			{
				case ShowingDevice.DeviceID:
					UpdateDevicePoses();
					break;
				case ShowingDevice.DeviceRole:
					UpdateDevicePoseByRole();
					break;
			}
		}

		void UpdateDevicePoses()
		{
			for (int i = 0; i < IDDevices.Length && IDDevices.Length < TrackerUtils.s_TrackerIds.Length; i++)
			{
				if (TrackerManager.Instance.GetTrackerPosition(TrackerUtils.s_TrackerIds[i], out Vector3 pos))
				{
					IDDevices[i].transform.localPosition = pos;
				}
				if (TrackerManager.Instance.GetTrackerRotation(TrackerUtils.s_TrackerIds[i], out Quaternion quat))
				{
					IDDevices[i].transform.rotation = quat;
				}
			}
		}

		bool GetTrackerIDFromRole(TrackerRole role, out TrackerId id)
		{
			id = TrackerId.Tracker0;

			for (int i = 0; i < TrackerUtils.s_TrackerIds.Length; i++)
			{
				if (TrackerManager.Instance.GetTrackerRole(TrackerUtils.s_TrackerIds[i]) == role)
				{
					id = TrackerUtils.s_TrackerIds[i];
					return true;
				}
			}

			return false;
		}
		bool GetTrackerPoseFromRole(TrackerRole role, ref Pose pose)
		{
			if (GetTrackerIDFromRole(role, out TrackerId id))
			{
				pose.position = TrackerManager.Instance.GetTrackerPosition(id);
				pose.rotation = TrackerManager.Instance.GetTrackerRotation(id);
				//DEBUG("GetTrackerPoseFromRole() " + role + " id " + id + " (" + pose.position.x.ToString() + ", " + pose.position.y.ToString() + ", " + pose.position.z.ToString() + ")");
				return true;
			}
			return false;
		}
		void UpdateDevicePoseByRole()
		{
			// Wrist_Right
			if (RightHand != null)
			{
				if (GetTrackerPoseFromRole(TrackerRole.Wrist_Right, ref RHPose) || GetTrackerPoseFromRole(TrackerRole.Pair1_Right, ref RHPose))
				{
					RightHand.transform.localPosition = RHPose.position;
					RightHand.transform.localRotation = RHPose.rotation;
				}
			}
			// Wrist_Left
			if (LeftHand != null)
			{
				if (GetTrackerPoseFromRole(TrackerRole.Wrist_Left, ref LHPose) || GetTrackerPoseFromRole(TrackerRole.Pair1_Left, ref LHPose))
				{
					LeftHand.transform.localPosition = LHPose.position;
					LeftHand.transform.localRotation = LHPose.rotation;
				}
			}
			// Waist
			if (Waist != null)
			{
				if (GetTrackerPoseFromRole(TrackerRole.Waist, ref WPose))
				{
					Waist.transform.localPosition = WPose.position;
					Waist.transform.localRotation = WPose.rotation;
				}
			}
			// Ankle_Right
			if (RightFoot != null)
			{
				if (GetTrackerPoseFromRole(TrackerRole.Ankle_Right, ref RFPose))
				{
					RightFoot.transform.localPosition = RFPose.position;
					RightFoot.transform.localRotation = RFPose.rotation;
				}
			}
			// Ankle_Left
			if (LeftFoot != null)
			{
				if (GetTrackerPoseFromRole(TrackerRole.Ankle_Left, ref LFPose))
				{
					LeftFoot.transform.localPosition = LFPose.position;
					LeftFoot.transform.localRotation = LFPose.rotation;
				}
			}
		}
	}
}
