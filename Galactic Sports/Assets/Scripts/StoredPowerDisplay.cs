using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StoredPowerDisplay : MonoBehaviour
{
    Text storedPowerText;
    
    // Start is called before the first frame update
    void Start()
    {
        storedPowerText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        storedPowerText.text = FindObjectOfType<Player>().GetStoredPulse().ToString("0");
    }
}
