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

    public void EnterGame(bool _server)
    {
        PlayerPrefs.SetString("CharacterName", m_CharacterName);
        PlayerPrefs.SetInt("IsServer", (_server) ? 1 : 0);

        Debug.Log(PlayerPrefs.GetString("CharacterName"));
        SceneManager.LoadScene(1);
    }
}
