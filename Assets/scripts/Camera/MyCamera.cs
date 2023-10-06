using Cinemachine;
using UnityEngine;

public class MyCamera : MonoBehaviour
{
    public static MyCamera Instance { private set; get; }

    [SerializeField] private CinemachineVirtualCamera _playerFollowCamera;

    private Camera _camera;

    public Camera Camera => _camera;

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

        _camera = Camera.main;
    }

    public void SetPlayerCameraFollow(Transform follow)
    {
        // Set the target for the player follow camera
        _playerFollowCamera.Follow = follow;
    }
}
