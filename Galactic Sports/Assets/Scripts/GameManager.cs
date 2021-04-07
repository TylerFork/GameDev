using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManager : MonoBehaviour
{
    [SerializeField] WeightCollection currentLevelWeights;
    [SerializeField] Text scoreText;

    Player player;

    void Start()
    {
        player = FindObjectOfType<Player>();
    }

    private void Update()
    {
        if (player.GetPlayerPressedConfirmButton())
        {
            scoreText.text = CalculateScore(player, currentLevelWeights).ToString();

        }

    }

    public WeightCollection GetLevelWeightCollection()
    {
        return currentLevelWeights;
    }

    private float CalculateScore(Player player, WeightCollection target)
    {
        float minScoringRange, maxScoringRange, playerEnergy, targetEnergy, playerEnergyRatioAwayFromTarget;
        playerEnergy = player.GetCurrentEnergy();
        targetEnergy = target.GetTotalWeight();
        minScoringRange = target.GetScoreRange().Item1;
        maxScoringRange = target.GetScoreRange().Item2;


        if ((playerEnergy < minScoringRange) | (playerEnergy > maxScoringRange))
        {
            return 0f;
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

        return Mathf.Lerp(100f, 0f, playerEnergyRatioAwayFromTarget);
    }

}
