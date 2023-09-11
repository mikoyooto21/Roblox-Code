using UnityEngine;

public class GameDataManager : MonoBehaviour
{
    public static GameDataManager Instance { private set; get; }

    [SerializeField] private SkinsConfig _skinsConfig;

    private int _playerCountCoins;

    public SkinsConfig SkinsConfig => _skinsConfig;
    public int PlayerCountCoins => _playerCountCoins;

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

}
