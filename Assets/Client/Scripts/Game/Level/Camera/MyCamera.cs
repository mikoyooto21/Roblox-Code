using Cinemachine;
using UnityEngine;

public class MyCamera : MonoBehaviour
{
    public static MyCamera Instance { private set; get; }

    [SerializeField] private CinemachineVirtualCamera _playerFollowCamera;
    [SerializeField] private CinemachineVirtualCamera _gameOverCamera;

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
        GameEvents.OnGameOverEvent.AddListener((bool isPlayerVictory) =>
        {
            _playerFollowCamera.gameObject.SetActive(false);
            _gameOverCamera.gameObject.SetActive(true);
        });
    }

    public void SetCinemachineVirtualCameraFollow(Transform follow)
    {
        _playerFollowCamera.Follow = follow;
    }
}
