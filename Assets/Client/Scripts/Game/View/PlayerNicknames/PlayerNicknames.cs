using System.Collections.Generic;
using UnityEngine;

public class PlayerNicknames : MonoBehaviour
{
    [SerializeField] private PlayerNicknameTxt _playerNicknameTxtPrefab;

    private List<PlayerNicknameTxt> _playerNicknameTxtList;
    private List<PlayerController> _playersList;

    private Camera _camera;

    private void Start()
    {
        GameEvents.OnLevelBuiltEvent.AddListener(() =>
        {
            DisplayNames();
        });
    }

    private void DisplayNames()
    {
        _camera = Camera.main;
        _playersList = LevelController.Instance.LevelBuilder.PlayersList;

        Transform _transform = transform;
        _playerNicknameTxtList = new List<PlayerNicknameTxt>();

        for (int i = 0; i < _playersList.Count; i++)
        {
            PlayerNicknameTxt nicknameTxt = Instantiate(_playerNicknameTxtPrefab, _transform);
            _playerNicknameTxtList.Add(nicknameTxt);
        }
    }

    public void MyUpdate()
    {
        if (_playersList != null)
        {
            for (int i = 0; i < _playerNicknameTxtList.Count; i++)
            {
                if (i >= _playersList.Count)
                {
                    _playerNicknameTxtList[i].SetNickname(Vector2.zero, "");
                }
                else
                {
                    Vector3 screenPosition = _camera.WorldToScreenPoint(_playersList[i].NicknameTxtPosition.position);

                    if (IsOnScreen(screenPosition))
                        _playerNicknameTxtList[i].SetNickname(screenPosition, _playersList[i].Nickname);
                    else
                        _playerNicknameTxtList[i].SetNickname(Vector2.zero, "");
                }
            }
        }
    }

    private bool IsOnScreen(Vector3 position)
    {
        return position.x >= 0 && position.x <= Screen.width && position.y >= 0 && position.y <= Screen.height && position.z > 0;
    }
}
