using UnityEngine;
using Mirror;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Collections;
using UnityEngine.InputSystem.XR;
using UnityEngine.XR.Interaction.Toolkit;

public class PlayerInfor : NetworkBehaviour
{
    #region Variable declaration
    [Header("Player Information Field")]
    [SyncVar] public int order;
    [SyncVar(hook = nameof(OnPlayerNameChanged))] public string playerName;
    [SyncVar] public bool isHost;
    [SyncVar(hook = nameof(OnPlayerDeviceChanged))] public string deviceName;
    [Header("Player's Stats")]
    public int maxHp = 100;
    [SyncVar] public int hp = 100;
    [Header("Prefabs")]
    public GameObject playerModelPrefab;
    public GameObject playerModel;
    public ModelController modelController;


    PlayersManager playersManager;
    #endregion

    #region When player object is spawned
    public override void OnStartClient()
    {
        base.OnStartClient();

        StartCoroutine(InitialModelPosition());
        playerModel = Instantiate(playerModelPrefab);
        modelController = playerModel.GetComponent<ModelController>();
        modelController.SetPlayerInforForColliders(this);
        GetComponent<PlayerController>().ReferIkObjectToReference(modelController.GetIkObjects());
        StartCoroutine(CalibrateModelPose());
        
        if (isLocalPlayer)
        {
            GameObject.Find("MENU CANVAS - Screen Space").GetComponent<Canvas>().worldCamera = transform.GetChild(0).GetChild(0).GetComponent<Camera>();
            MenuManager.Instance.localPlayer = this;
            MenuManager.Instance.localPlayerUI.SetActive(true);
            modelController.localPlayerCamera = transform.GetChild(0).GetChild(0);
            CmdSetPlayerDeviceName(SystemInfo.deviceModel);
            SyncList.instance.CmdAddBodyPartsOffsetsList(NetworkManager.singleton.GetComponent<PlayersManager>().bodyPartsOffsets);
        }
        else
        {
            DestroyUnnecessaryComponentOnOtherPlayer();
            StartCoroutine(AddOtherPlayersCanvasList());
            MenuManager.Instance.otherPlayers.Add(gameObject);
        }
    }
    IEnumerator InitialModelPosition()
    {
        while (order == 0)
        {
            yield return null;
        }
        transform.position = new Vector3(order - 1, 0, 0);
    }
    IEnumerator CalibrateModelPose()
    {
        while (SyncList.instance.bodyPartsOffsetsList.Count < order)
        {
            yield return null;
        }
        modelController.CalibrateModelPose(order);
    }
    IEnumerator AddOtherPlayersCanvasList()
    {
        while (MenuManager.Instance.localPlayer == null)
        {
            yield return null;
        }

        MenuManager.Instance.localPlayer.modelController.otherPlayersCanvas.Add(modelController.uiCanvas.transform);
    }
    #endregion

    public void OnDestroy()
    {
        Destroy(playerModel);
        SyncList.instance.bodyPartsOffsetsList.RemoveAt(order - 1);
    }


    #region Function that update local player's data
    [Command]
    public void CmdSetPlayerDeviceName(string pDeviceName)
    {
        deviceName = pDeviceName;
    }

    void OnPlayerNameChanged(string oldPlayerName, string newPlayerName)
    {
        StartCoroutine(DisplayPlayerStatsUI(newPlayerName));
    }
    IEnumerator DisplayPlayerStatsUI(string newPlayerName)
    {
        if (isLocalPlayer) yield break;
        while (modelController == null)
        {
            yield return null;
        }
        modelController.DisplayPlayerStatsUI(newPlayerName);
    }

    void OnPlayerDeviceChanged(string oldDeviceName, string newDeviceName)
    {
        deviceName = newDeviceName;
        if (isLocalPlayer) CmdAddNewPlayerToList(this);
    }


    [Command]
    public void CmdCalibrateModelPoseOnClients(List<float> bodyPartsOffsets)
    {
        RpcCalibrateModelPoseOnClients(bodyPartsOffsets);
    }

