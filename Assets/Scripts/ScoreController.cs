using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class ScoreController : Singleton<ScoreController>
{

    [SerializeField] private TextMeshProUGUI scoreText;
    [SerializeField] private int score;
    private bool _isAssessing;

    private void Start()
    {
        scoreText.text = score.ToString();
    }
    public void UpdateScore(int val)
    {

        if(score == 0)
        {
            GameController.Instance.ResetGame();
        }

        score += val;
        if(!_isAssessing)
        {
            StartCoroutine(AssessScore());

        }
    }

    public void ResetScore()
    {
        score = 0;
        scoreText.text = score.ToString();
       
    }

    public IEnumerator AssessScore()
    {
        _isAssessing = true;
        yield return new WaitForSeconds(1f);
        scoreText.text = score.ToString();

        if (score > 21)
        {
            GameController.Instance.LoseGame();
        } else if(score == 21)
        {
            GameController.Instance.WinGame();
        }

        _isAssessing = false;
    }
}
