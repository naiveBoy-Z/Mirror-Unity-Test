using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DisplayMenu : MonoBehaviour
{
    [Tooltip("Distance from the player's camera to Menu UI along the Z-axis")]
    public float distance;

    Transform playerHead;


    private void Start()
    {
        playerHead = GameObject.Find("Head Target Reference").transform;
    }


    void Update()
    {
        transform.position = new Vector3(playerHead.position.x, transform.position.y, playerHead.position.z + distance);
    }
}
