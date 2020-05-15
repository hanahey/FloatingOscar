using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreWindow : MonoBehaviour
{
    private Text highscoreText;
    private Text scoreText;

    private void Awake()
    {
        scoreText = transform.Find("scoreText").GetComponent<Text>();
        highscoreText = transform.Find("highscoreText").GetComponent<Text>();
    }

    private void Start()
    {
        highscoreText.text = "HIGHSCORE: " + Score.GetHighScore().ToString();
        Hide();
        Oscar.GetInstance().OnStartPlaying += ScoreWindow_OnStartPlaying;
        Oscar.GetInstance().OnHit += ScoreWindow_OnHit;
    }

    private void ScoreWindow_OnStartPlaying(object sender, EventArgs e)
    {
        Show();
    }
    

    private void ScoreWindow_OnHit(object sender, EventArgs e)
    {
        Hide();
    }
    private void Update()
    {
        scoreText.text = (Level.GetInstance().GetBranchesPassed()).ToString();
    }

    private void Hide()
    {
        gameObject.SetActive(false);
    }

    private void Show()
    {
        gameObject.SetActive(true);
    }
}
