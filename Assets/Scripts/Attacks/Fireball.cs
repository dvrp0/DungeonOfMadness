using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fireball : MonoBehaviour
{
    public int Damage;
    public Transform ExplosionParticle;
    public Transform ExplosionArea;
    public float Speed;

    private void Start()
    {
        StartCoroutine(Move());
    }

    private IEnumerator Move()
    {
        while (true)
        {
            transform.position += transform.up * Time.deltaTime * Speed;

            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy") || collision.CompareTag("Wall"))
        {
            Camera.main.DOShakePosition(0.065f, 0.055f, 0, 0, false).OnComplete(() => { Camera.main.transform.position = new Vector3(0, 0, -10); });
            Instantiate(ExplosionParticle, transform.position, Quaternion.Euler(transform.eulerAngles.z, 90, 90));
            var area = Instantiate(ExplosionArea, transform.position, Quaternion.identity);
            area.GetComponent<FireballArea>().Damage = Damage;
            Destroy(gameObject);
        }
    }
}