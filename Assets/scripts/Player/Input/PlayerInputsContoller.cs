using UnityEngine;

public class PlayerInputsContoller : MonoBehaviour
{
    public static PlayerInputsContoller Instance { private set; get; }

    [SerializeField] private MobileInputController _mobileInputController;
    [SerializeField] private DesktopInputController _desktopInputController;

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

    public void ShowMobileInputController(PlayerInputs playerInputs)
    {
        _mobileInputController.SetPlayerInputs(playerInputs);
    }

    public void ShowDesktopInputController(PlayerInputs playerInputs)
    {
        _desktopInputController.SetPlayerInputs(playerInputs);
    }
}
