using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "RockEnvironment", menuName = "Environments/Rock")]
public class RockEnvironment : Environment
{
    public Transform Cannon;
    public float Delay;

    public override IEnumerator Attack(Player player)
    {
        Instantiate(Cannon, player.transform.position, player.GetAttackAngle());

        yield return new WaitForSeconds(Delay);
    }
}