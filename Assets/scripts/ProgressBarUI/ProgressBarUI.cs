using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class ProgressBarUI : MonoBehaviour
{
    #region Events
    public static readonly UnityEvent<int> OnCheckpointActivatedEvent = new UnityEvent<int>();

    public static void InvokeCheckpointActivatedEvent(int activatedCheckpointId)
    {
        OnCheckpointActivatedEvent.Invoke(activatedCheckpointId);
    }
    #endregion

    private Slider _slider;

    private void OnEnable()
    {
        OnCheckpointActivatedEvent.AddListener(OnCheckpointActivated);
    }

    private void Start()
    {
        if (_slider == null)
            _slider = GetComponent<Slider>();

        _slider.maxValue = MapController.Instance.GetCheckpointCount();
    }

    private void OnCheckpointActivated(int activatedCheckpointId)
    {
        if (_slider == null)
            _slider = GetComponent<Slider>();

        _slider.value = activatedCheckpointId;
    }
}
