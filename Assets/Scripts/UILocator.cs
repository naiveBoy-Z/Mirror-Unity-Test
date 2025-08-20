using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UILocator : MonoBehaviour
{
    public Transform player;

    void Update()
    {
        transform.position = new Vector3(player.position.x, player.position.y + 0.3f, player.position.z);
    }
}
