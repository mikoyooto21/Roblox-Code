using UnityEngine;
using UnityEngine.UI;

public class ButtonImageSwitcher : MonoBehaviour
{
    public Image outlineHolder; // ��������� �� ImageOutlineHolder
    public Sprite defaultImage; // ���������� ������ �� �������������
    public Sprite selectedImage; // ����������, ���� ������ �������

    private bool isSelected = false;

    private void Start()
    {
        // �������� ImageOutlineHolder
        outlineHolder = GetComponentInChildren<Image>();
    }

    public void ToggleButtonState()
    {
        isSelected = !isSelected;

        // ������� ���������� � ������
        Image buttonImage = GetComponent<Image>();
        buttonImage.sprite = isSelected ? selectedImage : defaultImage;

        // ������� ���������� ���������� "Outline" �� "ImageOutlineHolder"
        if (outlineHolder != null)
        {
            Outline outline = outlineHolder.GetComponent<Outline>();
            if (outline != null)
            {
                outline.enabled = isSelected; // �������� ��� �������� �������
                // ���������� ���� ��������� "Outline", �� ��� ������
            }
        }
    }
}
