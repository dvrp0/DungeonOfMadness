using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class RevengeEnemy : Enemy
{
    public Transform SmallEnemy;

    public override void OnDeath()
    {
        GameManager.Instance.Spawner.AddToSpawnedPool(Instantiate(SmallEnemy, transform.position, Quaternion.identity));
    }
}