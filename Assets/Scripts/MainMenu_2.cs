using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainMenu_2 : MonoBehaviour
{
    [SerializeField]
    Button m_AdministratorRoleButton;

    [SerializeField]
    Button m_UserRoleButton;

    [SerializeField]
    Text m_errorText;

    [SerializeField]
    InputField m_SaveMapIntoInputField, m_LoadMapInputField;

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
        {
            LoadScene("NewARScene");
            //Debug.Log("Enter AR system");
        }
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

        int maps_number = GlobalConfig.LOAD_MAP;
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
        if (m_SaveMapIntoInputField && m_LoadMapInputField)
        {
            GlobalConfig.SAVE_INTO_MAP = int.Parse(m_SaveMapIntoInputField.text);
            GlobalConfig.LOAD_MAP = int.Parse(m_LoadMapInputField.text);
            GlobalConfig.MapsSelection = GlobalConfig.LOAD_MAP;
        }
    }

    void SetMapsNumber()
    {
        if (m_SaveMapIntoInputField && m_LoadMapInputField)
        {
            m_SaveMapIntoInputField.text = GlobalConfig.SAVE_INTO_MAP.ToString();
            m_LoadMapInputField.text = GlobalConfig.LOAD_MAP.ToString();
        }
    }
}
