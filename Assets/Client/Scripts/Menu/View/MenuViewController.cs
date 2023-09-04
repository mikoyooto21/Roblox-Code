using DG.Tweening;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuViewController : MonoBehaviour
{
    public static MenuViewController Instance { private set; get; }

    [SerializeField] private MenuView _menuView;
    [SerializeField] private ShopView _shopView;
    [SerializeField] private MenuLoadingView _menuLoadingView;

    public MenuView MenuView => _menuView;
    public ShopView ShopView => _shopView;
    public MenuLoadingView MenuLoadingView => _menuLoadingView;

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

    public void LoadGameView()
    {
        DOTween.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }

    public void LoadGameTimerView()
    {
        DOTween.Clear();
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 2);
    }
}
