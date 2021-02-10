using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    // configuration parameters
    [Header("Player Properties")]
    [SerializeField] float startEnergy = 0.25f;
    [SerializeField] float currentEnergy = 0.5f;
    [SerializeField] float liftStopTime = 5f;
    [SerializeField] GameObject deathVFX;

    [Header("Lift Parameters")]
    [SerializeField] float playerLiftPower = 500f;
    [SerializeField] float storedPulse = 0f;
    [SerializeField] float maxCharge = 100f;
    [SerializeField] float pressTime = 0f;
    [SerializeField] float weightClass = 0f;
    [SerializeField] float weightLiftingTime = 0f;
    [SerializeField] Slider chargeLevelDisplay;
    
    [Header("Debug Parameters")]
    [SerializeField] float energyRawInputThrow;
    [SerializeField] float energyCycledThrow;
    [SerializeField] float lastEnergyInput;
    [SerializeField] float maxEnergyRawInputAchieved = 0f;
    [SerializeField] float lastPlayerEnergy = 0f;
    [SerializeField] float EnergyGrowthRate = 0f;
    [SerializeField] float maxStoredAchieved = 0f;
    [SerializeField] bool playerTriggered = false;

    // cached references
    private GameManager gameManager;
    Animator myAnimator;

    private void Awake()
    {
        gameManager = FindObjectOfType<GameManager>();
        myAnimator = GetComponent<Animator>();

    }

    private void Start()
    {

        chargeLevelDisplay.maxValue = maxCharge;
        chargeLevelDisplay.value = 0f;

        weightClass = gameManager.GetTargetLift().GetWeight();
        weightLiftingTime = gameManager.GetTargetLift().GetSuccessLiftingTime();

    }

    private void Update()
    {
        // ProcessPlayerInputThrow();

        if (playerTriggered)
        {
            liftStopTime -= Time.deltaTime;
        }

        if (liftStopTime <= Mathf.Epsilon)
        {
            return;
        } 
        
        energyRawInputThrow = Mathf.Abs(Input.GetAxis("Rotate"));
        maxEnergyRawInputAchieved = Mathf.Max(maxEnergyRawInputAchieved, energyRawInputThrow);

        if (energyRawInputThrow > Mathf.Epsilon)
        {
            playerTriggered = true;
            energyCycledThrow = ProcessRawIntoCycledEnergy(maxEnergyRawInputAchieved, chargeLevelDisplay.maxValue);
        }

        chargeLevelDisplay.transform.GetChild(0).GetComponent<Text>().text = liftStopTime.ToString("0.000");
        chargeLevelDisplay.value = energyCycledThrow;

    }

    private float ProcessRawIntoCycledEnergy(float rawInput, float amplitude)
    {
        return Mathf.PingPong(playerLiftPower *  energyRawInputThrow * Time.time, amplitude);
        //return (amplitude/1.5f) * (Mathf.Sin(10f * energyRawInputThrow * Time.time - Mathf.PI / 2) + 0.5f);
    }

    private void ProcessPlayerInputThrow()
    {
        energyRawInputThrow = Mathf.Abs(Input.GetAxisRaw("Rotate"));

        EnergyGrowthRate = (currentEnergy - lastPlayerEnergy);
        lastPlayerEnergy = currentEnergy;

        maxEnergyRawInputAchieved = Mathf.Max(currentEnergy, maxEnergyRawInputAchieved);

        if (energyRawInputThrow >= Mathf.Epsilon && !playerTriggered)
        {
            ChargePulse(energyRawInputThrow);
        }
        else if (energyRawInputThrow <= Mathf.Epsilon && !StoredPulseIsEmpty())
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
    }

    private void ChargePulse(float amount)
    {
        maxStoredAchieved = Mathf.Max(maxStoredAchieved, storedPulse);
        storedPulse = playerLiftPower * amount;
        chargeLevelDisplay.value = storedPulse;
    }
        
    // declared this a coroutine so that it runs not at every frame, but suspends until completed.
    IEnumerator ReleaseStoredEnergy()
    {
        pressTime += Time.deltaTime;
        if (pressTime >= weightLiftingTime)
        {
            storedPulse = 0f;
            yield break;
        }

        playerTriggered = true;
        currentEnergy += storedPulse/2f * Time.deltaTime;
        storedPulse = Mathf.Lerp(maxStoredAchieved, 0f, 0.5f*Mathf.Log(1 + pressTime / weightLiftingTime));
        chargeLevelDisplay.value = storedPulse;
        yield return new WaitForSeconds(1f);


    }

    bool StoredPulseIsEmpty()
    {
        if (storedPulse >= Mathf.Epsilon)
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

