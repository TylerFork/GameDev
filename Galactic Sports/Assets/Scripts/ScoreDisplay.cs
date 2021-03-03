using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreDisplay : MonoBehaviour
{
    Player player;
    Weight target;
    Text scoreText;

    // Start is called before the first frame update
    void Start()
    {
        scoreText = GetComponent<Text>();
        player = FindObjectOfType<Player>();
        target = FindObjectOfType<Weight>();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = CalculateScore(player, target).ToString();
    }

    private float CalculateScore(Player player, Weight target)
    {
        float score, playerEnergy, targetEnergy, energyPctDiff;
        playerEnergy = player.GetCurrentEnergy();
        targetEnergy = target.GetWeight();

        energyPctDiff = 100f * Mathf.Abs(targetEnergy - playerEnergy) / targetEnergy;

        if ((energyPctDiff >= 50f) && (energyPctDiff < 100f))
        {
            score = 0f;
        } else if ((energyPctDiff >= 30f) && (energyPctDiff < 50f))
        {
            score = 5f;
        } else if ((energyPctDiff >= 15f) && (energyPctDiff < 30f))
        {
            score = 10f;
        } else if ((energyPctDiff >= 5f) && (energyPctDiff < 15f))
        {
            score = 15;
        } else if ((energyPctDiff > 0f) && (energyPctDiff < 5f))
        {
            score = 20f;
        } else if (energyPctDiff == 0f)
        {
            score = 25f;
        } else
        {
            score = -99f;
        }        

        return score;
    }
}
