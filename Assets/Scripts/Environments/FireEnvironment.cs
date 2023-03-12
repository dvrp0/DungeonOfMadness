using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "FireEnvironment", menuName = "Environments/Fire")]
public class FireEnvironment : Environment
{
    public Transform Fireball;
    public int LavaDamage;
    public float Delay;

    public override IEnumerator Attack(Player player)
    {
        Instantiate(Fireball, player.transform.position, player.GetAttackAngle());

        yield return new WaitForSeconds(Delay);
    }
}