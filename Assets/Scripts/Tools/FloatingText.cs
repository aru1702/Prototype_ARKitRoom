using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloatingText : MonoBehaviour
{
    [SerializeField]
    GameObject m_QuadPanel;

    [SerializeField]
    TextMesh m_InsideText;

    bool panelOpened = false;

    public void OpenPanel()
    {
        m_QuadPanel.SetActive(true);
        panelOpened = true;
    }

    public void ClosePanel()
    {
        m_QuadPanel.SetActive(false);
        panelOpened = false;
    }

    public void OpenClosePanel()
    {
        if (!panelOpened)
        {
            OpenPanel();
        }
        else
        {
            ClosePanel();
        }
    }

    public void SetText(string text)
    {
        m_InsideText.text = text;
    }

    public string GetText()
    {
        return m_InsideText.text;
    }
}
