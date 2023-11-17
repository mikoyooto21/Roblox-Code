using HeneGames.Airplane;
using UnityEngine;

public class PlayerInputsContoller : MonoBehaviour
{
    public static PlayerInputsContoller Instance { private set; get; }

    [SerializeField] private AirplaneBtn _mobileAirplaneBtn;
    [SerializeField] private AirplaneBtn _desktopAirplaneBtn;

    [Space(5)]
    [SerializeField] private MobileInputController _mobileInputController;
    [SerializeField] private AirplaneMobileInputController _airplaneMobileInputController;
    [SerializeField] private DesktopInputController _desktopInputController;
    [SerializeField] private AirplaneDesktopInputController _airplaneDesktopInputController;

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

    public void ShowInputController(PlayerInputs playerInputs = null)
    {
        _mobileAirplaneBtn.gameObject.SetActive(false);
        _desktopAirplaneBtn.gameObject.SetActive(false);
        _airplaneMobileInputController.gameObject.SetActive(false);
        _airplaneDesktopInputController.gameObject.SetActive(false);

        if (Application.isMobilePlatform)
        {
            _mobileInputController.SetPlayerInputs(playerInputs);
            _airplaneMobileInputController.SetPlayerInputs(playerInputs);

            _mobileAirplaneBtn.gameObject.SetActive(true);
            _mobileInputController.gameObject.SetActive(true);
        }
        else
        {
            _desktopInputController.SetPlayerInputs(playerInputs);
            _airplaneDesktopInputController.SetPlayerInputs(playerInputs);

            _desktopAirplaneBtn.gameObject.SetActive(true);
            _desktopInputController.gameObject.SetActive(true);
        }
    }

    public void ShowAirplaneInputController(SimpleAirPlaneController airPlaneController)
    {
        _mobileAirplaneBtn.gameObject.SetActive(false);
        _desktopAirplaneBtn.gameObject.SetActive(false);
        _mobileInputController.gameObject.SetActive(false);
        _desktopInputController.gameObject.SetActive(false);

        if (Application.isMobilePlatform)
        {
            _mobileAirplaneBtn.gameObject.SetActive(true);
            _airplaneMobileInputController.SetAirPlaneController(airPlaneController);
            _airplaneMobileInputController.gameObject.SetActive(true);
        }
        else
        {
            _desktopAirplaneBtn.gameObject.SetActive(true);
            _airplaneDesktopInputController.SetAirPlaneController(airPlaneController);
            _airplaneDesktopInputController.gameObject.SetActive(true);
        }
    }

    public AirplaneBtn GetAirplaneBtn()
    {
        if (Application.isMobilePlatform)
            return _mobileAirplaneBtn;
        else
            return _desktopAirplaneBtn;
    }

    public void OnClickAirplaneBtn()
    {
        Airplane.Instance.OnClickAirplaneBtn();
    }
}
