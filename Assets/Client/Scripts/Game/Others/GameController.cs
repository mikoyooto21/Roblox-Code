using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour
{
    public static GameController Instance { private set; get; }

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        else if (Instance != this)
            Destroy(gameObject);
    }

    private void Start()
    {
        DOTween.SetTweensCapacity(500, 125);

        GameEvents.OnGameOverEvent.AddListener((bool isPlayerVictory) =>
        {
            if (isPlayerVictory)
                GameWon();
            else
                GameLost();
        });
    }

    public void RestartGame()
    {
        DOTween.KillAll();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    private void GameLost()
    {
        GameViewController.Instance.GameOverView.EnableView();
    }

    private void GameWon()
    {
        GameViewController.Instance.GameVictoryView.EnableView();
    }

    public void LoadMenuScene()
    {
        DOTween.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - 1);
    }
}
