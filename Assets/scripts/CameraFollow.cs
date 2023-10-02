using UnityEngine;

public class CameraFollow : MonoBehaviour
{
    public Transform target;
    public float distance = 5.0f;
    public float minimumDistance = 2.0f; // Минимальное расстояние, чтобы камера не врезалась в игрока
    public LayerMask collisionLayer; // Добавьте слои, которые должны блокировать камеру (например, Walls)


    private void LateUpdate()
    {
        if (target)
        {
            Vector3 position = target.position - transform.forward * distance;
            Vector3 finalPosition = HandleCameraObstruction(position);
            transform.position = finalPosition;
        }
    }

    private Vector3 HandleCameraObstruction(Vector3 desiredPosition)
    {
        RaycastHit hit;
        if (Physics.Linecast(target.position, desiredPosition, out hit, collisionLayer))
        {
            float adjustedDistance = Vector3.Distance(target.position, hit.point) - 0.2f; // Отступ от точки столкновения
            adjustedDistance = Mathf.Max(adjustedDistance, minimumDistance); // Не позволяйте камере приближаться ближе к игроку, чем minimumDistance
            desiredPosition = target.position - transform.forward * adjustedDistance;
        }
        return desiredPosition;
    }
}