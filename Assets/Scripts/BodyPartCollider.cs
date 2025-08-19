using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BodyPartCollider : MonoBehaviour
{
    [Tooltip("[Collider Type = 0] if this is actually a model's body part, otherwise [Collider Type = 1]")]
    public int colliderType;
    public int hpLoss;
    public PlayerInfor player;

    public void SetPlayer(PlayerInfor playerInfor)
    {
        player = playerInfor;
    }

    public void TakeDamage()
    {
        player.CmdTakeDamage(hpLoss + Random.Range(-5, 6), colliderType);
    }
}
