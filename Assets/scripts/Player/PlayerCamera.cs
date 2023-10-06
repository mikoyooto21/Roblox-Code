using UnityEngine;

public class PlayerCamera : MonoBehaviour
{
    [SerializeField] private float _smoothFactor = 1f;
    [SerializeField] private float _lookVerticalSensitivity = 0.5f;
    [SerializeField] private float _lookHorizontalSensitivity = 1.5f;

    [Space(10)]
    [SerializeField] private float TopClamp = 70f;
    [SerializeField] private float BottomClamp = -5f;

    [Space(10)]
    [SerializeField] private Transform _cinemachineCameraTarget;

    private PlayerInputs _playerInputs;
    private Transform _cameraTransform;

    private float _verticalRotation = 0;
    private float _horizontalRotation = 0;

    private void Start()
    {
        _playerInputs = GetComponent<PlayerInputs>();

        _cameraTransform = MyCamera.Instance.Camera.transform;

        // set the starting rotation of the camera 
        _verticalRotation = _cameraTransform.eulerAngles.x;
        _horizontalRotation = transform.eulerAngles.y;

        // set the target for the camera 
        MyCamera.Instance.SetPlayerCameraFollow(_cinemachineCameraTarget);
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        // camera rotation 
        if (_cinemachineCameraTarget == null)
            return;

        _horizontalRotation += _playerInputs.Look.x * _lookHorizontalSensitivity;
        _verticalRotation += _playerInputs.Look.y * _lookVerticalSensitivity;

        _verticalRotation = Mathf.Clamp(_verticalRotation, BottomClamp, TopClamp);
        Quaternion targetRotation = Quaternion.Euler(_verticalRotation, _horizontalRotation, 0);
        _cinemachineCameraTarget.rotation = Quaternion.Slerp(_cameraTransform.rotation, targetRotation, _smoothFactor);
    }
}

