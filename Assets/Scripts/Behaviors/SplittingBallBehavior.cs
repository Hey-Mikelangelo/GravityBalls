using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(LightWeightRigidbody))]
public class SplittingBallBehavior : MonoBehaviour, IPoolable
{
    private new LightWeightRigidbody rigidbody;
    [SerializeField] private SpawnableType ballSpawnableType;
    [SerializeField] private float minForce;
    [SerializeField] private float maxForce;
    [SerializeField] private float criticalMass;

    [SerializeField] private float primaryBallMass = 1;
    [SerializeField] private float disabledInteractionsTime = 0.5f;
    private GravitationalAttractionBehavior gravitationalAttraction;
    private SphereCollider sphereCollider;
    private PrefabPool ballsPool;
    private Timer enableInteractionsTimer;

    private float prevMass;
    public void BeforeReturnToPool()
    {
        enableInteractionsTimer.StopAndReset();
    }

    public void OnSpawnFromPool()
    {
        prevMass = rigidbody.Mass;
    }

    private void Awake()
    {
        rigidbody = GetComponent<LightWeightRigidbody>();
        sphereCollider = GetComponent<SphereCollider>();
        gravitationalAttraction = GetComponent<GravitationalAttractionBehavior>();
        enableInteractionsTimer = new Timer(this);
    }

    private void OnEnable()
    {
        enableInteractionsTimer.OnTimerCycle += EnableInteractionsTimer_OnTimerCycle;
    }

    private void OnDisable()
    {
        enableInteractionsTimer.OnTimerCycle -= EnableInteractionsTimer_OnTimerCycle;
    }

    private void Start()
    {
        ballsPool = Pool.Instance.GetPool(ballSpawnableType);
    }

    private void FixedUpdate()
    {
        if (rigidbody.Mass != prevMass)
        {
            if (rigidbody.Mass >= criticalMass)
            {
                BreakIntoPrimaryBalls(rigidbody.Mass, primaryBallMass);
            }
            prevMass = rigidbody.Mass;
        }
    }

    private void BreakIntoPrimaryBalls(float bigBallsMass, float primaryBallMass)
    {
        float primaryBallsCount = bigBallsMass / primaryBallMass;
        int concretePrimaryBallsCount = (int)(primaryBallsCount);

        for (int i = 0; i < concretePrimaryBallsCount; i++)
        {
            Poolable poolableBall = ballsPool.GetInstance(returnEnabled: false);
            SplittingBallBehavior otherSplittingBall = poolableBall.GetComponent<SplittingBallBehavior>();
            otherSplittingBall.DisableInteractions();
            otherSplittingBall.gameObject.SetActive(true);
            otherSplittingBall.EnableInteractionsAfterTime(disabledInteractionsTime);

            poolableBall.transform.position = transform.position;
            Vector3 randomForce = MathHelper.GetRandomVector3(-maxForce, maxForce);
            otherSplittingBall.rigidbody.AddForce(randomForce);
        }

        Poolable.DestroyPoolable(gameObject);
    }

    private void DisableInteractions()
    {
        sphereCollider.enabled = false;
        if (gravitationalAttraction != null)
        {
            gravitationalAttraction.enabled = false;
        }
    }

    private void EnableInteractionsTimer_OnTimerCycle()
    {
        sphereCollider.enabled = true;
        if (gravitationalAttraction != null)
        {
            gravitationalAttraction.enabled = true;
        }
        enableInteractionsTimer.StopAndReset();
    }

    private void EnableInteractionsAfterTime(float time)
    {
        enableInteractionsTimer.SetCycleTime(time);
        enableInteractionsTimer.Start();
        //continues in EnableInteractionsTimer_OnTimerCycle
    }

}
