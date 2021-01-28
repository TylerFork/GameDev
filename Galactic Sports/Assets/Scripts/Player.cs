#define DEBUG

using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    // configuration parameters
    [Header("Circle Properties")]
    [SerializeField] Slider playerSlider;
    [SerializeField] float startEnergy = 0.25f;
    [SerializeField] float currentEnergy = 0.5f;
    [SerializeField] GameObject deathVFX;

    [Header("Energy Decay")]
    [SerializeField] float decaySpeed = 0.1f;
    [SerializeField] float decayAcceleration = 0.1f;

    [Header("Charge Parameters")]
    [SerializeField] float chargeRate = 1f;
    [SerializeField] float releaseRate = 0.1f; 
    [SerializeField] float storedPulse = 0f;
    [SerializeField] float maxCharge = 100f;
    [SerializeField] Slider chargeLevelDisplay;
    
    [Header("Debug Parameters")]
    [SerializeField] float energyInput;
    [SerializeField] float lastEnergyInput;
    [SerializeField] float inputVelocity = 0f;
    [SerializeField] float minVelocityAchieved = 0f;
    [SerializeField] float maxVelocityAchieved = 0f;
    [SerializeField] float maxEnergyAchieved = 0f;
    [SerializeField] float lockEnergy = 0.25f;
    [SerializeField] float flickSpeedThreshold = -0.5f;
    [SerializeField] float lastPlayerEnergy = 0f;
    [SerializeField] float EnergyGrowthRate = 0f;
    [SerializeField] bool playerTriggered = false;

    // cached references
    private GameManager gameManager;
    Animator myAnimator;

    InitialParams initialParams = new InitialParams();

    class InitialParams
    {
        public float m_decaySpeed, m_decayAcceleration, m_chargeRate, m_lockRadius;

        public InitialParams() { }

        public InitialParams(float decayS, float decayA, float chgR, float lockRad)
        {
            this.m_decaySpeed = decayS;
            this.m_decayAcceleration = decayA;
            this.m_chargeRate = chgR;
            this.m_lockRadius = lockRad;
        }
    }

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        myAnimator = GetComponent<Animator>();

    }

    private void Start()
    {
        lockEnergy = startEnergy;
        playerSlider.value = currentEnergy;

        initialParams.m_decaySpeed = decaySpeed;
        initialParams.m_decayAcceleration = decayAcceleration;
        initialParams.m_chargeRate = chargeRate;
        initialParams.m_lockRadius = lockEnergy;

        chargeLevelDisplay.maxValue = maxCharge;
        chargeLevelDisplay.value = 0f;

    }

    private void Update()
    {

        //currentEnergy -= CalculateEnergyDecay();
        energyInput = Mathf.Abs(Input.GetAxisRaw("Rotate"));

        inputVelocity = (energyInput - lastEnergyInput);
        minVelocityAchieved = Mathf.Min(minVelocityAchieved, inputVelocity);
        maxVelocityAchieved = Mathf.Max(maxVelocityAchieved, inputVelocity);
        lastEnergyInput = energyInput;

        EnergyGrowthRate = (currentEnergy - lastPlayerEnergy);
        lastPlayerEnergy = currentEnergy;

        maxEnergyAchieved = Mathf.Max(currentEnergy, maxEnergyAchieved);

        if (energyInput > 0f )//&& !playerTriggered)
        {
            ChargePulse(energyInput);
        }
        else if (energyInput == 0f && !StoredPulseIsEmpty())
        {
            myAnimator.SetTrigger("liftTrigger");
            StartCoroutine(ReleaseStoredEnergy());
            Debug.Log("Coroutine started.");
        }

        if (StoredPulseIsEmpty())
        {
            StopCoroutine(ReleaseStoredEnergy());
            Debug.Log("Coroutine stopped.");

        }

        HandleEnergyLock();
        currentEnergy = Mathf.Max(currentEnergy, lockEnergy);


        if (Input.GetButton("Jump"))
        {
            ResetParameters(initialParams);
        }

        playerSlider.value = currentEnergy;      
    }

    private void HandleEnergyLock()
    {
        bool IsEnergyIncreasing = EnergyGrowthRate > 0f;

        if (playerTriggered && !IsEnergyIncreasing)
        {

            //lockEnergy = maxEnergyAchieved * 0.9f;
            lockEnergy = 0f;
        }
    }
  
    private float CalculateEnergyDecay()
    {
        return (decaySpeed + 0.5f * decayAcceleration * Time.deltaTime) * Time.deltaTime;
    }

    private void ChargePulse(float amount)
    {
        storedPulse += chargeRate * amount;
        storedPulse = Mathf.Clamp(storedPulse, 0f, maxCharge);
        chargeLevelDisplay.value = storedPulse;
    }
        
    // declared this a coroutine so that it runs not at every frame, but suspends until completed.
    IEnumerator ReleaseStoredEnergy()
    {
        
        //playerTriggered = true;
        currentEnergy += (releaseRate * storedPulse) * Time.deltaTime;
        storedPulse -= (releaseRate * storedPulse) * Time.deltaTime;
        storedPulse = Mathf.Floor(Mathf.Max(0f, storedPulse));        
        chargeLevelDisplay.value = storedPulse;
        yield return new WaitForSeconds(0.5f);

        
    }

    bool StoredPulseIsEmpty()
    {
        if (storedPulse > 0)
        {
            return false;
        } else
        {
            return true;
        }
    }

    public void DeathVFX()
    {
        GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation);
        Destroy(explosion, 1f);
    }

    public float GetCurrentEnergy()
    {
        return currentEnergy;
    }

    public float GetStartEnergy()
    {
        return startEnergy;
    }

    public float GetStoredPulse()
    {
        return storedPulse;
    }


#if DEBUG
    private void ResetParameters(InitialParams param)
    {
        decaySpeed = param.m_decaySpeed;
        decayAcceleration = param.m_decayAcceleration;
        chargeRate = param.m_chargeRate;
        lockEnergy = param.m_lockRadius;
        playerTriggered = false;
        maxEnergyAchieved = 0f;
        maxVelocityAchieved = 0f;
    }
#endif
//    // deprecated
//    private void DrawCircle(float inputRadius)
//    {
//        float lineWidth = 1;
//        lineRenderer.widthMultiplier = lineWidth;

//        float deltaTheta = (2f * Mathf.PI) / vertexCount;
//        float theta = 0f;

//        lineRenderer.positionCount = vertexCount;
//        lineRenderer.loop = true;
//        for (int i = 0; i < lineRenderer.positionCount; i++)
//        {
//            float x = inputRadius * Mathf.Cos(theta);
//            float y = inputRadius * Mathf.Sin(theta);
//            Vector3 pos = new Vector3(x, y, 0f);
//            lineRenderer.SetPosition(i, pos);
//            theta += deltaTheta;
//        }
//    }


//#if UNITY_EDITOR
//    private void OnDrawGizmos()
//    {
//        float deltaTheta = (2f * Mathf.PI) / vertexCount;
//        float theta = 0f;

//        Vector3 oldPos = Vector3.zero;
//        for (int i = 0; i < vertexCount + 1; i++)
//        {
//            float x = currentEnergy * Mathf.Cos(theta);
//            float y = currentEnergy * Mathf.Sin(theta);
//            Vector3 pos = new Vector3(x, y);
//            Gizmos.DrawLine(oldPos, transform.position + pos);
//            oldPos = transform.position + pos;

//            theta += deltaTheta;
//        }
//    }
//#endif

}

