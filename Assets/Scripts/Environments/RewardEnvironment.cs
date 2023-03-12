using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RewardEnvironment", menuName = "Environments/Reward")]
public class RewardEnvironment : Environment
{
    public Transform RewardChest;
    public Transform Bullet;
    public float Delay;

    public override IEnumerator Attack(Player player)
    {
        Instantiate(Bullet, player.transform.position, player.GetAttackAngle());

        yield return new WaitForSeconds(Delay);
    }
}