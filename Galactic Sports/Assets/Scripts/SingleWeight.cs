using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SingleWeight : MonoBehaviour
{
    [SerializeField] float weight = 75f;

    public float GetWeight()
    {
        return weight;
    }
    
}
