using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public int MaxLife;
    public float Speed;
    public float InvulnerableDuration;

    private int life;
    private Rigidbody2D rigidbody;
    private Quaternion attackAngle;
    private bool isInvulnerable;

    private void Start()
    {
        isInvulnerable = false;
        life = MaxLife;
        rigidbody = GetComponent<Rigidbody2D>();

        StartCoroutine(LookAtMouse());
        StartCoroutine(GetInput());
        StartCoroutine(Attack());
    }

    public void ResetState()
    {
        isInvulnerable = false;
        life = MaxLife;
    }

    private void FixedUpdate()
    {
        var movement = new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));

        if (GameManager.Instance.CanAttack())
            rigidbody.AddForce(movement, ForceMode2D.Impulse);

        if (rigidbody.velocity.magnitude > Speed)
            rigidbody.velocity = rigidbody.velocity.normalized * Speed;

        transform.position = new Vector3(transform.position.x, transform.position.y, -0.5f);
    }

    private IEnumerator LookAtMouse()
    {
        while (true)
        {
            var position = Camera.main.ScreenToWorldPoint(Input.mousePosition) - transform.position;
            var angle = Mathf.Atan2(position.y, position.x) * Mathf.Rad2Deg;
            attackAngle = Quaternion.Euler(0, 0, angle - 90);

            yield return null;
        }
    }

    private IEnumerator GetInput()
    {
        while (true)
        {
            if (GameManager.Instance.GetCurrentEnvironment() is not IceEnvironment)
            {
                if (Input.GetButtonUp("Horizontal"))
                    rigidbody.velocity = new Vector2(rigidbody.velocity.normalized.x * 0.5f, rigidbody.velocity.y);

                if (Input.GetButtonUp("Vertical"))
                    rigidbody.velocity = new Vector2(rigidbody.velocity.x, rigidbody.velocity.normalized.y * 0.5f);
            }

            yield return null;
        }
    }

    private IEnumerator Attack()
    {
        while (true)
        {
            if (Input.GetMouseButton(0) && GameManager.Instance.CanAttack())
            {
                GameManager.Instance.ConsumeAmmo();
                yield return StartCoroutine(GameManager.Instance.GetCurrentEnvironment().Attack(this));
            }

            yield return null;
        }
    }

    public Quaternion GetAttackAngle() => attackAngle;

    public void Hit()
    {
        if (!isInvulnerable)
        {
            GameManager.Instance.UIDrawer.DecreaseHeart(--life);
            StartCoroutine(GoInvulnerable());

            if (life == 0)
                GameManager.Instance.GameOver();
        }
    }

    public void Heal()
    {
        if (life < 3)
            GameManager.Instance.UIDrawer.IncreaseHeart(++life);
    }

    private IEnumerator GoInvulnerable()
    {
        isInvulnerable = true;
        yield return new WaitForSeconds(InvulnerableDuration);
        isInvulnerable = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Lava"))
            Hit();
    }
}