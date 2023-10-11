using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MapController : MonoBehaviour
{
    public static MapController Instance { private set; get; }

    [SerializeField] private List<GameObject> _checkpointsList;

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

    public int GetCheckpointIdByGameObject(GameObject checkpoint)
    {
        for (int i = 0; i < _checkpointsList.Count; i++)
        {
            if (_checkpointsList[i] == checkpoint)
                return i + 1;
        }

        return 0;
    }

    public int GetCheckpointCount()
    {
        return _checkpointsList.Count;
    }
}
