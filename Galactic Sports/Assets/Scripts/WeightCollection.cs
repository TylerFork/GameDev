using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class WeightCollection : MonoBehaviour
{
    [SerializeField] Weight[] weights;
    [SerializeField] AnimationCurve collectionLiftCurve;

    List<float> keyFrameTimes;
    List<float> keyFrameValues;

    // Start is called before the first frame update
    void Start()
    {
        SetCurveFromChildWeights();
    }

    /// <summary>
    /// Return the product of all weight AnimationCurves evaluated at the time <paramref name="input"/>
    /// </summary>
    /// <param name="input"></param>
    /// <returns></returns>
    public float EvaluateAllCurves(float input) 
    {
        float value = 1f;

        foreach (Weight weight in weights)
        {
            value = Mathf.Clamp01(value * weight.GetLiftCurve().Evaluate(input));
        }

        return value;
    }

    /// <summary>
    /// Attempts to detect all keyframes in child weights and only considers the largest values.
    /// </summary>
    void SetCurveFromChildWeights()
    {
        foreach (Weight weight in weights)
        {
            
            foreach (Keyframe key in weight.GetLiftCurve().keys)
            {
                if (CheckIfKeyTimeExists(collectionLiftCurve, key))
                {
                    for (int i = 0; i < collectionLiftCurve.keys.Length; i++)
                    {
                        if (collectionLiftCurve.keys[i].time == key.time)
                        {
                            if (collectionLiftCurve.keys[i].value > key.value)
                            {
                                continue;  
                            } else
                            {
                                collectionLiftCurve.RemoveKey(i);
                                collectionLiftCurve.AddKey(key);
                            }
                        }
                    }

                } else
                {
                    collectionLiftCurve.AddKey(key);
                }
            }
        }
    }

    bool CheckIfKeyTimeExists(AnimationCurve curve, Keyframe key)
    {
        for (int i = 0; i < curve.keys.Length; i++)
        {
            if (curve.keys[i].time == key.time)
            {
                return true;
            }
        }

        return false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //private void OnDrawGizmos()
    //{
    //    SetCurveFromChildWeights();
        
    //}

}
