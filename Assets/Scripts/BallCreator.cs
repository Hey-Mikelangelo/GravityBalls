using TMPro;
using UnityEngine;

public class BallCreator : MonoBehaviour
{
    [SerializeField] private Camera cam;
    [SerializeField] private float minDistance, maxDistance;
    [SerializeField] private SpawnableType spawnableType;
    [SerializeField] private TextMeshProUGUI ballsCountUI;
    [SerializeField] private int maxBallCount = 250;
    [SerializeField] private float spawnRate = 0.25f;

    private int ballsCount;

    private Timer timer;
    private PrefabPool ballsPool;
    private RandomPointInCameraView randomPointInCameraView = new RandomPointInCameraView();

    private void Awake()
    {
        timer = new Timer(this);
    }

    private void Start()
    {
        ballsPool = Pool.Instance.GetPool(spawnableType);
        timer.SetCycleTime(spawnRate);
        timer.Start();
    }
    private void OnEnable()
    {
        timer.OnTimerCycle += Timer_OnTimerCycle;
        ballsCount = 0;
    }

    private void OnDisable()
    {
        timer.OnTimerCycle -= Timer_OnTimerCycle;

    }

    private void Timer_OnTimerCycle()
    {
        Poolable poolable = ballsPool.GetInstance();
        ballsCount++;
        poolable.transform.position = randomPointInCameraView.GetPoint(cam, minDistance, maxDistance, 2, 10);
        
        if (ballsCountUI != null)
        {
            ballsCountUI.text = ballsCount.ToString();
        }

        if (ballsCount >= maxBallCount && timer.IsRunning)
        {
            timer.Stop();
            GravitationalAttractionBehavior.GlobalGravityMultilplier = -1;
            GlobalBallParamsChanger.SetCollidersIsTrigger(false);
            GlobalBallParamsChanger.SetRigidbodiesKinematic(false);
        }
    }
}

