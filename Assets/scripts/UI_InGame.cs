using System.Collections;
using YG;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.SceneManagement;
using System;

public class UI_InGame : Coins
{

    // ������������� �� ������� GetDataEvent � OnEnable
    // private void OnEnable() => YandexGame.GetDataEvent += LoadData;

    // private void OnDisable() => YandexGame.GetDataEvent -= LoadData;


    public float simpleTimer = 0f;
    public static bool gamePaused = false;

    [Header("References")]
    public GameObject ad;
    public GameObject coins;
    public GameObject inGameUI;
    public GameObject onPauseMenu;
    public GameObject menuButtonPC;
    public GameObject menuButtonMobile;
    public GameObject continuePlayingView;
    [SerializeField] private GameObject MobileControl;
    [SerializeField] private GameObject Won;
    [SerializeField] private GameObject End;
    public TMP_Text timerText;
    public TMP_Text adTimerText;
    public Image checkpointsTrackerBar;
    public GameObject details;
    public GameObject player;
    RectTransform rectTransform;
    Vector2 newSize;

    float playerPosX;
    float playerPosY;
    float playerPosZ;

    bool switched = false;

    int lastCheckpointId = 0;

    public float adTimer = 80f;


    private bool _isShowTimer;
    private Coroutine _coroutine;

    private void OnEnable()
    {
        ProgressBarUI.OnCheckpointActivatedEvent.AddListener(OnCheckpointActivatedEvent);
    }

    void Start()
    {
        rectTransform = checkpointsTrackerBar.rectTransform;

        // if (PlayerPrefs.HasKey("PlayerExited"))
        // {
        //     if (PlayerPrefs.GetInt("PlayerExited") == 1)
        //     {
        //         if (PlayerPrefs.HasKey("PlayerPosX") && PlayerPrefs.HasKey("PlayerPosY") && PlayerPrefs.HasKey("PlayerPosZ"))
        //         {
        //             playerPosX = PlayerPrefs.GetFloat("PlayerPosX");
        //             playerPosY = PlayerPrefs.GetFloat("PlayerPosY");
        //             playerPosZ = PlayerPrefs.GetFloat("PlayerPosZ");

        //             Vector3 playerPosition = new Vector3(playerPosX, playerPosY, playerPosZ);
        //             player.transform.position = playerPosition;
        //             Debug.Log("Ui_InGame script " + player.transform.position);
        //         }
        //     }
        // }

        // if (YandexGame.SDKEnabled == true)
        // {
        //     // ���� ����������, �� ��������� ��� ����� ��� ��������
        //     if (YandexGame.savesData.isExited == 1)
        //         LoadData();

        //     // ���� ������ ��� �� �����������, �� ����� �� ���������� � ������ Start,
        //     // �� �� ���������� ��� ������ ������� GetDataEvent, ����� ��������� �������
        // }
        Debug.Log("IsExit " + PlayerPrefs.GetInt("PlayerExited"));

        MobileMenuButtonMaker();
        LoadCheckpointTracker();
        
        coinsCollected = PlayerPrefs.GetInt("Coins");
    }

    // Update is called once per frame
    void Update()
    {
        
        
        VideoAdNew();
        //CheckpointsTracker();

        if (!gamePaused)
        {
            if (SceneManager.GetActiveScene().buildIndex == 2)
                simpleTimer += Time.deltaTime;
            adTimer -= Time.deltaTime;
        }

        DisplayTime();
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

    void MobileMenuButtonMaker()
    {
        if (YandexGame.EnvironmentData.isMobile)
        {
            menuButtonPC.SetActive(false);
            menuButtonMobile.SetActive(true);
        }
        else if (YandexGame.EnvironmentData.isDesktop)
            MobileControl.SetActive(false);
    }
    void LoadCheckpointTracker()
    {
        if (PlayerPrefs.HasKey("PlayerExited"))
        {
            if (PlayerPrefs.GetInt("PlayerExited") == 1)
            {
                if (PlayerPrefs.HasKey("CheckpointsTrackerBarScaleX") && !switched)
                {
                    newSize.x = PlayerPrefs.GetFloat("CheckpointsTrackerBarScaleX");
                    switched = true;

                    // ������ ����� checkpointsTrackerBar
                    rectTransform.sizeDelta += newSize;
                }
            }
        }
    }


    void DisplayTime()
    {

        float minutes = Mathf.FloorToInt(simpleTimer / 60);
        float seconds = Mathf.FloorToInt(simpleTimer % 60);

        timerText.text = string.Format("{0:00}:{1:00}", minutes, seconds);
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            PlayerPrefs.SetString("LastTime", string.Format("{0:00}:{1:00}", minutes, seconds));
            PlayerPrefs.Save();
        }
    }

    public void CheckpointsTracker()
    {
        /*if (nextLvl == PlayerMovement.PassedLvls)
        {
            newSize = rectTransform.sizeDelta;
            newSize.x += 2.25f;
            rectTransform.sizeDelta = newSize;
            CollectCoin();
            nextLvl++;
        }*/
    }

    private void OnCheckpointActivatedEvent(int checkpointId)
    {
        if (lastCheckpointId == 0)
            lastCheckpointId = checkpointId;

        if (lastCheckpointId != checkpointId && lastCheckpointId < checkpointId)
        {
            newSize = rectTransform.sizeDelta;
            newSize.x += 2.25f;
            rectTransform.sizeDelta = newSize;
            CollectCoin();
            lastCheckpointId = checkpointId;
        }
    }

