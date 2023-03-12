using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Spell : MonoBehaviour
{
    public Transform OnHit;

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Enemy"))
        {
            Instantiate(OnHit, transform.position, Quaternion.Euler(transform.eulerAngles.z, 90, 90));
            Destroy(gameObject);
        }
        else
            Physics2D.IgnoreCollision(collision.collider, collision.otherCollider);
    }
}