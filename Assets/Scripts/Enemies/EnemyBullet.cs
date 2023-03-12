using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyBullet : MonoBehaviour
{
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

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            Camera.main.DOShakePosition(0.065f, 0.055f, 0, 0, false).OnComplete(() => { Camera.main.transform.position = new Vector3(0, 0, -10); });
            Instantiate(OnHit, transform.position, Quaternion.Euler(transform.eulerAngles.z, 90, 90));
            GameManager.Instance.Player.Hit();
            Destroy(gameObject);
        }
        else if (collision.collider.CompareTag("Wall"))
        {
            Camera.main.DOShakePosition(0.065f, 0.055f, 0, 0, false).OnComplete(() => { Camera.main.transform.position = new Vector3(0, 0, -10); });
            Instantiate(OnHit, transform.position, Quaternion.Euler(transform.eulerAngles.z, 90, 90));
            Destroy(gameObject);
        }
        else
            Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
    }
}