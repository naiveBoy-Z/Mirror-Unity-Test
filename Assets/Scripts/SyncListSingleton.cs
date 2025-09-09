using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class SyncListSingleton : NetworkBehaviour
{
    #region Implement a singleton
    public static SyncListSingleton instance;
    private void Awake()
    {
        if (instance != null && instance != this)
        {
            Destroy(instance);
        }
        instance = this;
    }
    #endregion

    [SyncVar] public int totalPlayer;
    public readonly SyncList<float> bodyPartsOffsetsList = new();

    public void AddBodyPartsOffsetsList(List<float> bodyPartsOffsets)
    {
        for (int i = 0; i < bodyPartsOffsets.Count; i++)
        {
            bodyPartsOffsetsList.Add(bodyPartsOffsets[i]);
        }
    }
}
