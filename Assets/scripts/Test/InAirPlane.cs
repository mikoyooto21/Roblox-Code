using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InAirPlane : MonoBehaviour
{
    [SerializeField] private GameObject _prefabAirPlane;
    [SerializeField] private PlayerMovement _playerMovement;
    [SerializeField] private CapsuleCollider _targetCollider;
    [SerializeField] private GameObject _player;
    [SerializeField] private GameObject _effect;
    ///////////////////////////////////////////////////////
    private bool _cheat = false;
    private string cheatCode = "uio";
    private string input = "";
    private float resetDelay = 1f;
    private float lastInputTime;

    private GameObject _currentAirplaneInstance;
    private bool _isActivePlane = false;

    private void Update()
    {
        TpInPlane();
        CheckButtonAirPlane();
        if (Time.time - lastInputTime > resetDelay)
        {
            input = "";
        }

        foreach (char c in Input.inputString)
        {
            input += c;
            lastInputTime = Time.time;

            if (input == cheatCode)
            {
                ActivateCheat();
            }

            if (input.Length > cheatCode.Length)
            {
                input = input.Substring(input.Length - cheatCode.Length);
            }
        }
    }

    private void CheckButtonAirPlane()
    {
        if (Input.GetKeyDown(KeyCode.G) && _cheat)
        {
            if (!_isActivePlane)
            {
                InPlane();
                _isActivePlane = true;
            }
            else
            {
                ReturnPlayer();
                _isActivePlane = false;
            }
        }
    }

    private void InPlane()
    {
        _playerMovement.enabled = false;
        _targetCollider.isTrigger = true;
        _playerMovement.SetActiveSkinPlayer(false);
        Instantiate(_effect, _player.transform.position, Quaternion.identity);
        Debug.Log("In plane");

        Vector3 posOfSpawn = _player.transform.position;
        Quaternion rotation = _player.transform.rotation;

        _currentAirplaneInstance = Instantiate(_prefabAirPlane, posOfSpawn, rotation);
    }

    private void ReturnPlayer()
    {
        _playerMovement.enabled = true;
        _targetCollider.isTrigger = false;
        _playerMovement.SetActiveSkinPlayer(true);
        Instantiate(_effect, _player.transform.position, Quaternion.identity);
        Debug.Log("Out plane");
        if (_currentAirplaneInstance != null)
            Destroy(_currentAirplaneInstance);
    }

    private void TpInPlane()
    {
        if (_isActivePlane && _currentAirplaneInstance != null)
            _player.transform.position = _currentAirplaneInstance.transform.position;
    }
    void ActivateCheat()
    {
        Debug.Log("Cheat activated!");
        _cheat = true;
    }
}