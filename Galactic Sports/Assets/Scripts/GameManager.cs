using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] WeightCollection currentLevelWeights;

    public WeightCollection GetLevelWeightCollection()
    {
        return currentLevelWeights;
    }

    
}
