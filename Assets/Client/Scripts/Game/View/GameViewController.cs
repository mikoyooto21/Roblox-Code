using UnityEngine;

public class GameViewController : MonoBehaviour
{
    public static GameViewController Instance { private set; get; }

    [SerializeField] private GameView _gameView;
    [SerializeField] private PlayerNicknames _playerNicknames;
    [SerializeField] private GameOverView _gameOverView;
    [SerializeField] private GameVictoryView _gameVictoryView;

    public GameView GameView => _gameView;
    public PlayerNicknames PlayerNicknames => _playerNicknames;
    public GameOverView GameOverView => _gameOverView;
    public GameVictoryView GameVictoryView => _gameVictoryView;


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

    private void Update()
    {
        _gameView.MyUpdate();
        _playerNicknames.MyUpdate();
    }
}
