using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "PoisonEnvironment", menuName = "Environments/Poison")]
public class PoisonEnvironment : Environment
{
    public Transform Dart;
    public int DartCount;
    public float DartSpread;
    public float Delay;

    public override IEnumerator Attack(Player player)
    {
        var angle = (DartCount - 1) * -DartSpread;

        for (int i = 0; i < DartCount; i++)
        {
            Instantiate(Dart, player.transform.position, Quaternion.Euler(0, 0, player.GetAttackAngle().eulerAngles.z + angle));
            angle += DartSpread;
        }

        yield return new WaitForSeconds(Delay);
    }
}