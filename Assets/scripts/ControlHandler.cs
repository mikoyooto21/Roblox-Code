using System.Collections;
using UnityEngine;
using YG;

public class ControlHandler : MonoBehaviour
{
    public GameObject MobilePanel;
    public PlayerMovement playerControll;
    public ThirdPersonCam CameraFollowToDisable;
    [SerializeField] bool _controllPC = false;
    [SerializeField] private Transform[] _transforms;
    [SerializeField] GameObject _buttonPausedPc;
    [SerializeField] private AudioSource _audioSource;
    private void Awake()
    {
        StartCoroutine(InitializeControls());
    }
    private IEnumerator InitializeControls()
    {
        // Ждем, пока YandexGame.EnvironmentData.isMobile инициализируется
        while (YandexGame.EnvironmentData.isMobile == null) // предполагая, что isMobile может быть nullable (или используйте другую подходящую проверку)
        {
            yield return null; // Ждем один кадр
        }

        _controllPC = !YandexGame.EnvironmentData.isMobile;

        if (!_controllPC)////////////////////////// Tuuuut fix !_controllPC
        {
            MobilePanel.SetActive(true);
            playerControll.ControllPc = false;
            CameraFollowToDisable.combatCam.SetActive(true);
            _buttonPausedPc.SetActive(false);
        }
    }
}