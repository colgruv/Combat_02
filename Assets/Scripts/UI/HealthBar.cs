using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HealthBar : MonoBehaviour
{
    public RectTransform HealthFill;
    public RectTransform GuardFill;

    private float m_MaxWidth;

	// Use this for initialization
	void Start ()
    {
        m_MaxWidth = HealthFill.sizeDelta.x;
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void DisplayVitals(float _healthWidth, float _guardWidth)
    {
        HealthFill.sizeDelta = new Vector2(_healthWidth * m_MaxWidth, HealthFill.sizeDelta.y);
        GuardFill.sizeDelta = new Vector2(_guardWidth * m_MaxWidth, GuardFill.sizeDelta.y);
    }
}
