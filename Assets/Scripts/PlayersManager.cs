/* A NetworkManager class managing callbacks for events when a client connects to/disconnects from the server */

using Mirror;
using System.Collections.Generic;
using UnityEngine;


public struct PlayerData
{
    public string playerName;
    public string deviceName;
    public bool isHost;
    public bool isReady;
}


public class PlayersManager : NetworkManager
{
    public List<float> bodyPartsOffsets = new();
    public List<PlayerData> players = new();
    public int playerReady = 0;

    public override void OnServerAddPlayer(NetworkConnectionToClient conn)
    {
        base.OnServerAddPlayer(conn);

        PlayerInfor player = conn.identity.GetComponent<PlayerInfor>();
        player.order = NetworkServer.connections.Count;
        if (NetworkServer.connections.Count == 1)
        {
            player.playerName = "Player 1 (Host)";
            player.isHost = true;
        }
        else
        {
            player.playerName = $"Player {NetworkServer.connections.Count}";
            player.isHost = false;
        }
    }


    public override void OnServerDisconnect(NetworkConnectionToClient conn)
    {
        string pName = conn.identity.GetComponent<PlayerInfor>().playerName;
        int index = players.FindIndex(p => p.playerName == pName);
        if (players[index].isReady) playerReady--;
        players.RemoveAt(index);

        base.OnServerDisconnect(conn);

        UIUpdater.Instance.RpcUpdatePlayerList(players);
    }
}
