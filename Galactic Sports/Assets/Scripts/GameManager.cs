using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    [SerializeField] TargetLift targetLift;

    public TargetLift GetTargetLift()
    {
        return targetLift;
    }

    
}
