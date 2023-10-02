using UnityEngine;
using UnityEngine.UI;

public class ButtonImageSwitcher : MonoBehaviour
{
    public Image outlineHolder; // Посилання на ImageOutlineHolder
    public Sprite defaultImage; // Зображення кнопки за замовчуванням
    public Sprite selectedImage; // Зображення, коли кнопка вибрана

    private bool isSelected = false;

    private void Start()
    {
        // Отримуємо ImageOutlineHolder
        outlineHolder = GetComponentInChildren<Image>();
    }

    public void ToggleButtonState()
    {
        isSelected = !isSelected;

        // Змінюємо зображення в кнопці
        Image buttonImage = GetComponent<Image>();
        buttonImage.sprite = isSelected ? selectedImage : defaultImage;

        // Змінюємо властивості компонента "Outline" на "ImageOutlineHolder"
        if (outlineHolder != null)
        {
            Outline outline = outlineHolder.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = isSelected; // Включаємо або вимикаємо обводку
                // Налаштуйте інші параметри "Outline", які вам потрібні
            }
        }
    }
}
