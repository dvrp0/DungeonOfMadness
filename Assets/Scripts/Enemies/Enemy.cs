using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Enemy : MonoBehaviour, IEnemy
{
    public float Speed;
    public int Health;
    public Transform OnDeathParticle;
    [HideInInspector]
    public int Score;

    protected Rigidbody2D rigidbody;

    private void Start()
    {
        rigidbody = GetComponent<Rigidbody2D>();
        StartCoroutine(DoLifelongWork());
        StartCoroutine(Move());
        StartCoroutine(CheckHealth());
    }

    public virtual IEnumerator DoLifelongWork()
    {
        yield return null;
    }

    public virtual IEnumerator Move()
    {
        while (true)
        {
            var movement = GameManager.Instance.Player.transform.position - transform.position;
            movement.Normalize();

            rigidbody.AddForce(movement, ForceMode2D.Impulse);

            if (rigidbody.velocity.magnitude > Speed)
                rigidbody.velocity = rigidbody.velocity.normalized * Speed;

            yield return null;
        }
    }

    private IEnumerator CheckHealth()
    {
        while (true)
        {
            if (Health <= 0)
            {
                OnDeath();
                Instantiate(OnDeathParticle, transform.position, Quaternion.Euler(transform.eulerAngles.z, 90, 90));
                GameManager.Instance.AddScore(Score);

                if (GameManager.Instance.GetCurrentEnvironment() is DemonEnvironment demon)
                    Instantiate(demon.Tombstone, transform.position, Quaternion.identity);

                Destroy(gameObject);
            }

            yield return null;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.collider.CompareTag("Player"))
        {
            GameManager.Instance.Player.Hit();
            Instantiate(OnDeathParticle, transform.position, Quaternion.Euler(transform.eulerAngles.z, 90, 90));
            OnDeath();
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Lava"))
            Hit((GameManager.Instance.GetCurrentEnvironment() as FireEnvironment).LavaDamage);
    }

    public virtual void OnDeath()
    {

    }

    public void Hit(int damage)
    {
        Health -= damage;
    }
}