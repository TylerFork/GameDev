using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
#if UNITY_EDITOR
using UnityEditor;
#endif


[ExecuteInEditMode()]
public class ProgressBar : MonoBehaviour
{
#if UNITY_EDITOR
    [MenuItem("GameObject/UI/Linear Progress Bar")]
    public static void AddLinearProgressBar()
    {
        GameObject obj = Instantiate(Resources.Load<GameObject>("UI/Linear Progress Bar"));
        obj.transform.SetParent(Selection.activeGameObject.transform, false);
    }
#endif
      
    // todo scoring: can use targetPos as the best score, and can use the fillAmount of the upper/lower sliders as the "best" acceptable scores.
    // any current fill from the player higher/lower than these upper/lower slider fillAmounts would count as 0 score.

    [Header("Slider")]
    [SerializeField] int minimum;
    [SerializeField] int maximum;
    [SerializeField] int current;
    [SerializeField] Image mask;
    [SerializeField] Image fill;
    [SerializeField] Color color;

    [Header("Target")]
    [SerializeField] Image target;
    [SerializeField] float targetPos;
    [SerializeField] Image upperTargetMask;
    [SerializeField] Image upperTargetFill;
    [SerializeField] Color upperTargetColor;
    [SerializeField] float upperTargetCurrent;
    [SerializeField] Image lowerTargetMask;
    [SerializeField] Image lowerTargetFill;
    [SerializeField] Color lowerTargetColor;
    [SerializeField] float lowerTargetCurrent;

    Rect rectangle;

    // Start is called before the first frame update
    void Start()
    {
        // ensure pivots of progress bar are set to x=0
        mask.rectTransform.pivot = new Vector2(0f, mask.rectTransform.pivot.y);
        upperTargetMask.rectTransform.pivot = new Vector2(0f, upperTargetMask.rectTransform.pivot.y);
        lowerTargetMask.rectTransform.pivot = new Vector2(0f, lowerTargetMask.rectTransform.pivot.y);
    }

    // Update is called once per frame
    void Update()
    {
        GetCurrentFill();
        GetTargetFill();
        SetTargetPosition();

        rectangle = gameObject.GetComponent<RectTransform>().rect;
        DisplayRectBounds(new Rect[] { rectangle }, new string[] { "slider"});
    }


    void DisplayRectBounds(Rect[] rects, string[] names)
    {
        string text = "";
        for (int i = 0; i < rects.Length; i++)
        {
            text += names[i] + ": xPos: " + rects[i].position.x.ToString() + ", xMin: " + Mathf.Abs(rects[i].position.x - rects[i].xMin).ToString() + ", xMax: " + Mathf.Abs(rects[i].position.x + rects[i].xMax).ToString();
            text += "\n";
        }
        print(text);
    }

   

    void GetCurrentFill()
    {
        float maximumOffset = maximum - minimum;
        float currentOffset = current - minimum;
        currentOffset = Mathf.Clamp(currentOffset, 0f, maximumOffset);
        float fillAmount = currentOffset / maximumOffset;
        mask.fillAmount = fillAmount;

        fill.color = color;
    }

    void GetTargetFill()
    {
        float upperTargetFillAmount = Mathf.Clamp(upperTargetCurrent / maximum, 0f, (maximum - targetPos)/maximum);
        upperTargetMask.fillAmount = upperTargetFillAmount;
        upperTargetFill.color = upperTargetColor;

        float lowerTargetFillAmount = Mathf.Clamp(lowerTargetCurrent / maximum, 0f, (targetPos)/maximum);
        lowerTargetMask.fillAmount = lowerTargetFillAmount;
        lowerTargetFill.color = lowerTargetColor;
    }

    void SetTargetPosition()
    {

        target.rectTransform.localPosition = new Vector3(targetPos, 0f, 0f);
        

    }

}