    [ClientRpc]
    public void RpcCalibrateModelPoseOnClients(List<float> bodyPartsOffsets)
    {
        List<Transform> bodyPartsTransform = playerModel.GetComponent<ModelController>().GetBodyPartsTransform();

        float armRatio = 8 / 13, forearmRatio = 5 / 13;

        //transform.localScale = new Vector3(bodyPartsOffsets[0], bodyPartsOffsets[0], bodyPartsOffsets[0]);
        //// body center:
        //bodyPartsTransform[1].position = bodyPartsTransform[1].position + new Vector3(0, bodyPartsOffsets[1], 0);
        //// foot:
        //bodyPartsTransform[2].position = bodyPartsTransform[2].position + new Vector3(0, bodyPartsOffsets[2], 0);
        //bodyPartsTransform[3].position = bodyPartsTransform[3].position + new Vector3(0, bodyPartsOffsets[2], 0);
        //bodyPartsTransform[4].position = bodyPartsTransform[4].position + new Vector3(0, bodyPartsOffsets[2], 0);
        //bodyPartsTransform[5].position = bodyPartsTransform[5].position + new Vector3(0, bodyPartsOffsets[2], 0);
        //bodyPartsTransform[16].position = bodyPartsTransform[16].position + new Vector3(0, bodyPartsOffsets[2] * 2, 0);
        //bodyPartsTransform[17].position = bodyPartsTransform[17].position + new Vector3(0, bodyPartsOffsets[2], 0);
        //bodyPartsTransform[18].position = bodyPartsTransform[18].position + new Vector3(0, bodyPartsOffsets[2] * 2, 0);
        //bodyPartsTransform[19].position = bodyPartsTransform[19].position + new Vector3(0, bodyPartsOffsets[2], 0);
        //// upper body:
        //bodyPartsTransform[6].position = bodyPartsTransform[6].position + new Vector3(0, bodyPartsOffsets[3] / 3, 0);
        //bodyPartsTransform[7].position = bodyPartsTransform[7].position + new Vector3(0, bodyPartsOffsets[3] / 3, 0);
        //bodyPartsTransform[8].position = bodyPartsTransform[8].position + new Vector3(0, bodyPartsOffsets[3] / 3, 0);
        //bodyPartsTransform[9].position = bodyPartsTransform[9].position + new Vector3(0, bodyPartsOffsets[4] / 2, 0);
        //bodyPartsTransform[10].position = bodyPartsTransform[10].position + new Vector3(0, bodyPartsOffsets[4] / 2, 0);
        //// arm:
        //bodyPartsTransform[12].position = bodyPartsTransform[12].position + new Vector3(-bodyPartsOffsets[5] * armRatio, 0, 0);
        //bodyPartsTransform[13].position = bodyPartsTransform[13].position + new Vector3(-bodyPartsOffsets[5] * forearmRatio, 0, 0);
        //bodyPartsTransform[14].position = bodyPartsTransform[14].position + new Vector3(bodyPartsOffsets[5] * armRatio, 0, 0);
        //bodyPartsTransform[15].position = bodyPartsTransform[15].position + new Vector3(bodyPartsOffsets[5] * forearmRatio, 0, 0);
        //bodyPartsTransform[20].position = bodyPartsTransform[20].position + new Vector3(-bodyPartsOffsets[5] * forearmRatio, bodyPartsOffsets[3], 0);
        //bodyPartsTransform[21].position = bodyPartsTransform[21].position + new Vector3(-bodyPartsOffsets[5] * forearmRatio, bodyPartsOffsets[3], 0);
        //bodyPartsTransform[22].position = bodyPartsTransform[22].position + new Vector3(bodyPartsOffsets[5] * forearmRatio, bodyPartsOffsets[3], 0);
        //bodyPartsTransform[23].position = bodyPartsTransform[23].position + new Vector3(bodyPartsOffsets[5] * forearmRatio, bodyPartsOffsets[3], 0);

        playerModel.GetComponent<Animator>().enabled = true;
        if (isLocalPlayer)
        {
            PlayerController playerController = GetComponent<PlayerController>();
            playerController.waistPose.enabled = true;
            playerController.leftLegPose.enabled = true;
            playerController.rightLegPose.enabled = true;
        }
    }
    #endregion


    public void DestroyUnnecessaryComponentOnOtherPlayer()
    {
        Transform mainCameraOfOtherXRRig = transform.GetChild(0).GetChild(0);
        Destroy(mainCameraOfOtherXRRig.GetComponent<Camera>());
        Destroy(mainCameraOfOtherXRRig.GetComponent<AudioListener>());
        Destroy(mainCameraOfOtherXRRig.GetComponent<TrackedPoseDriver>());
        Destroy(transform.GetChild(0).GetChild(1).GetComponent<ActionBasedController>());
        Destroy(transform.GetChild(0).GetChild(2).GetComponent<ActionBasedController>());
        Destroy(transform.GetChild(0).GetChild(3).GetComponent<UltimateTracker>());
        Destroy(transform.GetChild(0).GetChild(4).GetComponent<UltimateTracker>());
        Destroy(transform.GetChild(0).GetChild(5).GetComponent<UltimateTracker>());
    }


    [Command]
    public void CmdAddNewPlayerToList(PlayerInfor newPlayer)
    {
        playersManager = NetworkManager.singleton.GetComponent<PlayersManager>();
        PlayerData playerData = new()
        {
            playerName = newPlayer.playerName,
            isHost = newPlayer.isHost,
            deviceName = newPlayer.deviceName
        };
        playersManager.players.Add(playerData);

        UIUpdater.Instance.RpcUpdatePlayerList(playersManager.players);
    }


    [Command]
    public void CmdUpdateReadyState()
    {
        playersManager = NetworkManager.singleton.GetComponent<PlayersManager>();
        int id = connectionToClient.connectionId;
        if (NetworkServer.connections.TryGetValue(id, out var conn))
        {
            string pName = conn.identity.GetComponent<PlayerInfor>().playerName;
            int index = playersManager.players.FindIndex(p => p.playerName == pName);
            PlayerData data = playersManager.players[index];
            if (data.isReady)
            {
                data.isReady = false;
                playersManager.playerReady--;
            }
            else
            {
                data.isReady = true;
                playersManager.playerReady++;
                if (playersManager.playerReady > 1 && playersManager.playerReady == playersManager.players.Count)
                {
                    UIUpdater.Instance.RpcDisplayOtherPlayers();
                }
            }
            playersManager.players[index] = data;
            UIUpdater.Instance.RpcUpdateReadyState(index, data.isReady);
        }
    }


    [Command(requiresAuthority = false)]
    public void CmdTakeDamage(int dmg, int clip)
    {
        RpcTakeDamage(dmg, clip);
    }

    [ClientRpc]
    public void RpcTakeDamage(int dmg, int clip)
    {
        hp -= dmg;
        modelController.UpdateHp(hp, maxHp, isLocalPlayer);
        modelController.PlayCollisionSfx(clip);
    }
}
