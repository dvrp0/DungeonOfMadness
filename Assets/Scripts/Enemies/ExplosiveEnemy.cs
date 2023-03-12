using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveEnemy : Enemy
{
    public Transform ExplosionParticle;
    public Transform ExplosionArea;

    public override void OnDeath()
    {
        Camera.main.DOShakePosition(0.065f, 0.055f, 0, 0, false).OnComplete(() => { Camera.main.transform.position = new Vector3(0, 0, -10); });
        Instantiate(ExplosionParticle, transform.position, Quaternion.Euler(transform.eulerAngles.z, 90, 90));
        var area = Instantiate(ExplosionArea, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}