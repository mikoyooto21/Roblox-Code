using UnityEngine;

public class AutoDestroyEffect : MonoBehaviour
{
    public float delay = 1f;

    private void Start()
    {
        Destroy(gameObject, delay);
    }
}