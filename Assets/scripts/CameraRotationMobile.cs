using UnityEngine.EventSystems;
using UnityEngine;

public class CameraRotationMobile : MonoBehaviour, IDragHandler
{
    public Transform cameraTransform;
    public Transform orientation;

    public float xSpeed = 220.0f;
    public float ySpeed = 100.0f;
    public float yMinLimit = -20f;
    public float yMaxLimit = 80f;

    private float x = 0.0f;
    private float y = 0.0f;

    private void Start()
    {
        Vector3 angles = cameraTransform.eulerAngles;
        x = angles.y;
        y = angles.x;
    }

    public void OnDrag(PointerEventData eventData)
    {
        x += eventData.delta.x * xSpeed * 0.02f;
        y -= eventData.delta.y * ySpeed * 0.02f;

        y = ClampAngle(y, yMinLimit, yMaxLimit);

        Quaternion cameraRotation = Quaternion.Euler(y, x, 0);
        cameraTransform.rotation = cameraRotation;
    }

    private float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}
