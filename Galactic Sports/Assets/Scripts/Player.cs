using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{

    // configuration parameters
    [Header("Player Properties")]
    [SerializeField] GameObject deathVFX;

    [Header("Lift Parameters")]
    [SerializeField] float maxCharge = 100f;
    [SerializeField] float weightClass = 0f;
    [SerializeField] float frequencyStrength = 500f;
    [SerializeField] float weightLiftingTime = 0f;
    [SerializeField] AnimationCurve weightLiftCurve;
    [SerializeField] Slider chargeLevelDisplay;
    
    [Header("Debug Parameters")]
    [SerializeField] float energyRawInputThrow;
    [SerializeField] float energyCycledThrow;
    [SerializeField] bool playerStarted = false;
    [SerializeField] bool playerPressedConfirmButton = false;

    float processedFrequency;

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
        weightLiftCurve = gameManager.GetLevelWeightCollection().GetLiftCurve();

        chargeLevelDisplay.maxValue = maxCharge;
        chargeLevelDisplay.value = 0f;

        weightClass = gameManager.GetLevelWeightCollection().GetTotalWeight();
        weightLiftingTime = gameManager.GetLevelWeightCollection().GetSuccessLiftingTime();

    }

    private void Update()
    {
        // ProcessPlayerInputThrow();
        if ((playerStarted && Input.GetButtonDown("Jump")) | (weightLiftingTime <= Mathf.Epsilon))
        {
            playerPressedConfirmButton = true;
        }

        if (energyRawInputThrow > Mathf.Epsilon)
        {
            playerStarted = true;
        }
        
        energyRawInputThrow = Mathf.Abs(Input.GetAxis("Vertical"));

        if (playerStarted && !playerPressedConfirmButton)
        {
            weightLiftingTime -= Time.deltaTime;
            //processedFrequency = frequencyStrength * weightLiftCurve.Evaluate(energyRawInputThrow);
            //StartCoroutine(CyclicEnergyBar());
            energyCycledThrow = ProcessRawIntoCycledEnergy(energyRawInputThrow, chargeLevelDisplay.maxValue);
        }

        chargeLevelDisplay.transform.GetChild(0).GetComponent<Text>().text = weightLiftingTime.ToString("0.000");
        chargeLevelDisplay.value = energyCycledThrow;

    }

    ///<Summary>
    ///Processes player input into a cycled version of the input.
    ///
    ///</Summary>:
    private float ProcessRawIntoCycledEnergy(float rawInput, float amplitude)
    {
        float processedFrequency = frequencyStrength * weightLiftCurve.Evaluate(rawInput);
        //float processedInput = Mathf.Floor(rawInput*cycleFrequency);
        return (amplitude / 1.5f) * (Mathf.Sin(processedFrequency*Time.time - Mathf.PI / 2) + 0.5f);
    }

    IEnumerator CyclicEnergyBar()
    {
        energyCycledThrow = (chargeLevelDisplay.maxValue / 1.5f) * (Mathf.Sin(processedFrequency * Time.time - Mathf.PI / 2) + 0.5f);
        yield return null;
    }
     
    public void DeathVFX()
    {
        GameObject explosion = Instantiate(deathVFX, transform.position, transform.rotation);
        Destroy(explosion, 1f);
    }

    public float GetCurrentEnergy()
    {
        return energyCycledThrow;
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
    //private void ProcessPlayerInputThrow()
    //{
    //    energyRawInputThrow = Mathf.Abs(Input.GetAxisRaw("Rotate"));

    //    EnergyGrowthRate = (currentEnergy - lastPlayerEnergy);
    //    lastPlayerEnergy = currentEnergy;

    //    maxEnergyRawInputAchieved = Mathf.Max(currentEnergy, maxEnergyRawInputAchieved);

    //    if (energyRawInputThrow >= Mathf.Epsilon && !playerTriggered)
    //    {
    //        ChargePulse(energyRawInputThrow);
    //    }
    //    else if (energyRawInputThrow <= Mathf.Epsilon && !StoredPulseIsEmpty())
    //    {
    //        myAnimator.SetTrigger("liftTrigger");
    //        StartCoroutine(ReleaseStoredEnergy());
    //        Debug.Log("Coroutine started.");
    //    }

    //    if (StoredPulseIsEmpty())
    //    {
    //        StopCoroutine(ReleaseStoredEnergy());
    //        Debug.Log("Coroutine stopped.");

    //    }
    //}

    //private void ChargePulse(float amount)
    //{
    //    maxStoredAchieved = Mathf.Max(maxStoredAchieved, storedPulse);
    //    storedPulse = playerLiftPower * amount;
    //    chargeLevelDisplay.value = storedPulse;
    //}
    //// declared this a coroutine so that it runs not at every frame, but suspends until completed.
    //IEnumerator ReleaseStoredEnergy()
    //{
    //    pressTime += Time.deltaTime;
    //    if (pressTime >= weightLiftingTime)
    //    {
    //        storedPulse = 0f;
    //        yield break;
    //    }

    //    playerTriggered = true;
    //    currentEnergy += storedPulse/2f * Time.deltaTime;
    //    storedPulse = Mathf.Lerp(maxStoredAchieved, 0f, 0.5f*Mathf.Log(1 + pressTime / weightLiftingTime));
    //    chargeLevelDisplay.value = storedPulse;
    //    yield return new WaitForSeconds(1f);


    //}

    //bool StoredPulseIsEmpty()
    //{
    //    if (storedPulse >= Mathf.Epsilon)
    //    {
    //        return false;
    //    } else
    //    {
    //        return true;
    //    }
    //}


    //public float GetStartEnergy()
    //{
    //    return startEnergy;
    //}



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

