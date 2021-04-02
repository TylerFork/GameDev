using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class WeightCollection : MonoBehaviour
{
    // configuration parameters
    [Header("Properties")]
    [Tooltip("Enter the Rect Transform of the target power in Slider")] [SerializeField] RectTransform target;
    [SerializeField] AnimationCurve liftCurve = AnimationCurve.EaseInOut(0,0,1,1);
    [Range(15f, 85f)] [SerializeField] float weight;
    [Range(5f, 40f)] [SerializeField] float weightDifficulty = 5f;
    [SerializeField] float successLiftingTime = 3f;

    Tuple<float, float> ScoreRange;

    private void Awake()
    {
        SetTotalWeight();
        
    }

    private void Start()
    {
        ScoreRange = SetWeightTargetAndDifficulty();
        print(ScoreRange.Item1.ToString() + " <--> " + ScoreRange.Item2.ToString());
    }

    private Tuple<float, float> SetWeightTargetAndDifficulty()
    {
        float maximumSliderValue = target.GetComponentInParent<Slider>().maxValue;
        float sliderHeight = target.GetComponentInParent<Slider>().GetComponent<RectTransform>().rect.height;
        float sliderRectHeightToMaxValueRatio = sliderHeight / maximumSliderValue;
        float weightUpperBound = (weight) * sliderRectHeightToMaxValueRatio + weightDifficulty;
        float weightLowerBound = (weight) * sliderRectHeightToMaxValueRatio - weightDifficulty;

        if (weightUpperBound > maximumSliderValue * 2)
        {
            float fixedScale = 0.5f*(maximumSliderValue*sliderRectHeightToMaxValueRatio - weightLowerBound);
            float fixedPos = 0.5f*(maximumSliderValue * sliderRectHeightToMaxValueRatio + weightLowerBound);
            target.localPosition = new Vector2(0f, fixedPos);
            target.localScale = new Vector2(10f, fixedScale * 2f);
            //print("weightUpperBound: " + (fixedPos + fixedScale*0.5f).ToString() + ", weightLowerBound: " + (fixedPos - fixedScale * 0.5f).ToString());
            return new Tuple<float, float>(fixedPos + fixedScale * 0.5f, fixedPos - fixedScale * 0.5f);

        } else if (weightLowerBound < 0f * 2)
        {
            float fixedScale = 0.5f * (weightUpperBound - 0f * sliderRectHeightToMaxValueRatio);
            float fixedPos = 0.5f * (weightUpperBound + 0f * sliderRectHeightToMaxValueRatio);
            target.localPosition = new Vector2(0f, fixedPos);
            target.localScale = new Vector2(10f, fixedScale * 2f);
            //print("weightUpperBound: " + (fixedPos + fixedScale * 0.5f).ToString() + ", weightLowerBound: " + (fixedPos - fixedScale * 0.5f).ToString());
            return new Tuple<float, float>(fixedPos + fixedScale * 0.5f, fixedPos - fixedScale * 0.5f);


        } else
        {
            target.localPosition = new Vector2(0f, weight) * sliderHeight / maximumSliderValue;
            target.localScale = new Vector2(10f, weightDifficulty * 2f);
            //print("weightUpperBound: " + weightUpperBound.ToString() + ", weightLowerBound: " + weightLowerBound.ToString());
            return new Tuple<float, float>(weightUpperBound, weightLowerBound);

        }

    }

    //Tuple<float, float> CalculateUpperAndLowerBounds(float bar, float buffer)
    //{
    //    return new Tuple<float, float>(bar + buffer, bar - buffer);
    //}

    void SetTotalWeight()
    {
        float totalWeight = 0f;
        foreach (Transform weight in transform)
        {
            totalWeight += weight.gameObject.GetComponent<SingleWeight>().GetWeight();
        }

        if (totalWeight > 100f)
        {
            weight = 100f;
        } else
        {
            weight = totalWeight;
        }
    }

    public float GetTotalWeight()
    {
        return weight;
    }

    public float GetSuccessLiftingTime()
    {
        return successLiftingTime;
    }

    public AnimationCurve GetLiftCurve()
    {
        return liftCurve;
    }
    
    public Tuple<float, float> GetScoreRange()
    {
        return ScoreRange;
    }




}

