using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_NewARScene_MarkerStatusHandler : MonoBehaviour
{    
    [SerializeField]
    Text m_TextMarkerStatus;

    bool markerStatusActive;

    private void Start()
    {
        var status = GetMarkerStatusActive();
        m_TextMarkerStatus.gameObject.SetActive(status);
        markerStatusActive = status;
    }

    public void BtnShowHideMarkerStatus()
    {
        if (markerStatusActive)
        {
            m_TextMarkerStatus.gameObject.SetActive(false);
            markerStatusActive = false;
        }
        else
        {
            m_TextMarkerStatus.gameObject.SetActive(true);
            markerStatusActive = true;
        }
    }

    public void SetMarkerStatusText(string text)
    {
        m_TextMarkerStatus.text = text;
    }

    public string GetMarkerStatusText()
    {
        return m_TextMarkerStatus.text;
    }

    public bool GetMarkerStatusActive()
    {
        return m_TextMarkerStatus.gameObject.activeSelf;
    }
}
