using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] WeightCollection currentLevelWeights;
    [SerializeField] Text scoreText;
    [SerializeField] Text remainingTime;

    float runningScore = 0f;
    float weightLiftingTime = 0f;
    bool isLevelTimerFinished = false;

    Player player;
    LevelLoader levelLoader;

    /// setup singleton pattern for this object
    private void Awake()
    {
        SetupSingleton();
    }

    private void SetupSingleton()
    {
        if (FindObjectsOfType<GameManager>().Length > 1)
        {
            Destroy(gameObject);
        } else
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    void Start()
    {
        scoreText.text = 0.ToString();
        weightLiftingTime = currentLevelWeights.GetSuccessLiftingTime();
        player = FindObjectOfType<Player>();
        levelLoader = FindObjectOfType<LevelLoader>();
    }

    private void Update()
    {
        weightLiftingTime = Mathf.Max(weightLiftingTime - Time.deltaTime);
        remainingTime.text = weightLiftingTime.ToString("0.0");

        if (weightLiftingTime <= Mathf.Epsilon)
        {
            isLevelTimerFinished = true;
            player.SetIsLevelTimerOverFlag();
            StopTimer();
        }

        if (player.GetPlayerPressedConfirmButton() | isLevelTimerFinished)
        {
            scoreText.text = runningScore.ToString("0");
            StopTimer();
            StartCoroutine(levelLoader.WaitForTime());

        }

    }
 
    void StopTimer()
    {
        remainingTime.enabled = false;
    }


    public void CalculateAndAddToRunningScore()
    {
        float minScoringRange, maxScoringRange, playerEnergy, targetEnergy, playerEnergyRatioAwayFromTarget;
        playerEnergy = player.GetCurrentEnergy();
        targetEnergy = currentLevelWeights.GetTotalWeight();
        minScoringRange = currentLevelWeights.GetScoreRange().Item1;
        maxScoringRange = currentLevelWeights.GetScoreRange().Item2;


        if ((playerEnergy < minScoringRange) | (playerEnergy > maxScoringRange))
        {
            runningScore += 0f;
            return;
        }

        if (playerEnergy > targetEnergy)
        {
            playerEnergyRatioAwayFromTarget = Mathf.Abs(targetEnergy - playerEnergy) / Mathf.Abs(targetEnergy - maxScoringRange);
        } else if (playerEnergy < targetEnergy)
        {
            playerEnergyRatioAwayFromTarget = Mathf.Abs(targetEnergy - playerEnergy) / Mathf.Abs(targetEnergy - minScoringRange);
        } else
        {
            playerEnergyRatioAwayFromTarget = 0f;

        }

        runningScore += Mathf.Lerp(100f, 0f, playerEnergyRatioAwayFromTarget);
    }

    public WeightCollection GetLevelWeightCollection()
    {
        return currentLevelWeights;
    }

}
