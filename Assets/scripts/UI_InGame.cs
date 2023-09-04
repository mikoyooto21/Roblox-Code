using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEditor;
using System;
using UnityEngine.UI;

public class UI_InGame : MonoBehaviour
{
    public float simpleTimer = 0f;
    public float countdownTimeValue = 90;
    public static bool gamePaused = false;


    [Header("References")]
    public GameObject inGameUI;
    public GameObject onPauseMenu;

    public TMP_Text countdownTimerText;
    public TMP_Text timerText;
    public Image checkpointsTrackerBar;
    public Button menuButton;
    public Button resumeButton;

    int lastLvl = 1;

    // Update is called once per frame
    void Update()
    {
        if (countdownTimeValue > 0)
            countdownTimeValue -= Time.deltaTime;
        else
            countdownTimeValue = 0f;

        simpleTimer += Time.deltaTime;

        DisplayCountdownTime(countdownTimeValue);
        DisplayTime(simpleTimer);
        CheckpointsTracker();

        if (Input.GetKeyDown("tab") && !gamePaused)
        {
            Debug.Log("PAUSED");
            GamePause();
        }
        else if (Input.GetKeyDown("tab") && gamePaused)
        {
            Debug.Log("Ressumed");
            GameResume();
        }
    }

    void DisplayCountdownTime(float timeToDisplay)
    {
        if (timeToDisplay < 0)
            timeToDisplay = 0;

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        countdownTimerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void DisplayTime(float timeToDisplay)
    {

        float minutes = Mathf.FloorToInt(timeToDisplay / 60);
        float seconds = Mathf.FloorToInt(timeToDisplay % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
    }

    void CheckpointsTracker()
    {
        if (lastLvl < PlayerMovement.passedLvls)
        {
            RectTransform rectTransform = checkpointsTrackerBar.rectTransform;
            Vector2 newSize = rectTransform.sizeDelta;
            newSize.x += 2.32f;
            rectTransform.sizeDelta = newSize;

            lastLvl += 1;
        }
    }

    public void GamePause()
    {
        Time.timeScale = 0;
        inGameUI.SetActive(false);
        onPauseMenu.SetActive(true);
        gamePaused = true;
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    public void GameResume()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inGameUI.SetActive(true);
        onPauseMenu.SetActive(false);
        Time.timeScale = 1;
        gamePaused = false;
        Debug.Log("Ressumed");
    }
}
