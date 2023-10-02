using UnityEngine;

public class MenuDataManager : MonoBehaviour
{
    public static MenuDataManager Instance { private set; get; }

    [SerializeField] private SkinsConfig _skinsConfig;
    public SkinData _skinData;

    [SerializeField] private GameObject playerParent;

    public SkinsConfig SkinsConfig => _skinsConfig;

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

        PlayerPrefs.SetInt("PlayerSkinId", 1);
    }

    private void Start()
    {
        SkinsConfig skinConfig = Instance.SkinsConfig;
        SetSkin(skinConfig.Skins[PlayerPrefs.GetInt("PlayerSkinId") - 1]);
    }

    public void SetSkin(SkinData skinData)
    {

        _skinData = skinData;

        if (_skinData == null)
            return;

        if (playerParent.transform.childCount > 0)
            Destroy(playerParent.transform.GetChild(0).gameObject);

        Instantiate(_skinData.SkinModelPrefab, playerParent.transform);
    }
}
