using UnityEngine;

public class MenuDataManager : MonoBehaviour
{
    public static MenuDataManager Instance { private set; get; }

    [SerializeField] private SkinsConfig _skinsConfig;

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
    }
}
