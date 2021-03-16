using UnityEngine;
using UnityEngine.Events;

public class SwitchEventCaller : MonoBehaviour
{
    public event UnityAction onSwitchOn;
    public event UnityAction onSwitchOff;
    public float timeToSwitchOn = 1;
    public float timeToSwitchOff = 1;

    private bool _waitingForOff;
    private bool _runTimer = false;
    public float _timeElapsed = 0;

    private void OnEnable()
    {
        _waitingForOff = false;
    }
    private void OnDisable()
    {
    }
    public void StartOneSwitch()
    {
        _waitingForOff = false;
        _timeElapsed = 0;
        StartTimer();
    }
    public void StartTimer()
    {
        _runTimer = true;
    }
    public void StopTimer()
    {
        _runTimer = false;
    }
    public void ResetTimer()
    {
        _timeElapsed = 0;
    }
    private void Update()
    {
        if (_runTimer)
        {
            _timeElapsed += Time.deltaTime;
            if (!_waitingForOff && _timeElapsed >= timeToSwitchOn)
            {
                onSwitchOn?.Invoke();
                ResetTimer();
                _waitingForOff = true;
            }
            else if(_timeElapsed >= timeToSwitchOff)
            {
                onSwitchOff?.Invoke();
                ResetTimer();
                StopTimer();
                _waitingForOff = false;
            }
        }
    }
   
}
