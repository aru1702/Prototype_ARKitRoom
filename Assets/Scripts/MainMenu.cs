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

    string myWorldMapName = "catExample_session.worldmap";

    void OnEnable()
    {
        m_errorText.gameObject.SetActive(false);
    }

    static void LoadScene(string sceneName)
    {
        SceneManager.LoadScene(sceneName, LoadSceneMode.Single);
    }

    public void AdministratorRoleButtonPressed()
    {
        LoadScene("MappingConfigurationScene");
    }

    public void UserRoleButtonPressed()
    {
        if (CheckIfMapAvailable())
            //LoadScene("NewARScene");
            Debug.Log("Enter AR system");
        else
        {
            m_errorText.gameObject.SetActive(true);
            m_errorText.text = "There is no map file exists!\nPlease ask the administrator to configure AR system map.";
        }
    }

    bool CheckIfMapAvailable()
    {
        string path = Path.Combine(Application.persistentDataPath, myWorldMapName);
        return File.Exists(path);
    }
}