    public void GamePause()
    {
        gamePaused = true;
        PlayerPrefs.SetInt("PlayerExited", 1);
        YandexGame.savesData.isExited = 1;
        YandexGame.SaveProgress();
        RemoveDetails();
        Time.timeScale = 0;
        gamePaused = true;
        inGameUI.SetActive(false);
        onPauseMenu.SetActive(true);
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("Paused");
    }

    public void GameResume()
    {
        gamePaused = false;
        RemoveDetails();
        PlayerPrefs.SetInt("PlayerExited", 0);
        PlayerPrefs.Save();
        YandexGame.savesData.isExited = 0;
        YandexGame.SaveProgress();
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        inGameUI.SetActive(true);
        onPauseMenu.SetActive(false);
        Time.timeScale = 1;
        Debug.Log("Ressumed");
    }

    public void MapComplete()
    {
        gamePaused = true;
        Debug.Log("WIN");
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        if (SceneManager.GetActiveScene().buildIndex == 1)
        {
            PlayerPrefs.SetInt("UnlockedLevelWithTimer", 1); // 1 - ����� ������������
        }
        YandexGame.savesData.isExited = 0;
        YandexGame.SaveProgress();
        PlayerPrefs.SetInt("PlayerExited", 0);
        PlayerPrefs.Save();
        Time.timeScale = 0f;
        Won.SetActive(true);
    }

    public void MenuButton()
    {
        Time.timeScale = 1f;
        gamePaused = false;
        SaveCoins();
        SaveProgress();
        DOTween.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex - SceneManager.GetActiveScene().buildIndex);
    }



    public void SaveProgress()
    {
        
        if (SceneManager.GetActiveScene().buildIndex == 2)
        {
            if (PlayerPrefs.HasKey("time"))
            {
                if (PlayerPrefs.GetInt("time") < simpleTimer)
                    PlayerPrefs.SetInt("time", Mathf.RoundToInt(simpleTimer));
            }
            else
                PlayerPrefs.SetInt("time", Mathf.RoundToInt(simpleTimer));
            PlayerPrefs.Save();
            if ((YandexGame.savesData.time < simpleTimer) && YandexGame.savesData.time > 0)
                YandexGame.savesData.time = Mathf.RoundToInt(simpleTimer);
        }
        PlayerPrefs.SetInt("Coins", coinsCollected);
        YandexGame.savesData.money = coinsCollected;
        PlayerPrefs.Save();
        YandexGame.SaveProgress();
    }

    // public void LoadData()
    // {
    //     coinText.text = YandexGame.savesData.money.ToString();
    //     if (YandexGame.savesData.isExited == 1)
    //     {
    //         playerPosX = YandexGame.savesData.PlayerPosX;
    //         playerPosY = YandexGame.savesData.PlayerPosY;
    //         playerPosZ = YandexGame.savesData.PlayerPosZ;

    //         Vector3 playerPosition = new Vector3(playerPosX, playerPosY, playerPosZ);
    //         player.transform.position = playerPosition;
    //         coinsCollected = YandexGame.savesData.money;
    //         if (!switched)
    //         {
    //             newSize.x = YandexGame.savesData.CheckpointsTrackerBarScaleX;
    //             switched = true;

    //             // ������ ����� checkpointsTrackerBar
    //             rectTransform.sizeDelta += newSize;
    //         }
    //     }
    // }

    public void RemoveDetails()
    {
        if (gamePaused)
            details.SetActive(false);
        else details.SetActive(true);
    }

    public void VideoAdNew()
    {
        if (adTimer < 1)
        {
            /*VideoOpen();*/
        }
        if (adTimer < 3)
        {
            if (!_isShowTimer)
            {
                if (_coroutine != null)
                StopCoroutine(_coroutine);

                _coroutine = StartCoroutine(VideoAdNewTest());
            }
            /*ad.SetActive(true);
            adTimerText.text = string.Format("{0:0}...", adTimer);*/
        }
        else
            ad.SetActive(false);
    }

    IEnumerator VideoAdNewTest()
    {
        if (adTimer > 3)
            yield return null;

        Time.timeScale = 0;
        _isShowTimer = true;

        Debug.Log("START SHOW AD");

        for (int i = 3; i >= 0; i--)
        {
            if (i > 0)
            {
                ad.SetActive(true);
                adTimerText.text = string.Format("{0:0}...", i);
            }
            else
            {
                VideoOpen();
            }

            yield return new WaitForSecondsRealtime(1);
        }

        adTimer = 80;
        Time.timeScale = 1;
        _isShowTimer = false;
    }

    public void VideoOpen()
    {
        YandexGame.FullscreenShow();
    }
    public void VideoClose()
    {
        adTimer = 80;
    }
    public void VideoError()
    {
        adTimer = 80;
        Time.timeScale = 1;
        _isShowTimer = false;
    }
    public void ShowContinuePlayingView()
    {
        Time.timeScale = 0;
        continuePlayingView.SetActive(true);
    }
    public void ContinuePlaying()
    {
        adTimer = 80;
        Time.timeScale = 1;
        _isShowTimer = false;

        continuePlayingView.SetActive(false);
    }
}
