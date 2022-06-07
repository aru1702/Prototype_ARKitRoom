using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager_CatExample : MonoBehaviour
{
    [SerializeField]
    Button closePanelButton;

    [SerializeField]
    Text m_mapStatus;

    [SerializeField]
    GameObject panel;

    public void ClosePanel()
    {
        panel.SetActive(false);
    }

    public void OpenPanel()
    {
        panel.SetActive(true);
    }

    public Text MapStatus
    {
        get { return m_mapStatus; }
        set { m_mapStatus = value; }
    }
}
