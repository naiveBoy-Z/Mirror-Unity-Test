using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SyncList : NetworkBehaviour
{
    #region Implement a singleton
    public static SyncList instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
        }
        instance = this;
    }
    #endregion

    public readonly SyncList<List<float>> bodyPartsOffsetsList = new();

    [Command]
    public void CmdAddBodyPartsOffsetsList(List<float> bodyPartsOffsets)
    {
        bodyPartsOffsetsList.Add(bodyPartsOffsets);
    }
}
