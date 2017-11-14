using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenuHandler : MonoBehaviour
{
    public InputField InputName;
    public Button EnterGameButton;

    private string m_CharacterName;

    private void Start()
    {
        m_CharacterName = "";
    }

    void Update()
    {
        if (InputName != null)
            m_CharacterName = InputName.text;

        EnterGameButton.interactable = (m_CharacterName != "");
    }

    public void EnterGame()
    {
        PlayerPrefs.SetString("CharacterName", m_CharacterName);

        Debug.Log(PlayerPrefs.GetString("CharacterName"));
    }
}
