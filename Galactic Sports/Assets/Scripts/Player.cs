using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    // configuration parameters
    [Header("Player Properties")]
    [SerializeField] Slider playerSlider;
    [SerializeField] float startEnergy = 0.25f;
    [SerializeField] float currentEnergy = 0.5f;
    [SerializeField] float liftStopTime = 3f;
    [SerializeField] GameObject deathVFX;

    [Header("Lift Parameters")]
    [SerializeField] float chargeRate = 0.5f;
    [SerializeField] float storedPulse = 0f;
    [SerializeField] float maxCharge = 100f;
    [SerializeField] float pressTime = 0f;
    [SerializeField] float weightClass = 0f;
    [SerializeField] float weightLiftingTime = 0f;
    [SerializeField] Slider chargeLevelDisplay;
    
    [Header("Debug Parameters")]
    [SerializeField] float energyInput;
    [SerializeField] float lastEnergyInput;
    [SerializeField] float maxEnergyAchieved = 0f;
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
        playerSlider.value = currentEnergy;

        chargeLevelDisplay.maxValue = maxCharge;
        chargeLevelDisplay.value = 0f;

        weightClass = gameManager.GetTargetLift().GetWeight();
        weightLiftingTime = gameManager.GetTargetLift().GetSuccessLiftingTime();

    }

    private void Update()
    {

        energyInput = Mathf.Abs(Input.GetAxisRaw("Rotate"));

        EnergyGrowthRate = (currentEnergy - lastPlayerEnergy);
        lastPlayerEnergy = currentEnergy;

        maxEnergyAchieved = Mathf.Max(currentEnergy, maxEnergyAchieved);

        if (energyInput >= Mathf.Epsilon && !playerTriggered)
        {
            ChargePulse(energyInput);
        }
        else if (energyInput <= Mathf.Epsilon && !StoredPulseIsEmpty())
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

        playerSlider.value = currentEnergy;      
    }

    private void ChargePulse(float amount)
    {
        maxStoredAchieved = Mathf.Max(maxStoredAchieved, storedPulse);
        storedPulse = chargeRate * amount;
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

