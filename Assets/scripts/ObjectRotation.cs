using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class ObjectRotation : MonoBehaviour, IPointerDownHandler, IPointerUpHandler, IDragHandler
{
    float rotationSpeed = 1f;
    [SerializeField] Transform parent;
    Transform skinObj;
    bool isDragging = false;
    Vector3 lastMousePosition;

    private void Update()
    {
        if (skinObj == null)
        {
            skinObj = parent.transform.GetChild(0);
        }

        if (isDragging)
        {
            Vector3 currentMousePosition = Input.mousePosition;
            float deltaX = currentMousePosition.x - lastMousePosition.x;
            skinObj.Rotate(Vector3.down, deltaX * rotationSpeed);
            lastMousePosition = currentMousePosition;
        }
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isDragging = true;
        lastMousePosition = Input.mousePosition;
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isDragging = false;
    }

    public void OnDrag(PointerEventData eventData)
    {
        // Ми реалізуємо це у методі Update
    }
}
