using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Barrel : MonoBehaviour
{
    public Transform ExplosionParticle;
    public Transform ExplosionArea;

    public void Explode()
    {
        Instantiate(ExplosionParticle, transform.position, Quaternion.Euler(transform.eulerAngles.z, 90, 90));
        Instantiate(ExplosionArea, transform.position, Quaternion.identity);
        Destroy(gameObject);
    }
}