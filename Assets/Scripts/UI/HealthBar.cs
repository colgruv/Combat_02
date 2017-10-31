using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public RectTransform HealthFill;
    public RectTransform GuardFill;

	// Use this for initialization
	void Start ()
    {
		
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void DisplayVitals(float _healthWidth, float _guardWidth)
    {
        HealthFill.sizeDelta = new Vector2(_healthWidth, 1f);
        GuardFill.sizeDelta = new Vector2(_guardWidth, 1f);
    }
}
