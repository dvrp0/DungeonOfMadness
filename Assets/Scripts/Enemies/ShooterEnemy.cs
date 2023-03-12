using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class ShooterEnemy : Enemy
{
    public Transform Bullet;
    public float ShootDelay;

    public override IEnumerator DoLifelongWork()
    {
        while (true)
        {
            yield return new WaitForSeconds(ShootDelay);

            var position = GameManager.Instance.Player.transform.position - transform.position;
            var angle = Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg;

            Instantiate(Bullet, transform.position, Quaternion.Euler(0, 0, angle - 90));
        }
    }
}