using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Tilemaps;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    public WaveFunctionCollapse WaveFunctionCollapse;
    public Environment[] Environments;
    public Environment RewardEnvironment;
    public Transform MainGrid;
    public Tilemap[] Outputs;
    public Tile SpecialTile;
    public Player Player;
    public EnemySpawner Spawner;
    public UIDrawer UIDrawer;
    public float TransitionDuration;
    public Color DecorativeTileColor;

    private Environment currentEnvironment;
    private Color currentColor;
    private int ammos;
    private List<GameObject> spawnNodes;
    private int stage;
    private int score;
    private bool isRewardStage;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        Player.gameObject.SetActive(false);
    }

    public void StartGame()
    {
        Player.gameObject.SetActive(true);
        Player.ResetState();
        isRewardStage = false;
        stage = 1;
        score = 0;
        ChangeEnvironment();
        Spawner.StartSpawn();
    }

    public void GameOver()
    {
        ammos = 0;
        Spawner.StopSpawn();
        UIDrawer.ShowGameOver();
    }

    private void ChangeEnvironment()
    {
        if (isRewardStage)
        {
            currentEnvironment = RewardEnvironment;
            Instantiate((currentEnvironment as RewardEnvironment).RewardChest, GetRandomSpawnNode().transform.position, Quaternion.identity);
        }
        else
            currentEnvironment = Environments[Random.Range(0, Environments.Length)];

        ammos = currentEnvironment.Ammos;

        if (currentEnvironment.SpecialObject != null)
            SpecialTile.gameObject = currentEnvironment.SpecialObject.gameObject;

        for (int i = 0; i < 4; i++)
            WaveFunctionCollapse.Generate(currentEnvironment.Base, currentEnvironment.Input, Outputs[i]);

        spawnNodes = GameObject.FindGameObjectsWithTag("SpawnNode").ToList();
        Player.transform.position = GetRandomSpawnNode().transform.position;

        foreach (var output in Outputs)
            foreach (var pos in output.cellBounds.allPositionsWithin)
                if (output.GetTileFlags(pos) == TileFlags.None)
                    output.SetColor(pos, DecorativeTileColor);
    }

    public Environment GetCurrentEnvironment() => currentEnvironment;

    public Color GetCurrentColor() => currentColor;

    public void ConsumeAmmo()
    {
        if (--ammos == 0)
        {
            Spawner.StopSpawn();
            StartCoroutine(AdvanceStage());
        }
    }

    public void GainReward()
    {
        Player.Heal();
        StartCoroutine(AdvanceStage());
    }

    private IEnumerator AdvanceStage()
    {
        //이전에 보상 던전이었으면 스테이지 증가 없이 넘어가고(무의미한 크레딧 획득량 증가 방지) 아니면 스테이지 증가
        isRewardStage = isRewardStage ? false : ++stage % 6 == 0;
        Debug.Log(stage);

        yield return new WaitForSeconds(TransitionDuration);

        Camera.main.DOShakePosition(0.1f, 0.1f, 0, 0, false).SetLoops(5, LoopType.Incremental).OnComplete(() => 
        { 
            Camera.main.transform.position = new Vector3(0, 0, -10);
            ChangeEnvironment();
        });

        yield return new WaitForSeconds(0.1f * 5 + TransitionDuration);

        if (!isRewardStage)
            Spawner.StartSpawn();
    }

    public GameObject GetRandomSpawnNode()
    {
        if (spawnNodes != null && spawnNodes.Count != 0)
            return spawnNodes[Random.Range(0, spawnNodes.Count)];

        return null;
    }

    public int GetCurrentStage() => stage;

    public bool CanAttack() => ammos > 0;

    public int GetCurrentScore() => score;

    public void AddScore(int value) => score += value;

    public int GetCurretAmmos() => ammos;
}