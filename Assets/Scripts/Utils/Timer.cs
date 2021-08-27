using System.Collections;
using UnityEngine;

public class Timer
{
    public float TimeElapsed { get; private set; }
    public event System.Action OnTimerCycle;
    public event System.Action onTimerStopped;
    public bool IsRunning { get; private set; }
    public float LocalTimeScale { get; private set; }

    private MonoBehaviour monoBehaviour;
    private Coroutine timerCoroutine;

    private float cycleTime = 0;
    private float cycleTimeElapsed;
    private float stopAfterTimeElapsed;

    public Timer(MonoBehaviour monoBehaviour)
    {
        this.monoBehaviour = monoBehaviour;
        StopAndReset();
        SetLocalTimeScale(1);
    }

    IEnumerator TimerCoroutine()
    {
        float deltaTime;
        while (true)
        {
            deltaTime = UnityEngine.Time.deltaTime;
            float timeAdd = deltaTime * LocalTimeScale;
            TimeElapsed += timeAdd;
            if (cycleTime != 0)
            {
                cycleTimeElapsed += timeAdd;
                if(cycleTimeElapsed >= cycleTime)
                {
                    cycleTimeElapsed = 0;
                    OnTimerCycle?.Invoke();
                }
            }
            if(TimeElapsed >= stopAfterTimeElapsed)
            {
                Stop();
            }
            yield return null;
        }
    }

    public void Start(float stopAfterTime = float.MaxValue)
    {
        stopAfterTimeElapsed = stopAfterTime;

        if (IsRunning)
        {
            return;
        }

        if (timerCoroutine != null)
        {
            monoBehaviour.StopCoroutine(timerCoroutine);
        }
        IsRunning = true;
        timerCoroutine = monoBehaviour.StartCoroutine(TimerCoroutine());
    }

    /// <summary>
    /// Stops also cycle
    /// </summary>
    public void Stop()
    {
        if (!IsRunning)
        {
            return;
        }
        if (timerCoroutine != null)
        {
            monoBehaviour.StopCoroutine(timerCoroutine);
        }
        IsRunning = false;
        stopAfterTimeElapsed = float.MaxValue;
        StopCycle();
        onTimerStopped?.Invoke();
    }

    /// <summary>
    /// Resets also cycle
    /// </summary>
    public void Reset()
    {
        this.TimeElapsed = 0;
        ResetCycle();
    }
    public void SetLocalTimeScale(float timeScale)
    {
        LocalTimeScale = timeScale;
    }

    /// <summary>
    /// cycle time cannot be 0
    /// </summary>
    public void SetCycleTime(float cycleTime)
    {
        cycleTime = Mathf.Abs(cycleTime);
        this.cycleTime = cycleTime;
    }

    public void StopCycle()
    {
        this.cycleTime = 0;
    }

    public void ResetCycle()
    {
        this.cycleTimeElapsed = 0;
    }

    public void StopAndReset()
    {
        Stop();
        Reset();
    }

}
