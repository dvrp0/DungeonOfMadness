using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Missile : MonoBehaviour
{
    public int Damage;
    public Transform OnHit;
    public int MinAngle;
    public float Speed;

    private void Start()
    {
        StartCoroutine(Move());
        StartCoroutine(FollowTarget(GameManager.Instance.Spawner.GetClosestEnemy(GameManager.Instance.Player.transform.position)));
    }

    IEnumerator Move()
    {
        while (true)
        {
            transform.position += transform.up * Time.deltaTime * Speed;
            Speed += 0.1f;

            yield return null;
        }
    }

    //https://zprooo915.tistory.com/23
    IEnumerator FollowTarget(Transform target)
    {
        while (target != null)
        {
            var targetPosition = (target.position - transform.position).normalized;
            float dot = Vector3.Dot(transform.up, targetPosition);

            if (dot < 1)
            {
                float angle = Mathf.Acos(dot) * Mathf.Rad2Deg;

                if (angle >= MinAngle)
                {
                    var cross = Vector3.Cross(transform.up, targetPosition);
                    angle = transform.rotation.eulerAngles.z + (cross.z < 0 ? -Mathf.Min(10, angle) : Mathf.Min(10, angle));
                    transform.eulerAngles = new Vector3(0, 0, angle);
                }
            }

            yield return null;
        }
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Enemy"))
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
}