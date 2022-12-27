using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    [SerializeField]
    Button m_AdministratorRoleButton;

    [SerializeField]
    Button m_UserRoleButton;

    [SerializeField]
    Text m_errorText;

    /// <summary>
    /// This is test for creating multiple map
    /// </summary>
    [SerializeField]
    InputField m_MapsNumberInputField;

    string mapName;

    void OnEnable()
    {
        m_errorText.gameObject.SetActive(false);
        SetMapsNumber();
    }

    static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void AdministratorRoleButtonPressed()
    {
        ApplyMapsNumber();
        LoadScene("MappingConfigurationScene");
    }

    public void UserRoleButtonPressed()
    {
        ApplyMapsNumber();

        if (CheckIfMapAvailable())
            LoadScene("NewARScene");
            //Debug.Log("Enter AR system");
        else
        {
            m_errorText.gameObject.SetActive(true);
            m_errorText.text = "There is no " +
                mapName +
                " file exists!\nPlease ask the administrator to configure AR map first.";
        }
    }

    bool CheckIfMapAvailable()
    {
        string myWorldMapName;

        int maps_number = GlobalConfig.MapsSelection;
        if (maps_number <= 0) myWorldMapName = "catExample_session.worldmap";
        else
        {
            myWorldMapName = "catExample_session_" + maps_number + ".worldmap";
        }

        string path = Path.Combine(Application.persistentDataPath, myWorldMapName);
        mapName = myWorldMapName;
        return File.Exists(path);
    }

    void ApplyMapsNumber()
    {
        if (m_MapsNumberInputField)
            GlobalConfig.MapsSelection = int.Parse(m_MapsNumberInputField.text);
    }

    void SetMapsNumber()
    {
        if (m_MapsNumberInputField)
            m_MapsNumberInputField.text = GlobalConfig.MapsSelection.ToString();
    }
}
