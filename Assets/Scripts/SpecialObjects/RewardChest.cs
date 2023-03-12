using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RewardChest : MonoBehaviour
{
    public Transform RewardGainParticle;
    public Transform OnDeathParticle;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Attack"))
        {
            Camera.main.DOShakePosition(0.065f, 0.055f, 0, 0, false).OnComplete(() => { Camera.main.transform.position = new Vector3(0, 0, -10); });

            Instantiate(RewardGainParticle, transform.position, Quaternion.Euler(transform.eulerAngles.z, 90, 90));
            Instantiate(OnDeathParticle, transform.position, Quaternion.Euler(transform.eulerAngles.z, 90, 90));

            GameManager.Instance.GainReward();
            Destroy(gameObject);
        }
    }
}