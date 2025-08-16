using DG.Tweening;
using Mirror;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Windows;
using UnityEngine.XR;
using Wave.OpenXR;


public class MenuManager : MonoBehaviour
{
    public static MenuManager Instance { get; private set; }

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
    }

    public TextMeshProUGUI test;
    [Header("Model and Behaviour")]
    public GameObject playerModel;
    public Transform cameraOffsetTransform;
    public List<Transform> playerModelBodyParts = new();
    public List<UltimateTracker> ultimateTrackers = new();
    [Header("UI in Menu")]
    public TextMeshProUGUI txtTrackerConnectionError;
    public GameObject gameName;
    public GameObject pnlPoseCalibrator, pnlNetworkConnector;
    public GameObject btnCalibrate;
    public GameObject calibrationGuide1, calibrationGuide2;
    public Image leftTriggerIndicator, rightTriggerIndicator;
    public Sprite triggerPressed, triggerReleased;
    public Image calibrationBackgroundBar, calibrationForegroundBar;
    [Header("Player")]
    public PlayerInfor localPlayer;
    public List<GameObject> otherPlayers;
    [Header("Manager")]
    public PlayersManager playersManager;
    [Header("Role Selector UI")]
    public GameObject pnlRoleSelector;
    [Header("Clients List UI")]
    public GameObject pnlLobby;
    public GameObject pnlPlayerList;
    public GameObject prefabPlayerInforInList;
    [Header("Endgame UI")]
    public GameObject pnlEndgame;
    public TextMeshProUGUI txtResult;

    int statusCode = 0;
    UnityEngine.XR.InputDevice leftController, rightController;
    public bool isCalibrating, completeCalibrating;

    

    private void Start()
    {
        HideObjectsAndDisableComponentsAtStartUp();
        GetXRDevices();

        isCalibrating = false;
        completeCalibrating = false;
        StartCoroutine(StartConnection());
    }
    IEnumerator StartConnection()
    {
        yield return new WaitForSeconds(2);
        GameObject.Find("Player XR Origin (XR Rig)").SetActive(false);
        playersManager.networkAddress = "192.168.0.61";
        playersManager.StartHost();
    }


    private void Update()
    {
        if (statusCode == 1)
        {
            if (leftController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool isPressedLeft) &&
                rightController.TryGetFeatureValue(UnityEngine.XR.CommonUsages.triggerButton, out bool isPressedRight))
            {
                if (isPressedLeft)
                {
                    leftTriggerIndicator.sprite = triggerPressed;
                }
                else
                {
                    leftTriggerIndicator.sprite = triggerReleased;
                }

                if (isPressedRight)
                {
                    rightTriggerIndicator.sprite = triggerPressed;
                }
                else
                {
                    rightTriggerIndicator.sprite = triggerReleased;
                }

                if (isPressedLeft && isPressedRight)
                {
                    calibrationBackgroundBar.gameObject.SetActive(true);
                    calibrationForegroundBar.gameObject.SetActive(true);
                    if (!isCalibrating)
                    {
                        isCalibrating = true;
                        StartCoroutine(CalibrateModelPose());
                        StartCoroutine(LoadingCalibratingBar());
                    }
                }
                else
                {
                    calibrationBackgroundBar.gameObject.SetActive(false);
                    calibrationForegroundBar.gameObject.SetActive(false);
                    isCalibrating = false;
                }
            }
        }
    }


    private void HideObjectsAndDisableComponentsAtStartUp()
    {
        playerModel.SetActive(false);
        txtTrackerConnectionError.gameObject.SetActive(false);
    }


    private void GetXRDevices()
    {
        InputDeviceCharacteristics kControllerLeftCharacteristics = (
            InputDeviceCharacteristics.Left |
            InputDeviceCharacteristics.TrackedDevice |
            InputDeviceCharacteristics.Controller |
            InputDeviceCharacteristics.HeldInHand
        );
        InputDeviceCharacteristics kControllerRightCharacteristics = (
            InputDeviceCharacteristics.Right |
            InputDeviceCharacteristics.TrackedDevice |
            InputDeviceCharacteristics.Controller |
            InputDeviceCharacteristics.HeldInHand
        );
        List<UnityEngine.XR.InputDevice> inputDevices = new();
        InputDevices.GetDevices(inputDevices);
        if (inputDevices != null && inputDevices.Count > 0)
        {
            for (int i = 0; i < inputDevices.Count; i++)
            {
                if (inputDevices[i].characteristics.Equals(kControllerLeftCharacteristics))
                {
                    leftController = inputDevices[i];
                }
                else if (inputDevices[i].characteristics.Equals(kControllerRightCharacteristics))
                {
                    rightController = inputDevices[i];
                }
            }
        }
    }


    public void CalibrateTracker(GameObject triggeredButton)
    {
        bool connectedAllTrackers = true;
        string notConnectingTrackerList = "";
        bool[] roles = new bool[3];

        foreach (InputDeviceTracker.TrackerId id in System.Enum.GetValues(typeof(InputDeviceTracker.TrackerId)))
        {
            if (InputDeviceTracker.IsAvailable(id))
            {
                switch (InputDeviceTracker.GetRole(id))
                {
                    case InputDeviceTracker.TrackerRole.Waist:
                        roles[0] = true;
                        break;
                    case InputDeviceTracker.TrackerRole.Ankle_Left:
                        roles[1] = true;
                        break;
                    case InputDeviceTracker.TrackerRole.Ankle_Right:
                        roles[2] = true;
                        break;
                }
            }
        }

        if (!roles[0])
        {
            notConnectingTrackerList += "\n   • WAIST Tracker";
            connectedAllTrackers = false;
        }
        if (!roles[1])
        {
            notConnectingTrackerList += "\n   • LEFT ANKLE Tracker";
            connectedAllTrackers = false;
        }
        if (!roles[2])
        {
            notConnectingTrackerList += "\n   • RIGHT ANKLE Tracker";
            connectedAllTrackers = false;
        }

        if (!connectedAllTrackers)
        {
            triggeredButton.SetActive(true);
            txtTrackerConnectionError.gameObject.SetActive(true);
            txtTrackerConnectionError.text = $"⚠ Not found {notConnectingTrackerList}\nInspect all trackers thoroughly and calibrate again!";
        }
        else // calibrate model's pose
        {
            txtTrackerConnectionError.gameObject.SetActive(false);
            DisplayObjectInCalibratingPosePhase();
        }
    }



    private IEnumerator CalibrateModelPose()
    {
        float scaleFactor = GameObject.Find("Head Target Reference").transform.position.y / playerModelBodyParts[0].position.y;

        yield return null;
        if (!isCalibrating) yield break;

        float hipOffset = ultimateTrackers[0].GetCurrentPosition().y - 0.08f - playerModelBodyParts[1].position.y * scaleFactor;

        yield return null;
        if (!isCalibrating) yield break;

        float footOffset = -hipOffset / 2;
        float spineOffset = GameObject.Find("Left Hand Target Reference").transform.position.y - playerModelBodyParts[11].position.y * scaleFactor;

        yield return null;
        if (!isCalibrating) yield break;

        float headOffset = -spineOffset;
        spineOffset -= hipOffset;

        yield return null;
        if (!isCalibrating) yield break;

        Vector3 rightControllerPosition = GameObject.Find("Right Hand Target Reference").transform.position;
        float armOffset = Vector2.Distance(
                                new Vector2(rightControllerPosition.x, rightControllerPosition.z),
                                new Vector2(ultimateTrackers[0].GetCurrentPosition().x, ultimateTrackers[0].GetCurrentPosition().z)
                            )
                        - playerModelBodyParts[15].position.x * scaleFactor;


        yield return null;
        if (!isCalibrating) yield break;

        float armRatio = 8 / 13, forearmRatio = 5 / 13;

        while (!completeCalibrating)
        {
            if (!isCalibrating) yield break;
            yield return null;
        }

        ////body center:
        //playerModelBodyParts[1].position = playerModelBodyParts[1].position + new Vector3(0, hipOffset, 0);
        //// foot:
        //playerModelBodyParts[2].position = playerModelBodyParts[2].position + new Vector3(0, footOffset, 0);
        //playerModelBodyParts[3].position = playerModelBodyParts[3].position + new Vector3(0, footOffset, 0);
        //playerModelBodyParts[4].position = playerModelBodyParts[4].position + new Vector3(0, footOffset, 0);
        //playerModelBodyParts[5].position = playerModelBodyParts[5].position + new Vector3(0, footOffset, 0);
        //playerModelBodyParts[16].position = playerModelBodyParts[16].position + new Vector3(0, footOffset * 2, 0);
        //playerModelBodyParts[17].position = playerModelBodyParts[17].position + new Vector3(0, footOffset, 0);
        //playerModelBodyParts[18].position = playerModelBodyParts[18].position + new Vector3(0, footOffset * 2, 0);
        //playerModelBodyParts[19].position = playerModelBodyParts[19].position + new Vector3(0, footOffset, 0);
        //// upper body:
        //playerModelBodyParts[6].position = playerModelBodyParts[6].position + new Vector3(0, spineOffset / 3, 0);
        //playerModelBodyParts[7].position = playerModelBodyParts[7].position + new Vector3(0, spineOffset / 3, 0);
        //playerModelBodyParts[8].position = playerModelBodyParts[8].position + new Vector3(0, spineOffset / 3, 0);
        //playerModelBodyParts[9].position = playerModelBodyParts[9].position + new Vector3(0, headOffset / 2, 0);
        //playerModelBodyParts[10].position = playerModelBodyParts[10].position + new Vector3(0, headOffset / 2, 0);
        //// arm:
        //playerModelBodyParts[12].position = playerModelBodyParts[12].position + new Vector3(-armOffset * armRatio, 0, 0);
        //playerModelBodyParts[13].position = playerModelBodyParts[13].position + new Vector3(-armOffset * forearmRatio, 0, 0);
        //playerModelBodyParts[14].position = playerModelBodyParts[14].position + new Vector3(armOffset * armRatio, 0, 0);
        //playerModelBodyParts[15].position = playerModelBodyParts[15].position + new Vector3(armOffset * forearmRatio, 0, 0);
        //playerModelBodyParts[20].position = playerModelBodyParts[20].position + new Vector3(-armOffset * forearmRatio, spineOffset, 0);
        //playerModelBodyParts[21].position = playerModelBodyParts[21].position + new Vector3(-armOffset * forearmRatio, spineOffset, 0);
        //playerModelBodyParts[22].position = playerModelBodyParts[22].position + new Vector3(armOffset * forearmRatio, spineOffset, 0);
        //playerModelBodyParts[23].position = playerModelBodyParts[23].position + new Vector3(armOffset * forearmRatio, spineOffset, 0);

        NetworkManager.singleton.GetComponent<PlayersManager>().bodyPartsOffsets = new List<float> {
            scaleFactor, hipOffset, footOffset,
            spineOffset, headOffset, armOffset
        };

        cameraOffsetTransform.position = cameraOffsetTransform.position + new Vector3(0, 0, 3);
        //playerModel.transform.localScale = new Vector3(scaleFactor, scaleFactor, scaleFactor);
        playerModel.GetComponent<Animator>().enabled = true;
        ultimateTrackers[0].GetComponent<TrackerPoseIK>().enabled = true;
    }


    public IEnumerator LoadingCalibratingBar()
    {
        float timestamp = 0f;
        float vibratingMoment = 0f;
        while (isCalibrating && timestamp < 3f)
        {
            timestamp += Time.deltaTime;
            vibratingMoment += Time.deltaTime;
            calibrationForegroundBar.fillAmount = timestamp / 3;
            if (vibratingMoment >= 0.3f)
            {
                leftController.SendHapticImpulse(0, 0.1f, 0.1f);
                rightController.SendHapticImpulse(0, 0.1f, 0.1f);
                vibratingMoment = 0f;
            } 
            yield return null;
        }

        if (timestamp >= 3f)
        {
            completeCalibrating = true;
            statusCode = 2;
            HideObjectInCalibratingPosePhase();
        }
        else
        {
            calibrationForegroundBar.fillAmount = 0;
        }
    }


    private void DisplayObjectInCalibratingPosePhase()
    {
        btnCalibrate.SetActive(false);

        playerModel.SetActive(true);
        pnlPoseCalibrator.SetActive(true);

        statusCode = 1;
    }

    private void HideObjectInCalibratingPosePhase()
    {
        Destroy(GameObject.Find("Left Hand"));
        Destroy(GameObject.Find("Right Hand"));
        gameName.SetActive(false);
        pnlPoseCalibrator.SetActive(false);

        pnlNetworkConnector.SetActive(true);
    }



    public void VibrateController(int id)
    {
        if (id == 0)
        {
            leftController.SendHapticImpulse(0, 0.1f, 0.1f);
        }
        else
        {
            rightController.SendHapticImpulse(0, 0.1f, 0.1f);
        }
    }

    // ------------------------------------------------------------------------------------------------------------------- //
    //                                             NETWORK CONNECT UI
    // ------------------------------------------------------------------------------------------------------------------- //
    #region Network Connect UI
    public void StartHost()
    {
        //GameObject.Find("Player XR Origin (XR Rig)").SetActive(false);
        //GameObject.Find("Swat").SetActive(false);
        playersManager.StartHost();
        pnlRoleSelector.SetActive(false);
        pnlLobby.SetActive(true);
    }

    public void StartClient()
    {
        //GameObject.Find("Player XR Origin (XR Rig)").SetActive(false);
        //GameObject.Find("Swat").SetActive(false);

        playersManager.networkAddress = "192.168.0.60";
        
        playersManager.StartClient();
        pnlRoleSelector.SetActive(false);
        pnlLobby.SetActive(true);
    }

    public void StopClient()
    {
        pnlRoleSelector.SetActive(true);
        pnlLobby.SetActive(false);
        if (NetworkServer.active)
        {
            foreach (Transform child in pnlPlayerList.transform)
            {
                Destroy(child.gameObject);
            }
            playersManager.StopHost();
            playersManager.players.Clear();
            playersManager.playerReady = 0;
            otherPlayers.Clear();
        }
        else
        {
            playersManager.StopClient();
        }
    }


    public void ReadyToPlay()
    {
        localPlayer.CmdUpdateReadyState();
    }


    public void UpdateReadyState(int index, bool on)
    {
        pnlPlayerList.transform.GetChild(index).GetChild(0).gameObject.SetActive(on);
    }


    public void UpdatePlayerList(List<PlayerData> players)
    {
        test.text = players.Count.ToString();
        foreach (Transform child in pnlPlayerList.transform)
        {
            Destroy(child.gameObject);
        }

        foreach (PlayerData player in players)
        {
            GameObject playerInfo = Instantiate(prefabPlayerInforInList);
            playerInfo.transform.SetParent(pnlPlayerList.transform, false);

            string infor = player.playerName;
            if (player.playerName == localPlayer.playerName) infor += " <i><color=yellow>(YOU)</color></i>";
            infor += $"\n<color=green>[{player.deviceName}]</color>";
            playerInfo.GetComponent<TextMeshProUGUI>().text = infor;

            if (player.isReady) playerInfo.transform.GetChild(0).gameObject.SetActive(true);
        }
    }


    public void DisplayOtherPlayers()
    {
        pnlLobby.SetActive(false);

        GameObject.Find("Player XR Origin (XR Rig)").SetActive(false);
        GameObject.Find("Swat").SetActive(false);

        localPlayer.gameObject.SetActive(true);
        foreach (var player in otherPlayers)
        {
            player.SetActive(true);
        }

        for (int i = otherPlayers.Count - 1; i >= 0; i--)
        {
            if (otherPlayers[i] == null)
            {
                otherPlayers.RemoveAt(i);
            }
            else
            {
                otherPlayers[i].SetActive(true);
            }
        }
    }


    public void DisplayFinalResult(bool win)
    {
        StartCoroutine(DisplayFinalResultCoroutine(win));
    }

    IEnumerator DisplayFinalResultCoroutine(bool win)
    {
        pnlEndgame.SetActive(true);
        if (win)
        {
            txtResult.text = "You WIN :)))";
        }
        else
        {
            txtResult.text = "You LOSE :(((";
        }

        yield return new WaitForSeconds(2);
        pnlRoleSelector.SetActive(true);
        pnlEndgame.SetActive(false);

        StopClient();
    }
    #endregion
}
