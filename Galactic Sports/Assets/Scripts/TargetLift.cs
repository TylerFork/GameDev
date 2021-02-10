using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class TargetLift : MonoBehaviour
{
    // configuration parameters
    [Header("Properties")]
    [SerializeField] Transform target;
    [SerializeField] float weight = 75f;
    [SerializeField] float successLiftingTime = 3f;


    private void Start()
    {
        target.transform.localPosition = new Vector2(0f, weight) * target.GetComponentInParent<RectTransform>().rect.height / target.GetComponentInParent<Slider>().maxValue;
    }

    public float GetWeight()
    {
        return weight;
    }

    public float GetSuccessLiftingTime()
    {
        return successLiftingTime;
    }





}

