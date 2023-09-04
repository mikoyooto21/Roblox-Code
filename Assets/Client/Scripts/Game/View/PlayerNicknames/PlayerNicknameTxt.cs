using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerNicknameTxt : MonoBehaviour
{
    private TMP_Text _nickname;
    private Transform _transform;

    public void SetNickname(Vector2 position, string nickname)
    {
        if (_nickname == null || _transform == null)
        {
            _nickname = GetComponent<TMP_Text>();
            _transform = GetComponent<Transform>();
        }

        _transform.position = position;
        _nickname.text = nickname;
    }
}
