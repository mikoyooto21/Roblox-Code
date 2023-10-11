using UnityEngine;
using YG;

public class AdManager : MonoBehaviour
{
    public static AdManager Instance { private set; get; }

    [SerializeField] private float _adTimeout = 100;

    private float _lastAdShownTime;

    private void OnDestroy()
    {
        if (Instance == this)
            Instance = null;
    }

    private void Awake()
    {
        if (Instance == null)
            Instance = this;

        else if (Instance != this)
            Destroy(gameObject);
    }

    public void ShowAd()
    {

        // show ad
        if (Time.time - _lastAdShownTime < _adTimeout)
            return;

        Debug.Log("_adTimeout: " + (Time.time - _lastAdShownTime));

        YandexGame.FullscreenShow();
        _lastAdShownTime = Time.time;
    }
}
