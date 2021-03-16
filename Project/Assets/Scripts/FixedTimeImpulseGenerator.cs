using UnityEngine;
using UnityEngine.Events;

public class FixedTimeImpulseGenerator : MonoBehaviour
{
    public float timeDelay = 0.25f;
    public event UnityAction onImpulse;

    private bool _runTimer = false;
    private int _impulseCount = 0;
    private float _timeElapsed = 0;
    public void StartImpulses()
    {
        _runTimer = true;
    }
    public void StopImpulses()
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
            if (_timeElapsed >= timeDelay *_impulseCount)
            {
                _impulseCount++;
                onImpulse?.Invoke();
            }
        }
    }
}
