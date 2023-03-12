using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BarrelExplosionArea : MonoBehaviour
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
        if (collision.CompareTag("Enemy"))
        {
            collision.GetComponent<Enemy>().Hit(1);
            Destroy(gameObject);
        }
        else if (collision.CompareTag("Player"))
        {
            GameManager.Instance.Player.Hit();
            Destroy(gameObject);
        }
    }
}