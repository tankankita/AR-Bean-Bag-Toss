using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class GameController : Singleton<GameController>
{

    public bool IsGameStarted { get; private set; }
    [SerializeField] private GameObject normalScene, gameUI ,winPanel, losePanel;
    [SerializeField] private Button throwBagBoardButton, resetBoardButton;

    // Start is called before the first frame update
    protected override void Awake()
    {
        base.Awake();
        ARController.OnARRunning.AddListener(HandleARRunning);
    }

    public void HandleARRunning(bool arRunning)
    {
        ScreenLog.Log("Ar running " + arRunning);
        normalScene.SetActive(!arRunning);
        if (arRunning)
        {
            PlaceBoard();
        }
         else
        {
            gameUI.SetActive(true);
        }
    }

    public void PlaceBoard()
    {
        ScreenLog.Log("PlaceBoard");
        StartCoroutine(DoPlaceBoard());
    }

    private IEnumerator DoPlaceBoard()
    {
        ScreenLog.Log("Destroying old");

        BoardController.Instance.DestroyBoard();
        gameUI.SetActive(false);
        BoardController.Instance.StartPlacingBoard();
        while(BoardController.Instance.isPlacing)
        {
            yield return null;
        }
        gameUI.SetActive(true);
    }

    public void LoseGame()
    {
        ScreenLog.Log("LOST");
        throwBagBoardButton.interactable = false;
        losePanel.SetActive(true);
    }

    public void WinGame()
    {
        ScreenLog.Log("WIN");
        winPanel.SetActive(true);
    }

    public void GameStarted()
    {
        IsGameStarted = true;
        resetBoardButton.interactable = false;
    }

    public void ResetGame()
    {
        ScreenLog.Log("resetting the game now");
        ScoreController.Instance.ResetScore();
        ScreenLog.Log("resetting the bags now");

        BagsController.Instance.ResetBags();
        resetBoardButton.interactable = true;
        throwBagBoardButton.interactable = true;
        IsGameStarted = false;
        losePanel.SetActive(false);
        winPanel.SetActive(false);



    }
}
