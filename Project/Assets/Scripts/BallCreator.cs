using TMPro;
using UnityEngine;

public class BallCreator : MonoBehaviour
{
    public ObjectsPool ballsPool;
    public RandomPointInCameraView randomPoint;
    public FixedTimeImpulseGenerator impulseGenerator;
    public TextMeshProUGUI ballsCountUI;

    private int _prevBallsCount, _currentBallsCount;
    public int maxBallCount = 20;
    public int ballsCount;
    private bool _stoppedImpulses;
    private void OnEnable()
    {
        SubsribeEvents();
        ballsPool.InitObjectsPool();
        ballsCount = 0;
        _stoppedImpulses = false;
    }

    private void OnDisable()
    {
        UnsubsribeEvents();
    }
    void SubsribeEvents()
    {
        impulseGenerator.onImpulse += OnFixedImpulse;
    }
    void UnsubsribeEvents()
    {
        impulseGenerator.onImpulse -= OnFixedImpulse;
    }

    void OnFixedImpulse()
    {
        //create new ball
        GameObject ballGO = ballsPool.GetObject();
        ballsCount++;
        //move to position in camera view
        ballGO.transform.position = randomPoint.GetPoint();
    }
    private void Start()
    {
        impulseGenerator.StartImpulses();
    }
    void Update()
    {
        _currentBallsCount = ballsCount;
        if(_prevBallsCount == _currentBallsCount)
        {
            return;
        }
        _prevBallsCount = _currentBallsCount;
        ballsCountUI.text = _currentBallsCount.ToString();
        if (_currentBallsCount == maxBallCount)
        {
            if (!_stoppedImpulses)
            {
                impulseGenerator.StopImpulses();
                impulseGenerator.ResetTimer();
                _stoppedImpulses = true;
                GravitationalAttraction.SetGlobalGravityMultiplier(-1);
            }
           
        }
    }
}
