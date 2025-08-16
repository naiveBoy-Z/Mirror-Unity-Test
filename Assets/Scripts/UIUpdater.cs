using UnityEngine;
using Mirror;
using System.Collections.Generic;

public class UIUpdater : NetworkBehaviour
{
    public static UIUpdater Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(Instance);
            return;
        }
        Instance = this;
    }


    [ClientRpc]
    public void RpcUpdatePlayerList(List<PlayerData> players)
    {
        MenuManager.Instance.UpdatePlayerList(players);
    }


    [ClientRpc]
    public void RpcUpdateReadyState(int index, bool on)
    {
        MenuManager.Instance.UpdateReadyState(index, on);
    }


    [ClientRpc]
    public void RpcDisplayOtherPlayers()
    {
        MenuManager.Instance.DisplayOtherPlayers();
    }


    public void DisplayFinalResult(bool win)
    {
        MenuManager.Instance.DisplayFinalResult(win);
    }
}
