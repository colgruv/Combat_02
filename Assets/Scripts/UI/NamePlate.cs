using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class NamePlate : MonoBehaviour
{
    private NetworkCharacterData m_CharacterData;
    private Text m_NamePlateText;

	// Use this for initialization
	void Start ()
    {
        m_CharacterData = transform.parent.parent.GetComponent<NetworkCharacterData>();
        m_NamePlateText = GetComponent<Text>();
	}

    void Update()
    {
        if (m_CharacterData.CharacterName != null)
        {
            m_NamePlateText.text = m_CharacterData.CharacterName;
        }
    }
}
