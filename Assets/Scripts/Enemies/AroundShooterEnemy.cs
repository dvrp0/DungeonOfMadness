using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class AroundShooterEnemy : Enemy
{
    public Transform Bullet;
    public int BulletCount;
    public float ShootDelay;

    public override IEnumerator DoLifelongWork()
    {
        while (true)
        {
            yield return new WaitForSeconds(ShootDelay);

            for (int i = 0; i < BulletCount; i++)
                Instantiate(Bullet, transform.position, Quaternion.Euler(0, 0, 360 / BulletCount * i));
        }
    }
}