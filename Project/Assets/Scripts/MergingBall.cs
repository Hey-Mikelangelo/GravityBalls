using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(SphereCollider), typeof(Rigidbody), typeof(GravitationalAttraction))]
public class MergingBall : MonoBehaviour
{
    public int maxBallsToMerge = 50;
    public float maxForce;
    public ObjectsPool ballsPool;
    public SwitchEventCaller collisionStateChangeCaller;

    public static int ballsCount = 0;

    [HideInInspector]public List<MergingBall> _MergingBallsList = null;
    [HideInInspector]public bool isConsumed;

    [HideInInspector] public SphereCollider sphereCollider;
    [HideInInspector] public Rigidbody rb;
    [HideInInspector] public GravitationalAttraction gravitationalAttraction;
    private float _primaryMass;

    public const float PI4 = Mathf.PI * 4;

    private void Awake()
    {
        sphereCollider = GetComponent<SphereCollider>();
        rb = GetComponent<Rigidbody>();
        gravitationalAttraction = GetComponent<GravitationalAttraction>();
        _primaryMass = rb.mass;
        _MergingBallsList = null;
    }
    private void OnEnable()
    {
        isConsumed = false;
        ballsCount++;
        
        collisionStateChangeCaller.onSwitchOn += OnSwitchOn;

        collisionStateChangeCaller.onSwitchOff += OnSwitchOff;
    }
    private void OnDisable()
    {
        _MergingBallsList = null;
        ballsCount--;
        collisionStateChangeCaller.onSwitchOn -= OnSwitchOn;

        collisionStateChangeCaller.onSwitchOff -= OnSwitchOff;
    }
    void OnSwitchOn()
    {
        gravitationalAttraction.enabled = false;
    }
    void OnSwitchOff()
    {
        sphereCollider.enabled = true;
    }
    void InverseGravity()
    {
        //make inverse gravity
        gravitationalAttraction.gravityMultiplier *= -1;
        gravitationalAttraction.enabled = true;
    }
    private void FixedUpdate()
    {
        if (_MergingBallsList == null)
        {
            return;
        }
        if (_MergingBallsList.Count > 1)
        {
            MergeBalls(_MergingBallsList);
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (isConsumed)
        {
            return;
        }
        MergingBall otherBall;

        if (collision.gameObject.TryGetComponent(out otherBall))
        {
            
            if (_MergingBallsList == null)
            {
                _MergingBallsList = new List<MergingBall>(2);
                _MergingBallsList.Add(this);
            }
            if (otherBall._MergingBallsList != null)
            {
                //copy other ball's linked balls list (which includes other ball)
                List<MergingBall> otherBallMergingList = otherBall._MergingBallsList;
                
                for (int i = 0; i < otherBallMergingList.Count; i++)
                {
                    this._MergingBallsList.Add(otherBallMergingList[i]);
                }
            }
            else
            {
                _MergingBallsList.Add(otherBall);
            }
            //set flag to not allow other ball delete this ball
            otherBall.isConsumed = true;
            //disable other balls collider (disabling will be processed after all other collisions in current frame)
            otherBall.sphereCollider.enabled = false;

        }
    }

    void BreakIntoPrimaryBalls(float bigBallsMass)
    {
        float primaryBallsCount = bigBallsMass / _primaryMass;
        primaryBallsCount = (int)primaryBallsCount;
        GameObject ball;
        MergingBall mergingBall;
        Vector3 randomForce;
        for (int i = 0; i < primaryBallsCount; i++)
        {
            ball = ballsPool.GetObject(true, false);
            //position all balls in center of bigg ball
            ball.transform.position = transform.position;
            //disable colliders
            ball.GetComponent<Collider>().enabled = false;
            //enable gameobject
            ball.gameObject.SetActive(true);
            
            //call function which will call event which will trigger collider enabling
            mergingBall = ball.GetComponent<MergingBall>();
            mergingBall.collisionStateChangeCaller.StartOneSwitch();
            //generate and add random force
            randomForce = MathHelper.GetRandom3(-maxForce, maxForce);
            mergingBall.rb.velocity = randomForce;
            //mergingBall.rb.AddForce(randomForce);
        }
        //reset balls parameters
        ResetRigidbodyParameters(rb);
        //disable this big ball
        ObjectPoolReturner poolReturner;
        if (TryGetComponent(out poolReturner))
        {
            poolReturner.canReturn = true;
        }
        gameObject.SetActive(false);
    }
    void ResetRigidbodyParameters(Rigidbody rb)
    {
        rb.mass = _primaryMass;
        rb.velocity = Vector3.zero;

    }
    void MergeBalls(List<MergingBall> MergingBallsList)
    {
        float massSum;
        Vector3 centerOfMass;
        //find the center of mass
        GetCenterOfMass(MergingBallsList, out centerOfMass, out massSum);
        //find area surface sum
        float areaSum = GetBallsSurfaceArea(MergingBallsList);
        float newRadius = Mathf.Sqrt(areaSum / PI4);
        float newScale = newRadius * 2;

        MergingBall mainBall = this;
       
        //increase this ball's mass 
        mainBall.rb.mass = massSum;
        //move this ball to the center of mass
        mainBall.rb.position = centerOfMass;
        //scale this ball to make surface area of main as equal to sum of surface areas of all merged balls
        mainBall.transform.localScale = new Vector3(newScale, newScale, newScale);

        //loop through others ball's Merged pools omiting first element of MergingBallsList because it is main ball
        //and disable balls
        for (int i = 1; i < MergingBallsList.Count; i++)
        {
            MergingBall otherBall = MergingBallsList[i];
            ObjectPoolReturner poolReturner;
            if(otherBall.TryGetComponent(out poolReturner))
            {
                poolReturner.canReturn = true;
            }
            ResetRigidbodyParameters(otherBall.rb);
            otherBall.gameObject.SetActive(false);
            //enable collider because this object will be used later
            otherBall.sphereCollider.enabled = true;
        }
           
        MergingBallsList.Clear();
        MergingBallsList.Add(this);

        if (rb.mass >= _primaryMass * maxBallsToMerge)
        {
            BreakIntoPrimaryBalls(rb.mass);
        }
    }
    
    float GetBallsSurfaceArea(List<MergingBall> MergingBalls)
    {
        float sum = 0;
        float radius;
        for (int i = 0; i < MergingBalls.Count; i++)
        {
            radius = MergingBalls[i].transform.localScale.x / 2;
            sum += PI4 * radius * radius;
        }
        return sum;
    }
    void GetCenterOfMass(List<MergingBall> MergingBalls, out Vector3 centerOfMass, out float massSum)
    {
        Vector3 centOfMass = new Vector3(0, 0, 0);
        float mSum = 0;
        Rigidbody currentRb;
        for (int i = 0; i < MergingBalls.Count; i++)
        {
            currentRb = MergingBalls[i].rb;
            centOfMass += currentRb.position * currentRb.mass;
            mSum += currentRb.mass;
        }
        centOfMass /= mSum;
        massSum = mSum;
        centerOfMass = centOfMass;
    }
   
}
