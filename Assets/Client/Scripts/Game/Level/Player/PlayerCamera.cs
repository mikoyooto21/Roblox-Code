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

    private PlayerInputs _input;
    private Transform _cameraTransform;

    private float _verticalRotation = 0;
    private float _horizontalRotation = 0;

    private void Start()
    {
        _input = GetComponent<PlayerInputs>();
        _cameraTransform = Camera.main.transform;

        _verticalRotation = _cameraTransform.rotation.x;
        _horizontalRotation = _cameraTransform.rotation.y;

        MyCamera.Instance.SetCinemachineVirtualCameraFollow(_cinemachineCameraTarget);
    }

    private void LateUpdate()
    {
        CameraRotation();
    }

    private void CameraRotation()
    {
        if (_cinemachineCameraTarget == null)
            return;

        _horizontalRotation += _input.Look.x * _lookHorizontalSensitivity;
        _verticalRotation += _input.Look.y * _lookVerticalSensitivity;

        _verticalRotation = Mathf.Clamp(_verticalRotation, BottomClamp, TopClamp);
        Quaternion targetRotation = Quaternion.Euler(_verticalRotation, _horizontalRotation, 0);
        _cinemachineCameraTarget.rotation = Quaternion.Slerp(_cameraTransform.rotation, targetRotation, _smoothFactor);
    }
}
