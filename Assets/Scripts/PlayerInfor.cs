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
    public TextMeshProUGUI test;
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
            modelController.player = this;
            modelController.localPlayerCamera = transform.GetChild(0).GetChild(0);
            CmdSetPlayerDeviceName(SystemInfo.deviceModel);
            CmdAddBodyPartsOffsetsList(NetworkManager.singleton.GetComponent<PlayersManager>().bodyPartsOffsets);
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
        while (order == 0)
        {
            yield return null;
        }
        while (SyncListSingleton.instance.bodyPartsOffsetsList.Count != SyncListSingleton.instance.totalPlayer * 6)
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
        SyncListSingleton.instance.bodyPartsOffsetsList.RemoveAt(order - 1);
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
    void CmdAddBodyPartsOffsetsList(List<float> bodyPartsOffsets)
    {
        SyncListSingleton.instance.AddBodyPartsOffsetsList(bodyPartsOffsets);
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
                    MenuManager.Instance.localPlayerUI.SetActive(true);
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


    [Command]
    public void CmdPlayGunSfx()
    {
        RpcPlayGunSfx();
    }

    [ClientRpc]
    public void RpcPlayGunSfx()
    {
        if (!isLocalPlayer)
        {
            modelController.audioSource.PlayOneShot(modelController.audioSource.clip);
        }
    }
}
