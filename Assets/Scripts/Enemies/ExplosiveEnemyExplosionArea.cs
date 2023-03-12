using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ExplosiveEnemyExplosionArea : MonoBehaviour
{
    private void Start()
    {
        StartCoroutine(RemoveArea());
    }

    private IEnumerator RemoveArea()
    {
        yield return new WaitForSeconds(0.1f);
        Destroy(gameObject);
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            GameManager.Instance.Player.Hit();
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Enemy"))
        {
            Camera.main.DOShakePosition(0.065f, 0.055f, 0, 0, false).OnComplete(() => { Camera.main.transform.position = new Vector3(0, 0, -10); });
            collision.GetComponent<Enemy>().Hit(1);
            Destroy(gameObject);
        }
    }
}