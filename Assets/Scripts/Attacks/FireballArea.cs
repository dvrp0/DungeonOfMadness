using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FireballArea : MonoBehaviour
{
    [HideInInspector]
    public int Damage;

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
            collision.GetComponent<Enemy>().Hit(Damage);
            Destroy(gameObject);
        }
    }
}