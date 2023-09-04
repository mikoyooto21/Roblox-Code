using TMPro;
using UnityEngine;

public class GameView : MonoBehaviour
{
    [SerializeField] private TMP_Text _playersCountTxt;
    [SerializeField] private TMP_Text _coinsCountTxt;
    [SerializeField] private TMP_Text _timerTxt;

    private LevelController _levelController;

    private void Start()
    {
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;

        GameEvents.OnGameStartEvent.AddListener(() =>
        {
            _levelController = LevelController.Instance;
        });

        GameEvents.OnPlayerCollectedCoinEvent.AddListener(() =>
        {
            _coinsCountTxt.text = GameDataManager.Instance.PlayerCountCoins.ToString();
        });
    }

    public void MyUpdate()
    {
        if (_levelController != null)
        {
            int minutes = Mathf.FloorToInt(_levelController.RemainingGameDuration / 60);
            int seconds = Mathf.FloorToInt(_levelController.RemainingGameDuration % 60);

            _timerTxt.text = string.Format("{0:D2}:{1:D2}", minutes, seconds);
        }
    }

    public void SetPlayersCounrTxt(string count)
    {
        _playersCountTxt.text = count;
    }
}
