using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "IceEnvironment", menuName = "Environments/Ice")]
public class IceEnvironment : Environment
{
    public Transform Icicle;
    public float Delay;

    public override IEnumerator Attack(Player player)
    {
        Instantiate(Icicle, player.transform.position, player.GetAttackAngle());

        yield return new WaitForSeconds(Delay);
    }
}