using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour {

    public Text PlayerScore;
    public Text WorkerScore;

    private static int _playerScore = 0;
    private static int _workerScore = 0;

	void Update ()
    {
        PlayerScore.text = "x " + _playerScore;
        WorkerScore.text = "x " + _workerScore;

        if (_playerScore >= 30)
        {
            ResetScores();
            GameManager.GM.SetState(GameManager.GameState.GameOver);
            GameManager.GM.UIManager.DisplayEndingScreen("VICTORY");
        }
        else if (_workerScore >= 10)
        {
            ResetScores();
            GameManager.GM.SetState(GameManager.GameState.GameOver);
            GameManager.GM.UIManager.DisplayEndingScreen("GAME OVER");
        }
    }

    void ResetScores()
    {
        _playerScore = 0;
        _workerScore = 0;
    }

    public void AddPlayerScore()
    {
        _playerScore++;

        if(_playerScore == 10)
            StartCoroutine(GameManager.GM.UIManager.TPC.SetText("The workers are speeding up a bit!"));
        else if(_playerScore == 20)
            StartCoroutine(GameManager.GM.UIManager.TPC.SetText("The workers are speeding up further!"));
    }

    public void AdjustWorkerScore(int value)
    {
        _workerScore += value;
    }

    public int GetPlayerScore()
    {
        return _playerScore;
    }
}
