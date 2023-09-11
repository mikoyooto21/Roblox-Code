using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LevelUnlock : MonoBehaviour
{
    public GameObject unlockedLevelButton; // ��������� �� ������ ���� � ��������
    public GameObject lockedLevelButton; // ��������� �� ������ ���� ��� �������

    private void Start()
    {
        // ��������, �� ����� ������������� � ��������� �������� ������
        if (PlayerPrefs.HasKey("UnlockedLevelWithTimer") && PlayerPrefs.GetInt("UnlockedLevelWithTimer") == 1)
        {
            unlockedLevelButton.SetActive(true); // г���� ������������
            lockedLevelButton.SetActive(false); // г���� ��� ������� ����������
        }
        else
        {
            unlockedLevelButton.SetActive(false); // г���� ������������
            lockedLevelButton.SetActive(true); // г���� ��� ������� ��������
        }
    }
}
