using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "DemonEnvironment", menuName = "Environments/Demon")]
public class DemonEnvironment : Environment
{
    public Transform Tombstone;
    public Transform Spell;
    public float Delay;

    public override IEnumerator Attack(Player player)
    {
        Instantiate(Spell, player.transform.position, player.GetAttackAngle());

        yield return new WaitForSeconds(Delay);
    }
}