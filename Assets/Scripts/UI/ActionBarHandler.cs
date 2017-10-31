using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ActionBarHandler : MonoBehaviour
{
    private List<ActionButton> m_ActionButtons;
    
    // Use this for initialization
	void Start ()
    {
        m_ActionButtons = new List<ActionButton>();
        HorizontalLayoutGroup layoutGroup = GetComponentInChildren<HorizontalLayoutGroup>();
        for (int i = 0; i < layoutGroup.transform.childCount; i++)
        {
            m_ActionButtons.Add(layoutGroup.transform.GetChild(i).GetComponent<ActionButton>());
        }
	}
	
	// Update is called once per frame
	void Update ()
    {
		
	}

    public void DoAction(ActionButton _action)
    {

    }

    public bool AutoEquipAction(CharacterAction _action)
    {
        foreach(ActionButton button in m_ActionButtons)
        {
            if (!button.MappedAction)
            {
                button.MappedAction = _action;
                return true;
            }
        }

        return false;
    }
}
