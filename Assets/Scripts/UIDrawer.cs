using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIDrawer : MonoBehaviour
{
    public Text Score;
    public Image[] Hearts;
    public Text Ammos;
    public Sprite FilledHeart;
    public Sprite EmptyHeart;
    public Text DeathQuote;
    public Transform Intro;
    public Transform Ingame;
    public Transform GameOver;
    public string[] DeathQuotes;

    private void Start()
    {
        Ingame.gameObject.SetActive(false);
        GameOver.gameObject.SetActive(false);

        StartCoroutine(UpdateTexts());
    }

    public void StartGame()
    {
        Intro.gameObject.SetActive(false);
        Ingame.gameObject.SetActive(true);
        GameManager.Instance.StartGame();
    }

    public void ShowGameOver()
    {
        Camera.main.DOShakePosition(0.1f, 0.1f, 0, 0, false).SetLoops(5, LoopType.Incremental).OnComplete(() =>
        {
            Camera.main.transform.position = new Vector3(0, 0, -10);
            DeathQuote.text = DeathQuotes[Random.Range(0, DeathQuotes.Length)];
            GameOver.gameObject.SetActive(true);
        });
    }

    public void RetryGame()
    {
        GameOver.gameObject.SetActive(false);
        Ingame.gameObject.SetActive(true);
        ResetHearts();
        GameManager.Instance.StartGame();
    }

    public void QuitGame()
    {
        Application.Quit();
    }

    private IEnumerator UpdateTexts()
    {
        while (true)
        {
            Score.text = GameManager.Instance.GetCurrentScore().ToString();
            Ammos.text = GameManager.Instance.GetCurretAmmos().ToString();

            yield return null;
        }
    }

    public void ResetHearts()
    {
        foreach (var heart in Hearts)
            heart.sprite = FilledHeart;
    }

    public void DecreaseHeart(int life)
    {
        Hearts[life].sprite = EmptyHeart;
    }

    public void IncreaseHeart(int life)
    {
        for (int i = 0; i < life; i++)
            Hearts[i].sprite = FilledHeart;
    }
}