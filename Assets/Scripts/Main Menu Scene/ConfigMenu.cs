using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ConfigMenu : MonoBehaviour
{
    [SerializeField]
    GameObject m_MainUIPanel;

    [SerializeField]
    GameObject m_ConfigUIPanel;

    [SerializeField]
    GameObject m_LocalConfigHandler;

    public void GoToConfigMenu()
    {
        m_MainUIPanel.SetActive(false);
        m_ConfigUIPanel.SetActive(true);

        m_LocalConfigHandler
            .GetComponent<LocalConfigHandler>()
            .ExportToCSV();
    }

    public void SaveAndReturn()
    {
        m_MainUIPanel.SetActive(true);
        m_ConfigUIPanel.SetActive(false);

        m_LocalConfigHandler
            .GetComponent<LocalConfigHandler>()
            .ExportToCSV();
    }
}
