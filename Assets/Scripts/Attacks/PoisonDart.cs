using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PoisonDart : MonoBehaviour
{
    public int Damage;
    public Transform OnHit;
    public float Speed;

    private void Start()
    {
        StartCoroutine(Move());
    }

    IEnumerator Move()
    {
        while (true)
        {
            transform.position += transform.up * Time.deltaTime * Speed;

            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Barrel"))
        {
            Camera.main.DOShakePosition(0.065f, 0.055f, 0, 0, false).OnComplete(() => { Camera.main.transform.position = new Vector3(0, 0, -10); });
            Instantiate(OnHit, transform.position, Quaternion.Euler(transform.eulerAngles.z, 90, 90));
            collision.GetComponent<Barrel>().Explode();
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Enemy"))
        {
            Camera.main.DOShakePosition(0.065f, 0.055f, 0, 0, false).OnComplete(() => { Camera.main.transform.position = new Vector3(0, 0, -10); });
            Instantiate(OnHit, transform.position, Quaternion.Euler(transform.eulerAngles.z, 90, 90));
            collision.GetComponent<Enemy>().Hit(Damage);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Wall"))
        {
            Camera.main.DOShakePosition(0.065f, 0.055f, 0, 0, false).OnComplete(() => { Camera.main.transform.position = new Vector3(0, 0, -10); });
            Instantiate(OnHit, transform.position, Quaternion.Euler(transform.eulerAngles.z, 90, 90));
            Destroy(gameObject);
        }
    }

    private void OnBecameInvisible()
    {
        Destroy(gameObject);
    }
}