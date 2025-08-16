using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartCollider : MonoBehaviour
{
    public int hpLoss;
    public PlayerInfor player;
    public AudioSource audioSource;

    public void SetPlayer(PlayerInfor playerInfor)
    {
        player = playerInfor;
    }

    public void TakeDamage()
    {
        audioSource.PlayOneShot(audioSource.clip);

        player.CmdTakeDamage(hpLoss + Random.Range(-5, 6));
    }
}
