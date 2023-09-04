using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelController : MonoBehaviour
{
    public static LevelController Instance { private set; get; }

    private bool _isGameOver;
    private bool _isGameStart;
    private float _remainingGameDuration;

    private LevelBuilder _levelBuilder;

    public float RemainingGameDuration => _remainingGameDuration;
    public LevelBuilder LevelBuilder => _levelBuilder;

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

        _levelBuilder = GetComponent<LevelBuilder>();
    }

    private void Start()
    {
        _levelBuilder.BuildLevel();

        _remainingGameDuration = _levelBuilder.TotalDestructionTime;

        GameViewController.Instance.GameView.SetPlayersCounrTxt(_levelBuilder.PlayersList.Count.ToString());

        GameEvents.OnGameStartEvent.AddListener(() =>
        {
            _isGameStart = true;
            int totalPlayers = _levelBuilder.PlayersList.Count;

            for (int i = 0; i < totalPlayers; i += 10)
            {
                StartCoroutine(UpdatePlayersCoroutine(i, Mathf.Min(i + 10, totalPlayers)));
            }
        });

        GameEvents.OnPlayerLoseEvent.AddListener((PlayerController player) =>
        {
            PlayerLose(player);
        });
    }

    private void Update()
    {
        if (_remainingGameDuration > 0 && _isGameStart)
            _remainingGameDuration -= Time.deltaTime;
        else if (_remainingGameDuration < 0)
            _remainingGameDuration = 0;
    }

    public void PlayerLose(PlayerController player)
    {
        for (int i = 0; i < _levelBuilder.PlayersList.Count; i++)
        {
            if (_levelBuilder.PlayersList[i] == player)
            {
                _levelBuilder.PlayersList.RemoveAt(i);
                break;
            }
        }

        GameViewController.Instance.GameView.SetPlayersCounrTxt(_levelBuilder.PlayersList.Count.ToString());

        if (_isGameOver)
            return;

        if (_levelBuilder.PlayersList.Count <= 0)
        {
            _isGameOver = true;
            _levelBuilder.PlayersList.Clear();

            GameEvents.InvokeGameOverEvent(true);
        }
        else if (_levelBuilder.PlayersList.Count == 1 && _levelBuilder.PlayersList[0] == _levelBuilder.Player)
        {
            _isGameOver = true;
            _levelBuilder.PlayersList.Clear();

            GameEvents.InvokeGameOverEvent(true);
        }
        else if (player == _levelBuilder.Player)
        {
            _isGameOver = true;
            GameEvents.InvokeGameOverEvent(false);
        }
    }

    private IEnumerator UpdatePlayersCoroutine(int startIndex, int endIndex)
    {
        List<PlayerController> playersList = _levelBuilder.PlayersList;

        while (true)
        {
            for (int i = startIndex; i < endIndex; i++)
            {
                if (i < playersList.Count)
                    playersList[i].MyUpdate();
            }

            yield return null;
        }
    }
}
