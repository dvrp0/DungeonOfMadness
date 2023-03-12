using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "ForestEnvironment", menuName = "Environments/Forest")]
public class ForestEnvironment : Environment
{
    public Transform Dart;
    public float DartSpread;
    public float Delay;

    public override IEnumerator Attack(Player player)
    {
        var z = player.GetAttackAngle().eulerAngles.z + Random.Range(-DartSpread, DartSpread);
        Instantiate(Dart, player.transform.position, Quaternion.Euler(0, 0, z));

        yield return new WaitForSeconds(Delay);
    }
}