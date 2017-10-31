using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionButton : MonoBehaviour
{
    // Key used to activate
    public KeyCode MappedKey;

    // Action activated by button
    public CharacterAction MappedAction;

    // Cooldown gauge
    public RectTransform Cooldown;
    private float m_CooldownFull;

    // PrepTime gauge
    public RectTransform PrepTimeGauge;
    private float m_PrepTimeFull;

    // UI asset shortcuts
    private Button m_Button;
    private Image m_Image;

	// Use this for initialization
	void Start ()
    {
        m_Button = GetComponent<Button>();
        m_Image = GetComponent<Image>();

        if (MappedAction)
        {
            m_Image.sprite = MappedAction.Icon;
        }
        else
        {
            m_Button.interactable = false;
        }

        // Store fill values of cooldown and prep time gauges
        if (Cooldown)
            m_CooldownFull = Cooldown.sizeDelta.x;
        if (PrepTimeGauge)
            m_PrepTimeFull = PrepTimeGauge.sizeDelta.x;
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!MappedAction)
            return;

        if (Input.GetKeyUp(MappedKey))
        {
            MappedAction.ActivateAction();
        }

        // If the action has remaining prep time, reflect that prep time in the gauge below the button
        if (MappedAction.RemainingPrepTime > 0f)
        {
            PrepTimeGauge.parent.gameObject.SetActive(true);
            Vector2 sizeDelta = PrepTimeGauge.sizeDelta;

            sizeDelta.x = (1f - (MappedAction.RemainingPrepTime / MappedAction.PrepTime)) * m_PrepTimeFull;

            
            PrepTimeGauge.sizeDelta = sizeDelta;
        }
        else
            PrepTimeGauge.parent.gameObject.SetActive(false);

        if (MappedAction.RemainingCooldown > 0f)
        {
            Cooldown.gameObject.SetActive(true);
            Vector2 sizeDelta = Cooldown.sizeDelta;
            sizeDelta.y = (MappedAction.RemainingCooldown / MappedAction.TotalCooldown) * m_CooldownFull;
            Cooldown.sizeDelta = sizeDelta;
        }
        else
            Cooldown.gameObject.SetActive(false);
	}
}
