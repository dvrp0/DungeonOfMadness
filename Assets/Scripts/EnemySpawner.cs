using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    public EnemyEntry[] EnemyEntries;
    public float SpawnDelay;
    public float CreditCoefficient = 0.75f;
    public Transform SpawnIndicator;
    public Transform OnDestroy;

    private float credit;
    private List<EnemyEntry> entries;
    private List<Transform> spawnedEnemies = new List<Transform>();
    private List<Transform> spawnIndicators = new List<Transform>();
    private Coroutine gainCreditCoroutine;
    private List<Coroutine> delayedSpawnCoroutines = new List<Coroutine>();

    private void Start()
    {
        StartCoroutine(RemoveDestroyedEnemies());
    }

    public void StartSpawn()
    {
        credit = 0;
        gainCreditCoroutine = StartCoroutine(GainCredit());
    }

    public void StopSpawn()
    {
        StopCoroutine(gainCreditCoroutine);
        foreach (var coroutine in delayedSpawnCoroutines)
            StopCoroutine(coroutine);

        foreach (var indicator in spawnIndicators)
        {
            Instantiate(OnDestroy, indicator.position, Quaternion.Euler(0, 90, 90));
            Destroy(indicator.gameObject);
        }

        foreach (var enemy in spawnedEnemies)
        {
            Instantiate(OnDestroy, enemy.position, Quaternion.Euler(0, 90, 90));
            Destroy(enemy.gameObject);
        }

        foreach (var obj in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            Instantiate(OnDestroy, obj.transform.position, Quaternion.Euler(0, 90, 90));
            Destroy(obj);
        }

        foreach (var obj in GameObject.FindGameObjectsWithTag("Tombstone"))
        {
            Instantiate(OnDestroy, obj.transform.position, Quaternion.Euler(0, 90, 90));
            Destroy(obj);
        }
    }

    private IEnumerator GainCredit()
    {
        while (true)
        {
            credit += CreditCoefficient * ((0.4f * GameManager.Instance.GetCurrentStage()) + 1) * 2 / 2;
            entries = EnemyEntries.Where(x => x.Cost <= credit).ToList();

            if (entries.Count > 0)
                delayedSpawnCoroutines.Add(StartCoroutine(SpawnWithDelay()));

            yield return new WaitForSeconds(1f);
        }
    }

    private IEnumerator SpawnWithDelay()
    {
        var target = GetWeightedRandomEnemy();
        var position = GameManager.Instance.GetRandomSpawnNode().transform.position;
        var indicator = Instantiate(SpawnIndicator, position, Quaternion.identity);
        spawnIndicators.Add(indicator);
        credit -= target.Cost;

        yield return new WaitForSeconds(SpawnDelay);

        var enemy = Instantiate(target.Enemy, new Vector3(position.x, position.y, 0), Quaternion.identity);
        enemy.GetComponent<Enemy>().Score = (int)target.Cost * 100;
        spawnedEnemies.Add(enemy);
        Destroy(indicator.gameObject);
    }

    private IEnumerator RemoveDestroyedEnemies()
    {
        while (true)
        {
            spawnedEnemies.RemoveAll(x => x == null);
            spawnIndicators.RemoveAll(x => x == null);

            yield return null;
        }
    }

    private EnemyEntry GetWeightedRandomEnemy()
    {
        float cumulative = 0, roll = Random.Range(0, entries.Select(x => x.Cost).Sum());

        foreach (var entry in entries)
        {
            cumulative += entry.Cost;

            if (cumulative > roll)
                return entry;
        }

        return entries[0];
    }

    public void AddToSpawnedPool(Transform target) => spawnedEnemies.Add(target);

    //https://forum.unity.com/threads/clean-est-way-to-find-nearest-object-of-many-c.44315/
    public Transform GetClosestEnemy(Vector3 center)
    {
        Transform bestTarget = null;
        float closestSqr = Mathf.Infinity;

        foreach (var enemy in spawnedEnemies)
        {
            if (enemy != null)
            {
                var direction = enemy.position - center;
                float dSqrToTarget = direction.sqrMagnitude;

                if (dSqrToTarget < closestSqr && center != enemy.position)
                {
                    closestSqr = dSqrToTarget;
                    bestTarget = enemy;
                }
            }
        }

        return bestTarget;
    }
}